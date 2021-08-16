/*Program for the game "Hyperspace Cheese Battle" corresponding to the end of the first semester of the course
 Version 1.0
    Implemented the code for to manage the position and movement of the players
Version 2.0
    Implemented Full player behaviour inluding cheese squares.
    Implemented test mode for the exam test
    Implemented 1d6 dice for normal play
    Implemented full gameplay loop. The game is now fully playable.
Version 3.0
    Implemented enhancement "Power of Six"
    Implemented AI Players
Version 4.0
    Implemented UNICODE board
Created by OuterGazer*/

using System;
using System.Data;
using System.Runtime.InteropServices;

class Hyperspace_Cheese_Battle
{
    static int numberOfPlayers; //Variable that holds how many players are playing the current game
    static int numberOfAIPlayers = 0; //Variable that holds the number of AI opponents in the game    
    static int diceValuePos = 0; //used in test mode    
    static int futurePositionX; //two temporary square placements used when a player lands in an occupied square
    static int futurePositionY;
    static int playerToShoot; //variable that holds which player will be shot when someone uses the cheese power

    static string testMode = "N"; //variable to hold whether the game will be played in test mode
    const string AI_1 = "Angry Allan"; //The names of the three possible AI players
    const string AI_2 = "Speedy Steve";
    const string AI_3 = "Clever Trevor";

    static bool gameOver = false; //Variable that keeps track if somebody reaches the end of the game
    static bool powerOfSix = false; //Variable that holds whether the player has thrown a six or not

    static Random dice_1d6 = new Random();

    /// <summary>
    /// Code necesary to print underlined characters and strings in the console
    /// </summary>
    const int STD_OUTPUT_HANDLE = -11;
    const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
    private static string WriteUnderline(string s)
    {
        var handle = GetStdHandle(STD_OUTPUT_HANDLE);
        uint mode;
        GetConsoleMode(handle, out mode);
        mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        SetConsoleMode(handle, mode);
        return s = $"\x1B[4m{s}\x1B[24m";
    }

    /// <summary>
    /// 1. Array to hold the info on the players and their position
    /// 2. Array to hold dice values for test mode
    /// </summary>
    static Player[] player = new Player[4];
    static int[] diceValues = new int[] {2, 2, 3, 4, 3, 2, 2, 6, 5, 5, 6, 6, 5, 5};

    /// <summary>
    /// This is the actual game board printed out of UNICODE characters
    /// </summary>
    static string[,] charBoard = new string[18,26]
    {
        {" ", " ", "_", "_", " ", "_", "_", " ", "_", "_", " ", "_", "_", " ", "_", "_", " ", "_",
         "_", " ", "_", "_", " ", "_", "_", " ",},
        {"7", "|", "\x2193", " ", "|", "\x2192", " ", "|", "\x2192", " ", "|", "\x2192", " ", "|", "\x2192", " ", "|", //row 7
         "\x2192", " ", "|", "\x2193", " ", "|", "*", "*", "|"}, 
        {" ", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|",
         "_", "_", "|", "_", "_", "|", WriteUnderline("*"), WriteUnderline("*"), "|"},
        {"6", "|", "\x2192", " ", "|", "\x2192", " ", "|", "\x2191", " ", "|", "\x2193", " ", "|", "\x2191", " ", "|", //row 6
         "\x2190", " ", "|", "\x2190", " ", "|", "\x2190", " ", "|"}, 
        {" ", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|",
         "_", "_", "|", "_", "_", "|", "_", "_", "|"},
        {"5", "|", "\x2192", " ", "|", "\x2192", " ", "|", "\x2192", " ", "|", "\x2192", "*", "|", "\x2191", " ", "|", //row 5
         "\x2191", " ", "|", "\x2190", " ", "|", "\x2190", " ", "|"}, 
        {" ", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|",
         "_", "_", "|", "_", "_", "|", "_", "_", "|"},
        {"4", "|", "\x2192", " ", "|", "\x2192", " ", "|", "\x2191", " ", "|", "\x2192", " ", "|", "\x2191", " ", "|", //row 4
         "\x2191", " ", "|", "\x2190", "*", "|", "\x2190", " ", "|"}, 
        {" ", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|",
         "_", "_", "|", "_", "_", "|", "_", "_", "|"},
        {"3", "|", "\x2192", "*", "|", "\x2192", " ", "|", "\x2191", " ", "|", "\x2192", " ", "|", "\x2191", " ", "|", //row 3
         "\x2191", " ", "|", "\x2190", " ", "|", "\x2190", " ", "|"}, 
        {" ", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|",
         "_", "_", "|", "_", "_", "|", "_", "_", "|"},
        {"2", "|", "\x2192", " ", "|", "\x2192", " ", "|", "\x2191", " ", "|", "\x2190", " ", "|", "\x2190", " ", "|", //row 2
         "\x2190", " ", "|", "\x2190", " ", "|", "\x2190", " ", "|"}, 
        {" ", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|",
         "_", "_", "|", "_", "_", "|", "_", "_", "|"},
        {"1", "|", "\x2192", " ", "|", "\x2192", " ", "|", "\x2191", " ", "|", "\x2191", " ", "|", "\x2191", "*", "|", //row 1
         "\x2191", " ", "|", "\x2190", " ", "|", "\x2190", " ", "|"}, 
        {" ", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|",
         "_", "_", "|", "_", "_", "|", "_", "_", "|"},
        {"0", "|", "\x2191", " ", "|", "\x2191", " ", "|", "\x2191", " ", "|", "\x2191", " ", "|", "\x2191", " ", "|", //row 0
         "\x2191", " ", "|", "\x2191", " ", "|", "\x2191", " ", "|"}, 
        {" ", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|", "_", "_", "|",
         "_", "_", "|", "_", "_", "|", "_", "_", "|"},
        {" ", " ", "0", " ", " ", "1", " ", " ", "2", " ", " ", "3", " ", " ", "4", " ", " ", "5",
         " ", " ", "6", " ", " ", "7", " ", " "}
    };

