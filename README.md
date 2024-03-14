# Better XD Shadow Pokémon Randomizer
A randomizer for Pokémon XD that changes every Shadow in the game with better movesets. All the .csv and .txt files are necessary for the program to work (all comes with the releases).

Creates two new files: **[NEW] Shadow Pokémon.csv** and **[NEW] Trainer Pokemon DeckData_Story.bin**. Take out the [NEW] from the names and replace the corresponding files extracted from the game using the GoD Tool.

It follows the following convention for moves:
* 1 level-up move
* 1 TM move
* 1 breeding move
* 1 Tutor or Purification move

The randomizer doesn't pick duplicates for Pokémon and moves. It also has failsafes for if a Pokémon has no Egg Moves, learns no TMs or no move tutor moves, but weird Pokémon (like Caterpie, Magikarp, etc) will not have 4 moves. Ditto and Smeargle will have Transform & Sketch, respectively, along with a purification move (it would be kinda hard to exclude these two from the picks). 

The list of egg moves for evolved Pokémon has been updated to include the moves learned by pre-evolutions. If the Pokémon has no egg moves, it picks from all the level-up moves the Pokémon can learn after the level it's at. Some special Pokémon like Metapod and other cocoon Pokémon had "egg moves" added that correspond to the moves their pre-evolution, just to make sure they have 4 moves in total and don't start with Harden only. Legendaries also had a single new "egg move" added (most have Extrasensory, but the Regis had some unique picks).

**tutor_moves.csv** is the list of all move tutors moves all Pokémon in the game can learn in Gen 3 (including but not limited to XD's move tutor)

**purification_moves.txt** is a list of the generic special moves Shadow Pokémon get at purification. It contains most of these moves (excluding the unique ones legendaries get and Morning Sun, plus the addition of Encore). Feel free to change this list as desired (using the proper move names as they appear in the list of moves extracted from the game), but keep in mind any Pokémon can learn any of these moves.

New in v1.1 is a Shadow move randomizer that tries to give each Pokémon a damage-dealing move appropriate for its level on the 1st slot, a high chance of having a status move on the 2nd slot, and also distributes Shadow End and the birds' signature moves rarely to strong Pokémon (these last ones based on compatibility with Ice Beam / Flamethrower / Thunderbolt). More explanation below if you're interested in changing the parameters.

## Options
To change the options you have to compile the program yourself (I might change this for a later release if there's enough interest)

* **Skip Lugia:** skips changing the Shadow Lugia (by default is true)
* **Skip Togepi:** skips changing the gift Togepi (by default is true)
* **Similar BST:** only picks a new random Shadow from the pool of Pokémon with a Base Stat Total within a range of the original Pokémon (by default it's true)
* **BSTRange:** the range of the Base Stat Total for picking a new Shadow. It considers the original BST +- the range. By default, it's 50 (so a Pokémon with 500 BST can be replaced by any Pokémon with 450 to 550 BST, etc).
* **MoveTutorPercentageAsPurificationMoves:** how likely the 4th move is to be a move tutor move instead of the purification move. A 1 means that all moves on the 4th slot will be a move tutor move and 0 means that all moves in the 4th slot will be a purification move (by default it's 0.8)

## Shadow Move randomizer
Each Pokémon has a guaranteed damage-dealing move in the first slot, a 90% chance of a status move in the 2nd slot, and equal chance of a status or damage-dealing move for the other two slots (if applicable).

Generic damage-dealing Shadow moves are divided into physical (Blitz, Rush, and Break) and special (Wave, Rave, and Storm). Each Pokémon can only have one move from each category. The chance of getting a physical or special move depends on the ratio of the Pokémon's Attack to its Special Attack (so if they are the same the chance is 50%); this chance follows a logistic distribution that can be controlled with *PhysicalSpecialTolerance* (default 8). The chance of getting a weak, medium, or strong move follows a Poisson distribution for each, which are then normalized. The parameter that controls these distributions is the mean of the Poisson distribution *PoissonLambdaWeak*, *PoissonLambdaMid*, *PoissonLambdaStrong* that corresponds to a level (default values 22, 32, and 48, respectively). The default distribution of these types of moves is shown in this graph:

![image](https://github.com/TriteHexagon/XDRandomizer/assets/37734530/d5a30a74-705c-4100-9c59-616c0349706e)

Status moves are picked randomly from a list appropriate for the level of the Pokémon. Mist, Hold, and Shed are always on the list; Panic and Sky are added for Pokémon at or above *ShadowPanicSkyLevel* (default level 19); Half and Down are added to the list above *ShadowHalfDownLevel* (default level 33).

Shadow End is considered as a possible move only if the check for a physical strong move is passed. Then another random check is made against *ShadowEndProb*. This value changes based on the move slot: 15% for the first two moves and 35% for the last two.

Shadow Fire, Chill, and Bolt work similarly to Shadow End, and are only considered when a strong special move is picked, and then against *SpecialSignMoveProb* (50% for the first two move slots, 70% for the third and 4th move slots). However, there's a further compatibilty check against the moves they are based on (Flamethrower, Ice Beam, and Thunderbolt). If more than one move is compatible, it's picked then randomly.

## Known Bugs
* Maybe it doesn't work with Mr. Mime? I think the game *could* potentially still work since it does get the number, but more testing could be done.
