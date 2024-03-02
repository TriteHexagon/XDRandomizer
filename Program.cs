using System.Numerics;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

//options
bool SkipLugia = true;
bool SkipTogepi = true;
bool SimilarBST = false;
double MoveTutorPercentageAsPurificationMoves = 0.8; //only between 0 and 1 please
int BSTRange = 50;

string path = Directory.GetCurrentDirectory();

string ShadowListPath = path + "//" + "Shadow Pokemon.csv";
string PKMNStatPath = path + "//" + "Pokemon Stats.csv";
string TMPath = path + "//" + "TM Or HM.csv";
string TutorPath = path + "//" + "tutor_moves.csv";
string StoryDeckPath = path + "//" + "Trainer Pokemon DeckData_Story.bin.csv";
string PurificationMovesPath = path + "//" + "purification_moves.txt";

string NewShadowListPath = path + "//" + "[NEW] Shadow Pokemon.csv";
string NewStoryDeckPath = path + "//" + "[NEW] Trainer Pokemon DeckData_Story.bin.csv";

if (File.Exists(NewShadowListPath))
    File.Delete(NewShadowListPath);

if (File.Exists(NewStoryDeckPath))
    File.Delete(NewStoryDeckPath);

//Define pattern
Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

StreamWriter NewShadowList = new StreamWriter(NewShadowListPath);

List<string[]> NewMasterShadowList = new List<string[]>();