    /// <summary>
    /// Structure to hold the relevant information of each player
    /// </summary>
    struct Player
    {
        public string Name;
        public string Token;
        public int X;
        public int Y;
    }
    /// <summary>
    /// Enumeration with the states according to each direction the board can hold.
    /// </summary>
    enum SquareDirection
    {
        Up,
        Right,
        Down,
        Left,
        Win
    }

    /// <summary>
    /// Method that creates the board to play. It holds the required values for the directions
    /// to know where the players should head when they throw the dice
    /// </summary>
    static SquareDirection[,] board = new SquareDirection[8, 8]
        {
            {SquareDirection.Up, SquareDirection.Up, SquareDirection.Up, SquareDirection.Up,             //row 0
             SquareDirection.Up, SquareDirection.Up, SquareDirection.Up, SquareDirection.Up},   
            {SquareDirection.Right, SquareDirection.Right, SquareDirection.Up, SquareDirection.Up,     //row 1
             SquareDirection.Up, SquareDirection.Up, SquareDirection.Left, SquareDirection.Left,},  
            {SquareDirection.Right, SquareDirection.Right, SquareDirection.Up, SquareDirection.Left,    //row 2    
             SquareDirection.Left, SquareDirection.Left, SquareDirection.Left, SquareDirection.Left,},  
            {SquareDirection.Right, SquareDirection.Right, SquareDirection.Up, SquareDirection.Right,    //row 3    
             SquareDirection.Up, SquareDirection.Up, SquareDirection.Left, SquareDirection.Left,},  
            {SquareDirection.Right, SquareDirection.Right, SquareDirection.Up, SquareDirection.Right,    //row 4    
             SquareDirection.Up, SquareDirection.Up, SquareDirection.Left, SquareDirection.Left,},  
            {SquareDirection.Right, SquareDirection.Right, SquareDirection.Right, SquareDirection.Right, //row 5    
             SquareDirection.Up, SquareDirection.Up, SquareDirection.Left, SquareDirection.Left,},  
            {SquareDirection.Right, SquareDirection.Right, SquareDirection.Up, SquareDirection.Down,     //row 6    
             SquareDirection.Up, SquareDirection.Left, SquareDirection.Left, SquareDirection.Left,},  
            {SquareDirection.Down, SquareDirection.Right, SquareDirection.Right, SquareDirection.Right,  //row 7    
             SquareDirection.Right, SquareDirection.Right, SquareDirection.Down, SquareDirection.Win,}   
        };

