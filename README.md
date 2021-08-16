# Hyperspace-Cheese-Battle
Board game as first-semester final project of Rob Mile's C# "The Yellow Book".

This game is not directly included in the aforementioned book. You need to go to Rob Mile's webpage and download his courses' slides, weekly lab work and semester courseworks.

This game pitches 4 players against each other in a board of 7x7 squares. Each player moves by throwing a virtual 6-sided dice and following the square's limitations as signed by the appropriate arrows in order to get to the goal square.
A full explanation of the game's rules and characteristics appear at the beginning of running the program.

I implemented all extra-credit features of the project, namely:
- 3 AI characters with distinctive behaviour when landing on a cheese square:
  - Speedy Steve will always choose to throw the dice again.
  - Angry Allan will always shoot a player at random.
  - Clever Trevor will shoot the player nearest to the goal square or throw the dice again if he's in the lead.
- Power of Six: throwing a six grants you a new throw. Getting a 6 three times in a row results in the player falling to the bottom row.
- Board display: a board with ASCII characters has been implemented. Players are represented by their player number, cheese squares are represented by '* ' and the limiting arrows can be seen in each square. 