using (SkippableStreamReader reader1 = new SkippableStreamReader(ShadowListPath))
{
    //
    //we skip the first two line (header + blank shadow)
    NewShadowList.WriteLine(reader1.ReadLine());
    NewShadowList.WriteLine(reader1.ReadLine());
    string line = reader1.ReadLine();

    string[] ShadowListLine = CSVParser.Split(line);

    while (ShadowListLine[0] != "Shadow Pokemon - 84") //hard-coded for now
    {
        string ShadowPKMN = ShadowListLine[5];

        //we skip the shadows we don't want to change
        if (SkipLugia && ShadowPKMN == "LUGIA  (699)")
        {
            NewShadowList.WriteLine(line);
            goto shadowskip;
        }

        if (SkipTogepi && ShadowPKMN == "TOGEPI  (1)")
        {
            NewShadowList.WriteLine(line);
            goto shadowskip;
        }

        //initialize the lists of moves

        List<string> LevelUpMoveList = new List<string>();
        List<string> BackupMoveList = new List<string>();
        List<string> EggMoveList = new List<string>();
        List<string> TMMoveList = new List<string>();
        List<string> TutorMoveList = new List<string>();

        //Console.WriteLine("Shadow Level is " + ShadowListLine[3] + ShadowListLine[0]);
        int ShadowLevel = Int32.Parse(ShadowListLine[3]);

        (string ShadowPKMNName, int ShadowPKMNPositionInStoryDeck) = NameAndIDExtractor(ShadowPKMN);

        Console.WriteLine("Number: {0} | Name: {1}", ShadowPKMNPositionInStoryDeck, ShadowPKMNName);

        //we add the positions of the shadow to be changed to the master list

        //we are going to generate a new random mon
        Random rnd = new Random();
        int RandomPKMNNumber = 0;

        SkippableStreamReader reader2 = new SkippableStreamReader(PKMNStatPath);

        string[] PKMNStatLine = CSVParser.Split(reader2.ReadLine());
        int BSTTotal = 0;

        //get the BST of the current Pokémon
        if (SimilarBST)
        {
            bool FoundThePokémon = false;
            
            while (!FoundThePokémon)
            {   
                (string PKMNStatLineName, int PKMNStatLineNameID) = NameAndIDExtractor(PKMNStatLine[0]);
                if (ShadowPKMNName == PKMNStatLineName)
                {
                    BSTTotal = Int32.Parse(PKMNStatLine[103]) + Int32.Parse(PKMNStatLine[104]) + Int32.Parse(PKMNStatLine[105]) +
                        Int32.Parse(PKMNStatLine[106]) + Int32.Parse(PKMNStatLine[107]) + Int32.Parse(PKMNStatLine[108]);
                    FoundThePokémon = true;
                    Console.WriteLine("Found the Pokémon! BST is {0}", BSTTotal);
                }
                PKMNStatLine = CSVParser.Split(reader2.ReadLine());
            }
            reader2.Close();
        }

        bool PKMNApproved = false;
        bool BSTapproved = true;
        if (SimilarBST)
            BSTapproved = false;
        string NewShadowNameIndexNoName = "None";
        int NewShadowNameIndexNoNo = 0;

        while (!PKMNApproved)
        {
            RandomPKMNNumber = rnd.Next(2, 413);
            while (RandomPKMNNumber >= 253 && RandomPKMNNumber <= 277)
                RandomPKMNNumber = rnd.Next(2, 413);

            reader2 = new SkippableStreamReader(PKMNStatPath);
            reader2.SkipLines(RandomPKMNNumber);

            PKMNStatLine = CSVParser.Split(reader2.ReadLine());

            string NewShadowName = PKMNStatLine[0];
            //change format of the Shadow Name
            (NewShadowNameIndexNoName, NewShadowNameIndexNoNo) = NameAndIDExtractor(NewShadowName);

            if (SimilarBST)
            {
                int NewShadowBSTTotal = Int32.Parse(PKMNStatLine[103]) + Int32.Parse(PKMNStatLine[104]) + Int32.Parse(PKMNStatLine[105]) 
                    + Int32.Parse(PKMNStatLine[106]) + Int32.Parse(PKMNStatLine[107]) + Int32.Parse(PKMNStatLine[108]);
                //Console.WriteLine("Shadow Attempt is {0}. BST is {1}",PKMNStatLine[0], NewShadowBSTTotal);

                if (Math.Abs(BSTTotal - NewShadowBSTTotal) <= BSTRange)
                    BSTapproved = true;
                else
                {
                    reader2.Close();
                    reader2 = new SkippableStreamReader(PKMNStatPath);
                }
                    
            }
            
            if (BSTapproved)
                PKMNApproved = true;

            //we check if the species we chose hasn't already been added to the list
            for (int i =0; i<NewMasterShadowList.Count; i++)
            {
                (string NameToCompare, int IdontCare) = NameAndIDExtractor(NewMasterShadowList.ElementAt(i)[1]);
                if (NameToCompare == NewShadowNameIndexNoName)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Duplicate!");
                    Console.ResetColor();
                    PKMNApproved = false;
                }        
            }
        }

        string NewShadowNameIndexNo = NewShadowNameIndexNoName + " (" + NewShadowNameIndexNoNo + ")";

        //Console.WriteLine("New Random Shadow Pokémon is {0}, No. {1}", NewShadowNameIndexNo, RandomPKMNNumber);

        //new now add back the changed Shadow Pokémon to the list
        // but we want to keep the position name!
        string NewShadowNameStoryDeckNo = NewShadowNameIndexNoName + " - " + ShadowPKMNPositionInStoryDeck;
        ShadowListLine[5] = NewShadowNameStoryDeckNo;

        string newShadowLine = string.Join(",", ShadowListLine);

        NewShadowList.WriteLine(newShadowLine);
        //=======
        //start dealing with moves

        //=======
        //Level-Up move
        //first lv up move 131 | last - 168

        int PositionOfCurrentLevelUpMove = 0;
        int MoveLevel = 1;
        while (MoveLevel != 0)
        {                     
            if (MoveLevel <= ShadowLevel)
            {
                LevelUpMoveList.Add(PKMNStatLine[130 + 2 * PositionOfCurrentLevelUpMove + 1]);
            }
            else
            {
                BackupMoveList.Add(PKMNStatLine[130 + 2 * PositionOfCurrentLevelUpMove + 1]);
            }
            PositionOfCurrentLevelUpMove++;
            MoveLevel = Int32.Parse(PKMNStatLine[130 + 2 * PositionOfCurrentLevelUpMove]);
        }

        //Console.WriteLine("===Level Up List===");
        //ShowList(LevelUpMoveList);
        //Console.WriteLine("===Backup Level Up List===");
        //ShowList(BackupMoveList);

        //=======
        //breeding moves
        int PositionOfLastBreedingMove = 0;
        while (PKMNStatLine[95 + PositionOfLastBreedingMove] != "None (0)")
        {
            EggMoveList.Add(PKMNStatLine[95 + PositionOfLastBreedingMove]);
            PositionOfLastBreedingMove++;
            if (PositionOfLastBreedingMove > 7)
                break;
        }

        //Console.WriteLine("===Egg List===");
        //ShowList(EggMoveList);

        //=======
        //TM moves
        SkippableStreamReader reader3 = new SkippableStreamReader(TMPath);
        reader3.ReadLine(); // skip header
        int[] ListOfValidTMPositions = new int[57];
        bool LearnsTMMoves = false;
        for (int TMPosition = 23; TMPosition <= 23+57;TMPosition++)
        {
            string line3 = reader3.ReadLine(); //always read the next line in the TM List
            if (PKMNStatLine[TMPosition] == "TRUE")
            {
                LearnsTMMoves = true;
                string[] TMMoveListLine = CSVParser.Split(line3);
                TMMoveList.Add(TMMoveListLine[2]);
            }
        }

        //Console.WriteLine("===TM List===");
        //ShowList(TMMoveList);

        //======
        //tutormoves
        SkippableStreamReader reader4 = new SkippableStreamReader(TutorPath);
        reader4.SkipLines(RandomPKMNNumber - 2);
        string line4 = reader4.ReadLine();
        string[] TutorMoveListLine = CSVParser.Split(line4);
        bool LearnsTutorMoves = false;
        foreach (string move in TutorMoveListLine)
        {
            if (move != "")
            {
                LearnsTutorMoves = true;
                TutorMoveList.Add(move);
            }     
        }

        //Console.WriteLine("===Tutor List===");
        //ShowList(TutorMoveList);

        //generate the random moveset
        int RandomNumber = rnd.Next(0, LevelUpMoveList.Count);

        string LevelUpMove = LevelUpMoveList.ElementAt(RandomNumber);
        string TMMove = "None (0)";
        string EggMove = "None (0)";
        string TutorOrPurificationMove = "None (0)";

        int EternalPunishment = 0;
        if (EggMoveList.Count > 0)
        {
            RandomNumber = rnd.Next(0, EggMoveList.Count);
            EggMove = EggMoveList.ElementAt(RandomNumber);
        }
        else
        {
            if (BackupMoveList.Count>0)
            {
                RandomNumber = rnd.Next(0, BackupMoveList.Count);
                EggMove = BackupMoveList.ElementAt(RandomNumber);
            }
            else if (LevelUpMoveList.Count > 1)
            //if we don't have another move, we just keep trying for another level-up move at our current level
            {
                EggMove = LevelUpMove; //we do this to keep inside the while loop until we find a suitable egg move
                
                while (EggMove == LevelUpMove)
                {
                    RandomNumber = rnd.Next(0, LevelUpMoveList.Count);
                    EggMove = LevelUpMoveList.ElementAt(RandomNumber);
                    EternalPunishment++;
                    if (EternalPunishment > 20)
                    {
                        EternalPunishment = 0;
                        break;
                    }
                }  
            }
        }

        if (LearnsTMMoves)
        {
            TMMove = LevelUpMove; //we do this to avoid learning a TM we already know
            while (TMMove == LevelUpMove)
            {
                RandomNumber = rnd.Next(0, TMMoveList.Count);
                TMMove = TMMoveList.ElementAt(RandomNumber);
                EternalPunishment++;
                if (EternalPunishment > 20)
                {
                    EternalPunishment = 0;
                    break;
                }     
            }   
        }

        //pick purification move even if we will not use it, for the Pokémon that don't learn any tutor moves

        SkippableStreamReader PurificationMovesList = new SkippableStreamReader(PurificationMovesPath);

        TutorOrPurificationMove = LevelUpMove;

        while (TutorOrPurificationMove == LevelUpMove)
        {
            PurificationMovesList.SkipLines(rnd.Next(0, TotalLines(PurificationMovesPath)));
            TutorOrPurificationMove = PurificationMovesList.ReadLine();
            EternalPunishment++;
            if (EternalPunishment > 20)
            {
                EternalPunishment = 0;
                break;
            }
        }

        //we already have chosen a purification move, but if the move tutor option is available, we replace it with a random chance

        if (rnd.NextDouble() < MoveTutorPercentageAsPurificationMoves && LearnsTutorMoves)
        {
            TutorOrPurificationMove = LevelUpMove; //we do this to avoid learning a tutor move we already know
            while (TutorOrPurificationMove == LevelUpMove)
            {
                RandomNumber = rnd.Next(0, TutorMoveList.Count);
                TutorOrPurificationMove = TutorMoveList.ElementAt(RandomNumber);
                EternalPunishment++;
                if (EternalPunishment > 20)
                {
                    EternalPunishment = 0;
                    break;
                }
            }
        }

        string[] tempString = { ShadowPKMNPositionInStoryDeck.ToString(), NewShadowNameStoryDeckNo, NewShadowNameIndexNo, LevelUpMove, EggMove, TMMove, TutorOrPurificationMove };
        NewMasterShadowList.Add(tempString);
        //Console.WriteLine("Final Moveset: {0}, {1}, {2}, {3}", LevelUpMove, EggMove, TMMove, TutorOrPurificationMove);

    shadowskip:;

        //read next line in the list of shadows
        line = reader1.ReadLine();
        ShadowListLine = CSVParser.Split(line);
        Console.WriteLine();
    }
}