    /// <summary>
    /// Method that marks which squares are cheese (as true)
    /// </summary>
    static bool[,] cheeseBoard = new bool[8, 8]
        {
            {false, false, false, false, false, false, false, false}, //row 0
            {false, false, false, false, true, false, false, false}, //row 1
            {false, false, false, false, false, false, false, false}, //row 2
            {true, false, false, false, false, false, false, false}, //row 3
            {false, false, false, false, false, false, true, false}, //row 4
            {false, false, false, true, false, false, false, false}, //row 5
            {false, false, false, false, false, false, false, false}, //row 6
            {false, false, false, false, false, false, false, false}  //row 7
        };

    /// <summary>
    /// Method that prevents tthe programm from crashing any time that the user needs to input a number with
    /// max and min values
    /// </summary>
    /// <param name="prompt">question asked to the user</param>
    /// <param name="min">minimal value</param>
    /// <param name="max">maximal value</param>
    static void ReadInt(string prompt, int min, int max, out int playerInput)
    {        
        do
        {
            Console.WriteLine(prompt);
            try
            {
                playerInput = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                continue;
            }

            if ((playerInput < min) || (playerInput > max))
            {
                Console.WriteLine("You have entered an invalid number of players.\n" +
                    "Please enter a number between {0} and {1}.\n", min, max);
                continue;
            }
            else
            {
                break;
            }

        } while (true);
    }

    /// <summary>
    /// Method that asks how many players will play the game and clears the player position's array
    /// </summary>
    static void ResetGame()
    {
        Console.Clear();

        gameOver = false;  
        

        for(int i = 0; i < (numberOfPlayers+numberOfAIPlayers); i++) //resets the board to erase previous player positions
        {
            charBoard[(2 + 2 * ((8 - player[i].Y) - 1)),
                      (3 + 3 * ((player[i].X + 1) - 1))] = "_";
        }
        charBoard[2, 24] = "*";

        numberOfAIPlayers = 0;
        numberOfPlayers = 0;

        ReadInt("How many people will play? You can choose AI players later.", 0, 4, out numberOfPlayers);        

        for (int i = 0; i < numberOfPlayers; i++)
        {
            do
            {
                Console.WriteLine("What's the name of Player " + (i + 1) + "?");
                player[i].Name = Console.ReadLine().Trim();

                if (player[i].Name.Length == 0)
                {
                    Console.WriteLine("Names can't be left blank.");
                }
                else
                {
                    break;
                }
            } while (true);

            player[i].Token = (i + 1).ToString();
            player[i].X = 0;
            player[i].Y = 0;
        }


        if (numberOfPlayers < 4)
        {
            string wantAI;

            do
            {
                Console.WriteLine("Do you wish to add AI opponents? [Y/N]");
                wantAI = Console.ReadLine().Trim().ToUpper();
                if ((wantAI != "Y") && (wantAI != "N"))
                {
                    Console.WriteLine("Invalid answer. Write exclusively \"Y\" or \"N\".");
                    continue;
                }
                else
                {
                    break;
                }
            } while (true);

            if ((wantAI == "N") && (numberOfPlayers == 1))
            {
                Console.WriteLine("You can't play alone, you must choose some AI opponents.");
            }

            if ((wantAI == "Y") || (numberOfPlayers == 1))
            {
                Random nameAI = new Random();
                Console.WriteLine("There are " + (4 - numberOfPlayers) + " slots available.");

                ReadInt("How many AI opponents do you wish to add?", 1, (4 - numberOfPlayers), 
                    out numberOfAIPlayers);

                for(int i = numberOfPlayers; i < (numberOfAIPlayers + numberOfPlayers); i++)
                {
                    player[i].Name = nameAI.Next(1, 4).ToString();
                    
                    switch (player[i].Name)
                    {
                        case ("1"):
                            player[i].Name = AI_1;
                            break;
                        case ("2"):
                            player[i].Name = AI_2;
                            break;
                        case ("3"):
                            player[i].Name = AI_3;
                            break;
                    }

                    if(player[i].Name == player[i - 1].Name)
                    {
                        i = i - 1;
                        continue;
                    }
                    if(i > 2)
                    {
                        if (player[i].Name == player[i - 2].Name)
                        {
                            i = i - 1;
                            continue;
                        }
                    }

                    player[i].Token = (i + 1).ToString();
                    player[i].X = 0;
                    player[i].Y = 0;

                    Console.WriteLine("You will play against {0}. Press Enter to continue.", player[i].Name);
                    Console.ReadLine();
                }
          
            }            
        }
    }
        
