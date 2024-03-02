# XD Shadow Pokémon Randomizer
A randomizer for Pokémon XD that changes every Shadow in the game. All the .csv and .txt files are necessary for the program to work.

Creates two new files: **[NEW] Shadow Pokémon.csv** and **[NEW] Trainer Pokemon DeckData_Story.bin**. Take out the [NEW] from the names and replace the corresponding files extracted from the game using the GoD Tool.

It follows the following convention for moves:
* 1 level-up move
* 1 TM move
* 1 breeding move
* 1 Tutor or Purification move

The randomizer doesn't pick duplicates for Pokémon and moves. It also has failsafes for if a Pokémon has no Egg Moves, learns no TMs or no move tutor moves, but weird Pokémon (like Caterpie, Magikarp, etc) will not have 4 moves.

The list of egg moves for evolved Pokémon has been updated to include the moves learned by pre-evolutions. If the Pokémon has no egg moves, it picks from all the level-up moves the Pokémon can learn after the level it's at.

Legendaries also had a single new "egg move" added (most have Extrasensory, but the Regis had some unique picks)

**tutor_moves.csv** is the list of all move tutors moves all Pokémon in the game can learn in Gen 3 (including but not limited to XD's own move tutor)

**purification_moves.txt** is a list of the generic special moves Shadow Pokémon get at purification. It contains most of these moves (excluding the unique ones legendaries get and Morning Sun, plus the addition of Encore). Feel free to change this list as desired (using the proper move names as they appear in the list of moves extracted from the game), but keep in mind any Pokémon can learn any of these moves.

## Options
* **Skip Lugia:** skips changing the Shadow Lugia (by default is false)
* **Skip Togepi:** skips changing the gift Togepi (by default is false)
* **Similar BST:** only picks a new random Shadow from the pool of Pokémon with a Base Stat Total within a range of the original Pokémon (by default it's true)
* **BSTRange:** the range of the Base Stat Total for picking a new Shadow. It considers the original BST +- the range. By default it's 50 (so a Pokémon with 500 BST can be replaced by any Pokémon with 450 to 550 BST, etc).
* **MoveTutorPercentageAsPurificationMoves:** how likely the 4th move is to be a move tutor move instead of the purification move. So a 1 means that all moves on the 4th slot will be a move tutor move and 0 means that all moves in the 4th slot will be a purification move (by default it's 0.8)