NewShadowList.Close();
ShowArrayList(NewMasterShadowList);

StreamWriter NewStoryDeckList = new StreamWriter(NewStoryDeckPath);
//we finally create a new story deck list
using (SkippableStreamReader StoryDeckReader = new SkippableStreamReader(StoryDeckPath))
{
    //we read the header right away
    string line = StoryDeckReader.ReadLine();
    NewStoryDeckList.WriteLine(line);
    line = StoryDeckReader.ReadLine();

    int PositionInStoryDeck = 0;
    while (line != null)
    {
        PositionInStoryDeck++;
        string[] StoryDeckLine = CSVParser.Split(line);

        if (NewMasterShadowList.Count > 0)
        {
            for (int i = 0; i < NewMasterShadowList.Count; i++)
            {
                if (NewMasterShadowList.ElementAt(i)[0] == (PositionInStoryDeck - 1).ToString())
                {
                    StoryDeckLine[0] = NewMasterShadowList.ElementAt(i)[1];
                    StoryDeckLine[1] = NewMasterShadowList.ElementAt(i)[2];
                    StoryDeckLine[19] = NewMasterShadowList.ElementAt(i)[3];
                    StoryDeckLine[20] = NewMasterShadowList.ElementAt(i)[4];
                    StoryDeckLine[21] = NewMasterShadowList.ElementAt(i)[5];
                    StoryDeckLine[22] = NewMasterShadowList.ElementAt(i)[6];
                    //we remove this shadow (already dealt with)
                    NewMasterShadowList.RemoveAt(i);
                    break;
                }
            }
        }
        string newShadowLine = string.Join(",", StoryDeckLine);
        NewStoryDeckList.WriteLine(newShadowLine);
        line = StoryDeckReader.ReadLine();
    }   
}