    /// <summary>
    /// Method to establish a throw in test mode 
    /// </summary>
    /// <returns>a predetermined throw</returns>
    static int PresetDiceThrow()
    {
        int spots = diceValues[diceValuePos];

        diceValuePos += 1;

        if(diceValuePos == diceValues.Length)
        {
            diceValuePos = 0;
        }
        
        return spots;
    }

    /// <summary>
    /// Random number generator that will resemble a 1d6 dice.
    /// </summary>
    /// <returns>A number between 1 and 6</returns>
    static int DiceThrow()
    {
        return dice_1d6.Next(1, 7);
    }

    /// <summary>
    /// This method bumps the player to the appropriate square if another player is already in the square they landed. 
    /// </summary>
    /// <param name="X">column coordinate of the square where a player already is</param>
    /// <param name="Y">row coordinate of the square where a player already is</param>
    static void CheckFuturePosition(ref int X, ref int Y)
    {
        do
        {
            if (board[Y, X] == SquareDirection.Up)
            {
                Y += 1;
                continue;
            }
            if (board[Y, X] == SquareDirection.Right)
            {
                X += 1;
                continue;
            }
            if (board[Y, X] == SquareDirection.Down)
            {
                Y -= 1;
                continue;
            }
            if (board[Y, X] == SquareDirection.Left)
            {
                X -= 1;
                continue;
            }
        } while (RocketInSquare(X, Y) == true);
    }

    /// <summary>
    /// Method that lets the player choose which other player to shoot when they lando on a chesse square
    /// </summary>
    /// <param name="playerNo">the player of the current turn</param>
    static void PlayerToShoot(int playerNo)
    {
        do
        {
            Console.WriteLine("Enter the number of the player that you wish to shoot:");
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (i != playerNo)
                {
                    Console.WriteLine("\t- Player " + (i + 1) + ": " + player[i].Name);
                }
            }

            try
            {
                playerToShoot = int.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                continue;
            }

            if ((playerToShoot < 1) || (playerToShoot > 4))
            {
                Console.WriteLine("Please introduce a valid player number.");
                continue;
            }

            if ((playerToShoot - 1) == playerNo)
            {
                Console.WriteLine("You can't shoot yourself.");
                continue;
            }

            Console.WriteLine(player[(playerToShoot - 1)].Name + ", your engines have been shot and you drop to" +
                " the bottom row.");

            ChooseBottomRow(out futurePositionX, (playerToShoot - 1));

            charBoard[(2 + 2 * ((8 - player[(playerToShoot - 1)].Y) - 1)),
                      (3 + 3 * ((player[(playerToShoot - 1)].X + 1) - 1))] = "_";

            player[(playerToShoot - 1)].Y = 0;
            player[(playerToShoot - 1)].X = futurePositionX;

            charBoard[16, (3 + 3 * ((futurePositionX + 1) - 1))] =
                        WriteUnderline(player[(playerToShoot - 1)].Token);
            break;

        } while (true);

        ShowStatus();
        Console.Clear();
    }

    static void ChooseBottomRow(out int squarePositionX, int playerNo)
    {
        do
        {
            if ((player[playerNo].Name != AI_1) && (player[playerNo].Name != AI_2) &&
                (player[playerNo].Name != AI_3))
            {
                Console.WriteLine("In which square do you wish to place your rocket?");
                try
                {
                    squarePositionX = int.Parse(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                if ((squarePositionX < 0) || (squarePositionX > 7))
                {
                    Console.WriteLine("The number must be between 0 and 7");
                    continue;
                }
                else
                {
                    break;
                }
            }
            else
            {
                Random AISquare = new Random();
                squarePositionX = AISquare.Next(0, 8);
                break;
            }

        } while (true);
    }

    /// <summary>
    /// Method that decides the behaviour of the different AI players
    /// </summary>
    /// <param name="playerNo">The AI player of the current turn</param>
    static void AI_CheeseBehaviour(int playerNo)
    {
        int playerShotByAllan;

        Console.WriteLine("{0} has landed on a cheese square!", player[playerNo].Name);
        
        if(player[playerNo].Name == AI_1)
        {
            Console.WriteLine("{0} chooses to angrily shoot another player at random!", player[playerNo].Name);
            Random playerChosenByAllan = new Random();

            do
            {
                playerShotByAllan = playerChosenByAllan.Next(0, 3);
                if(playerShotByAllan == playerNo)
                {
                    continue;
                }
                else
                {
                    break;
                }
            } while (true);

            Console.ReadLine();
            Console.WriteLine("{0}, you have been shot by {1} and thus fall to the lowest row.",
                player[playerShotByAllan].Name, player[playerNo].Name);
               
            ChooseBottomRow(out futurePositionX, playerShotByAllan);

            charBoard[(2 + 2 * ((8 - player[playerShotByAllan].Y) - 1)),
                      (3 + 3 * ((player[playerShotByAllan].X + 1) - 1))] = "_";

            player[playerShotByAllan].Y = 0;
            player[playerShotByAllan].X = futurePositionX;

            charBoard[16, (3 + 3 * ((futurePositionX + 1) - 1))] =
                        WriteUnderline(player[playerShotByAllan].Token);

            return;
        }

        if(player[playerNo].Name == AI_2)
        {
            Console.WriteLine("{0} has no time for warfare, he chooses to throw the dice again!",
                player[playerNo].Name);
            PlayerTurn(playerNo);
            return;
        }

        if (player[playerNo].Name == AI_3)
        {
            for(int i = 7; i > player[playerNo].Y; i--)
            {
                for(int j = 7; j > 0; j--)
                {
                    for(int k = 0; k < (numberOfPlayers+numberOfAIPlayers); k++)
                    {
                        if ((player[playerNo].X == j) && (player[playerNo].Y == i))
                        {
                            continue;
                        }

                        if((player[k].X == j) && (player[k].Y == i))
                        {
                            Console.WriteLine("{0} sees {1} in the lead and decides to shoot his Cheese Death Ray!",
                                player[playerNo].Name, player[k].Name);

                            Console.ReadLine();
                            Console.WriteLine("{0}, you have been shot by {1} and thus fall to the lowest row.",
                                player[k].Name, player[playerNo].Name);

                            ChooseBottomRow(out futurePositionX, k);

                            charBoard[(2 + 2 * ((8 - player[k].Y) - 1)),
                                      (3 + 3 * ((player[k].X + 1) - 1))] = "_";

                            player[k].Y = 0;
                            player[k].X = futurePositionX;
                                                        
                            charBoard[16, (3 + 3 * ((futurePositionX + 1) - 1))] =
                                WriteUnderline(player[k].Token);

                            return;
                        }
                    }
                }
            }

            Console.WriteLine("{0} feels he's in the lead, he chooses to throw the dice again!",
                player[playerNo].Name);

            PlayerTurn(playerNo);

            return;
        }    
    }

    /// <summary>
    /// Method that will perform a dice throw and proceed to calculate the player movement
    /// </summary>
    /// <param name="playerNo">the appropriate player according to the turn</param>
    private static void PlayerTurn(int playerNo)
    {
        int diceResult = 0;        
        int powerOfSixCounter = 0;

        string cheeseChosen;

        bool playerWithinBoard = true;
        bool playerHasMoved = false;        

        do
        {
            playerHasMoved = false;
            playerWithinBoard = true;

            charBoard[(2 + 2 * ((8 - player[playerNo].Y) - 1)),
                      (3 + 3 * ((player[playerNo].X + 1) - 1))] = "_";

            switch (testMode)
            {
                case ("Y"):
                    diceResult = PresetDiceThrow();
                    break;
                case ("N"):
                    diceResult = DiceThrow();
                    break;
            }            

            Console.WriteLine("The dice shows a " + diceResult + ".\n");

            if(diceResult != 6)
            {
                powerOfSix = false;
            }
            
            if (diceResult == 6)
            {
                powerOfSix = true;
                powerOfSixCounter += 1;

                if (powerOfSixCounter == 3)
                {                    
                    Console.WriteLine(player[playerNo].Name + ", you have exhausted your engines. They explode, throwing" +
                        " you to the bottom row.");

                    ChooseBottomRow(out futurePositionX, playerNo);

                    charBoard[(2 + 2 * ((8 - player[playerNo].Y) - 1)),
                      (3 + 3 * ((player[playerNo].X + 1) - 1))] = "_";

                    player[playerNo].Y = 0;
                    player[playerNo].X = futurePositionX;

                    charBoard[16, (3 + 3*((futurePositionX+1) - 1))] = 
                        WriteUnderline(player[playerNo].Token);

                    return;
                }
            }

            if ((board[player[playerNo].Y, player[playerNo].X] == SquareDirection.Up) &&
                (playerHasMoved == false))
            {
                if ((player[playerNo].Y + diceResult) > 7)
                {
                    playerWithinBoard = false;
                }
                else
                {
                    if (RocketInSquare(player[playerNo].X,
                        (player[playerNo].Y + diceResult)) == true)
                    {
                        futurePositionX = player[playerNo].X;
                        futurePositionY = player[playerNo].Y + diceResult;

                        CheckFuturePosition(ref futurePositionX, ref futurePositionY);

                        player[playerNo].X = futurePositionX;
                        player[playerNo].Y = futurePositionY;
                        charBoard[(2 + 2 * ((8 - player[playerNo].Y) - 1)),
                            (3 + 3 * ((player[playerNo].X + 1) - 1))] =
                            WriteUnderline(player[playerNo].Token);
                        playerHasMoved = true;
                    }
                    else
                    {
                        player[playerNo].Y += diceResult;
                        charBoard[(2 + 2 * ((8 - player[playerNo].Y) - 1)),
                            (3 + 3 * ((player[playerNo].X + 1) - 1))] =
                            WriteUnderline(player[playerNo].Token);
                        playerHasMoved = true;
                    }
                }
            }

            if ((board[player[playerNo].Y, player[playerNo].X] == SquareDirection.Right) &&
                (playerHasMoved == false))
            {
                if ((player[playerNo].X + diceResult) > 7)
                {
                    playerWithinBoard = false;
                }
                else
                {
                    if (RocketInSquare((player[playerNo].X + diceResult),
                        player[playerNo].Y) == true)
                    {
                        futurePositionX = player[playerNo].X + diceResult;
                        futurePositionY = player[playerNo].Y;

                        CheckFuturePosition(ref futurePositionX, ref futurePositionY);

                        player[playerNo].X = futurePositionX;
                        player[playerNo].Y = futurePositionY;
                        charBoard[(2 + 2 * ((8 - player[playerNo].Y) - 1)),
                            (3 + 3 * ((player[playerNo].X + 1) - 1))] =
                            WriteUnderline(player[playerNo].Token);
                        playerHasMoved = true;
                    }
                    else
                    {
                        player[playerNo].X += diceResult;
                        charBoard[(2 + 2 * ((8 - player[playerNo].Y) - 1)),
                            (3 + 3 * ((player[playerNo].X + 1) - 1))] =
                            WriteUnderline(player[playerNo].Token);
                        playerHasMoved = true;
                    }
                }
            }

            if ((board[player[playerNo].Y, player[playerNo].X] == SquareDirection.Down) &&
                (playerHasMoved == false))
            {
                if ((player[playerNo].Y - diceResult) < 0)
                {
                    playerWithinBoard = false;
                }
                else
                {
                    if (RocketInSquare(player[playerNo].X,
                        (player[playerNo].Y - diceResult)) == true)
                    {
                        futurePositionX = player[playerNo].X;
                        futurePositionY = player[playerNo].Y - diceResult;

                        CheckFuturePosition(ref futurePositionX, ref futurePositionY);

                        player[playerNo].X = futurePositionX;
                        player[playerNo].Y = futurePositionY;
                        charBoard[(2 + 2 * ((8 - player[playerNo].Y) - 1)),
                            (3 + 3 * ((player[playerNo].X + 1) - 1))] =
                            WriteUnderline(player[playerNo].Token);
                        playerHasMoved = true;
                    }
                    else
                    {
                        player[playerNo].Y -= diceResult;
                        charBoard[(2 + 2 * ((8 - player[playerNo].Y) - 1)),
                            (3 + 3 * ((player[playerNo].X + 1) - 1))] =
                            WriteUnderline(player[playerNo].Token);
                        playerHasMoved = true;
                    }
                }
            }

            if ((board[player[playerNo].Y, player[playerNo].X] == SquareDirection.Left) &&
                (playerHasMoved == false))
            {
                if ((player[playerNo].X - diceResult) < 0)
                {
                    playerWithinBoard = false;
                }
                else
                {
                    if (RocketInSquare((player[playerNo].X - diceResult),
                        player[playerNo].Y) == true)
                    {
                        futurePositionX = player[playerNo].X - diceResult;
                        futurePositionY = player[playerNo].Y;

                        CheckFuturePosition(ref futurePositionX, ref futurePositionY);

                        player[playerNo].X = futurePositionX;
                        player[playerNo].Y = futurePositionY;
                        charBoard[(2 + 2 * ((8 - player[playerNo].Y) - 1)),
                            (3 + 3 * ((player[playerNo].X + 1) - 1))] =
                            WriteUnderline(player[playerNo].Token);
                        playerHasMoved = true;
                    }
                    else
                    {
                        player[playerNo].X -= diceResult;
                        charBoard[(2 + 2 * ((8 - player[playerNo].Y) - 1)),
                            (3 + 3 * ((player[playerNo].X + 1) - 1))] =
                            WriteUnderline(player[playerNo].Token);
                        playerHasMoved = true;
                    }
                }
            }

            if (playerWithinBoard == false)
            {
                Console.WriteLine("Unfortunately, your throw would move you out of bounds.");
                Console.WriteLine("You will stay where you are.");
                charBoard[(2 + 2 * ((8 - player[playerNo].Y) - 1)),
                            (3 + 3 * ((player[playerNo].X + 1) - 1))] =
                            WriteUnderline(player[playerNo].Token);
            }

            if ((player[playerNo].X == 7) && (player[playerNo].Y == 7))
            {
                gameOver = true;
                ShowStatus();
                Console.WriteLine();
                Console.WriteLine("{0} has reached the end. {0} has won the game!", player[playerNo].Name);
                Console.ReadLine();
            }

            ShowStatus();
            Console.Clear();

            if (CheesePowerSquare(player[playerNo].X, player[playerNo].Y) == true)
            {
                if ((player[playerNo].Name == AI_1) || (player[playerNo].Name == AI_2) ||
                    (player[playerNo].Name == AI_3))
                {
                    AI_CheeseBehaviour(playerNo);
                    return;
                }

                if (powerOfSix == false)
                {
                    Console.WriteLine("You have landed on a cheese square! You have the following options available:");
                    Console.WriteLine("\t1. You can throw the dice again");
                    Console.WriteLine("\t2. You can shoot your death ray to another player to send them back to the first row.\n");

                    do
                    {
                        Console.Write("Choose your option between 1 and 2: ");
                        cheeseChosen = Console.ReadLine().Trim();
                        if ((cheeseChosen != "1") && (cheeseChosen != "2"))
                        {
                            Console.WriteLine("Please enter exclusively \"1\" or \"2\"\n");
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    } while (true);

                    if (cheeseChosen == "1")
                    {
                        PlayerTurn(playerNo);
                        return;
                    }
                    else
                    {
                        PlayerToShoot(playerNo);                        
                    }
                }
                else
                {
                    Console.WriteLine("You have landed on a cheese square! The Power of Six grants you another throw.");

                    PlayerToShoot(playerNo);
                }              
            }

            if (powerOfSix == true)
            {
                Console.WriteLine(player[playerNo].Name + ", the Power of Six refuels your engines and grants" +
                    " you another throw.");
                Console.ReadLine();
                continue;
            }

            break;

        } while (true);                
    }

    /// <summary>
    /// Method to check if a player landed on a cheese square 
    /// </summary>
    /// <param name="X">x coordinate where the player landed</param>
    /// <param name="Y">y coordinate where the player landed</param>
    static bool CheesePowerSquare(int X, int Y)
    {
        if(cheeseBoard[Y, X] == true)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Method to assure that the square where the current plyer is going to move is empty or occupied
    /// by another player
    /// </summary>
    /// <param name="X">Coordinate X where the player will move</param>
    /// <param name="Y">Coordinate Y where the player will move</param>
    /// <returns></returns>
    static bool RocketInSquare(int X, int Y)
    {
        for(int i = 0; i < player.Length; i++)
        {
            if((player[i].X == X) && (player[i].Y == Y))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// This method shows the current status of the game after each turn
    /// </summary>
    static void ShowStatus()
    {
        Console.WriteLine(); //empty line
        Console.WriteLine("Hyperspace Cheese Battle Status Report\n" +
                          "======================================");
        Console.WriteLine();

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        for (int i = 0; i < charBoard.GetLength(0); i++)
        {
            for (int j = 0; j < charBoard.GetLength(1); j++)
            {
                Console.Write(charBoard[i, j]);
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine("There are {0} players in the game.", (numberOfPlayers+numberOfAIPlayers));
        for(int i = 0; i < (numberOfPlayers+numberOfAIPlayers); i++)
        {
            Console.WriteLine(player[i].Name + " is on square [" + player[i].X + ", " + player[i].Y + "]");
        }

        Console.WriteLine();
        Console.ReadLine();
    }

    /// <summary>
    /// This method moves the players in turns
    /// </summary>
    static void MakeMoves()
    {
        for (int i = 0; i < (numberOfPlayers+numberOfAIPlayers); i++)
        {
            Console.WriteLine(player[i].Name + " it's your turn! Press enter to throw the dice.");
            Console.ReadLine();

            PlayerTurn(i);

            if(gameOver == true)
            {
                break;
            }
        }
    }

    static void Main()
    {
        string doWeGoRoundAgain = "N"; //variable to hold if the players want to play again

        do
        {
            Console.WriteLine("Welcome to our game of Hyperspace Cheese Battle!\n" +
                "This is a game of intergalactic conflict and racing. And Cheese!\n\n" +
                "Rules are as follow:");
            Console.WriteLine("\t- Between 2 and 4 players can play.");
            Console.WriteLine("\t- Up to 3 AI players are possible, up to a maximum of 4 total players.");
            Console.WriteLine("\t- Each player controls a rocket which they can move through space.");
            Console.WriteLine("\t- Each player will throw a 1d6 dice in turns to decide their next square.");
            Console.WriteLine("\t- Power of Six: throwing a 6 grants you another throw. Throwing 3 sixes in a row\n" +
                              "\t   will exhaust your engines, causing you to automatically fall to the bottom row.");
            Console.WriteLine("\t- Certain squares are infused with \"Cheese Power\".\n" +
                              "\t   When landing on these squares, the player can choose to throw the dice again\n" +
                              "\t   or shoot their Death Ray to the engines of another player. The shot player\n" +
                              "\t   will be sent to the lowest row and they can choose their starting square.\n" +
                              "\t- Direction of movement is decided by the arrow of the square they are occupying.\n" +
                              "\t- No more than one player can occupy a given square at a time.\n" +
                              "\t- If you land in an occupied square you will automatically move onto the next\n" +
                              "\t   unnocupied square in the direction of the arrow that you landed on.\n" +
                              "\t- The first player to reach the [7,7] square wins.\n");
            Console.WriteLine("Good luck and have fun!\n");            

            do
            {
                Console.Write("Do you want to play in test mode? [Y/N]");
                testMode = Console.ReadLine().ToUpper();

                if ((testMode != "Y") && (testMode != "N"))
                {
                    Console.WriteLine("Please enter exclusively \"Y\" or \"N\".");
                    continue;
                }
                else
                {
                    break;
                }
            } while (true);

            ResetGame();

            Console.Clear();

            while (!gameOver)
            {
                MakeMoves();
                if (gameOver == true)
                {
                    break;
                }
                Console.Write("Press Enter to start the next round.");
                Console.ReadLine();
                Console.Clear();
            }

            Console.WriteLine("Game over! I hope you enjoyed it!\n");
            do
            {
                Console.WriteLine("Do you want to play again? [Y/N]");
                doWeGoRoundAgain = Console.ReadLine().Trim().ToUpper();
                if ((doWeGoRoundAgain != "Y") && (doWeGoRoundAgain != "N"))
                {
                    Console.WriteLine("Invalid answer. Write exclusively \"Y\" or \"N\".");
                    continue;
                }
                else
                {
                    break;
                }
            } while (true);            

        } while (doWeGoRoundAgain == "Y");      
                
        Console.WriteLine("\nPress Enter to return to Windows");
        Console.ReadLine();
    }
}