NewStoryDeckList.Close();

Console.WriteLine("Finished!");
Console.ReadLine();

static void ShowList(List<string> ListToShow)
{
    foreach (string element in ListToShow)
    {
        Console.WriteLine(element);
    }
}

static void ShowArrayList(List<string[]> ListToShow)
{
    foreach (string[] element in ListToShow)
    {
        foreach (string element2 in element)
            Console.Write(element2 + " ,");
        Console.WriteLine();
    }
}

int TotalLines(string filePath)
{
    using (StreamReader r = new StreamReader(filePath))
    {
        int i = 0;
        while (r.ReadLine() != null) { i++; }
        return i;
    }
}

(string, int) NameAndIDExtractor(string name)
{
    //doesn't work with Mr. Mime ahhhhhhhhhh
    string[] NameSplit = name.Split(' ');

    //foreach (string element in NameSplit)
    //{
    //    Console.Write(element + ",");
        
    //}
    //Console.WriteLine();

    if (NameSplit.Length == 3)
    {
        Match m1 = Regex.Match(NameSplit[2], "[0-9]{1,3}", RegexOptions.IgnoreCase);
        if (m1.Success)
            return (NameSplit[0], Int32.Parse(m1.Value));
        else
            return (NameSplit[0], 1000);
    }
    
    else if (NameSplit.Length == 4)
    {
        Match m2 = Regex.Match(NameSplit[3], "[0-9]{1,3}", RegexOptions.IgnoreCase);
        if (m2.Success)
            return (NameSplit[0], Int32.Parse(m2.Value));
        else
            return (NameSplit[0], 5000);
    }
    else
        return (NameSplit[0], 9999);
}

class SkippableStreamReader : StreamReader
{
    public SkippableStreamReader(string path) : base(path) { }

    public void SkipLines(int linecount)
    {
        for (int i = 0; i < linecount; i++)
        {
            this.ReadLine();
        }
    }
}
