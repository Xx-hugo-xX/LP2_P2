using System;
using System.IO;

namespace LP2_P2
{
    /// <summary>
    /// Class responsible for creating and handling the MainMenu and it's logic
    /// </summary>
    public class MainMenu
    {
        // Declare a private property of type GameLoop, responsible for calling
        // the game loop
        private GameLoop Game { get; set; }
        // Declare a private property of type HighScoreManager, responsible for
        // handling score related logic
        private HighScoreManager HSManager { get; }
        /// <summary>
        /// Class constructor with no parameters, initializing a new 
        /// HighScoreManager class instance and assigning it to the
        /// HSManager property
        /// </summary>
        public MainMenu()
        {
            // Instantiates the instance of 'HighScoreManager'
            // that will be used throughout the game 
            HSManager = new HighScoreManager();

            // Calls the 'HighScoreManager' method 'CreateFile', that checks if
            // the user already has the High Scores file, and creates
            // a new file, if the user doesn't already have it 
            HSManager.CreateFile();
        }
        /// <summary>
        /// The Main Menu where the user can start the game, show the high
        /// scores, show the controls or quit the game
        /// </summary>
        public void RunMenu()
        {
            // Declares new ConsoleKey variable and instantiates it as default 
            ConsoleKey key = default;

            // Loops while the variable 'key' is different to 'ConsoleKey.Q'
            while(key != ConsoleKey.Q)
            {
                // Clears the console
                Console.Clear();

                // Writes each possible input option for the user
                Console.WriteLine("1. Play\n" +
                    "2. High Scores\n" +
                    "3. Controls\n" +
                    "Q. Quit");

                // Sets the variable 'key' based on user input
                key = Console.ReadKey(true).Key;

                // Switch statement that analyzes
                // the value of the variable 'key'
                switch (key)
                {
                    // Case it's the numberKey '1' (not on the NumPad)
                    case ConsoleKey.D1:
                        // Sets the variable 'Game' as a new instance
                        // of 'GameLoop', that uses the 'HSManager'
                        // instance of the 'MainMenu' class
                        Game = new GameLoop(HSManager);

                        // Calls the method 'Loop' of the 'GameLoop' class,
                        // That runs the game loop
                        Game.Loop();
                        // Leaves the current case;
                        break;
                    // Case it's the numberKey '2' (not on the NumPad)
                    case ConsoleKey.D2:
                        // Calls the method 'ShowHighScores' of the 'MainMenu'
                        // class, that read the HighScores
                        // file and displays its contents
                        ShowHighScores();
                        // Leaves the current case;
                        break;
                    // Case it's the numberKey '3' (not on the NumPad)
                    case ConsoleKey.D3:
                        // Calls the method 'ShowControls' of the 'MainMenu'
                        // class, that shows the available controls
                        ShowControls();
                        // Leaves the current case;
                        break;
                    // Case it's the keyBoard key 'Q'
                    case ConsoleKey.Q:
                        // Calls the method 'Quit' of the 'MainMenu'
                        // class, that displays a "goodbye" message
                        Quit();
                        // Leaves the current case;
                        break;
                }
            }
        }
        /// <summary>
        /// Private method that parses the information in the HighScores file
        /// and displays them to the player
        /// </summary>
        private void ShowHighScores()
        {
            // Opens a StreamReader with the HighScores file
            // (and closes when the method ends)
            using StreamReader sr = new StreamReader(HSManager.fileNameFull);

            // Sets string that will contain each line of the file
            string s;
            // Sets what separates the values
            // (name and score value) of each line
            char separator = '\t';

            // Clears the console
            Console.Clear();
            // Writes the first line of the HighScores menu
            Console.WriteLine($"+++++++++ HighScores +++++++++\n");

            // Loops until the StreamReader finds an
            // empty line in the HighScores file
            while ((s = sr.ReadLine()) != null)
            {
                // Sets a new array of strings that saves
                // the contents separated of each line
                string[] nameAndScore = s.Split(separator);
                // Sets a new string with the first
                // element of the 'nameAndScore' array (name)
                string name = nameAndScore[0];
                // Sets a new string with the second
                // element of the 'nameAndScore' array (score)
                string score = nameAndScore[1];

                Console.WriteLine($"Player: {name}\tScore: {score,4}");
            }
            // Asks for user input before exiting the method
            Console.ReadKey(true);
        }
        /// <summary>
        /// Private method that displays the controls to the player
        /// </summary>
        private void ShowControls()
        {
            // Clears the console
            Console.Clear();

            // Writes every possible input option during the game and explain
            // how the game progresses to the player
            Console.WriteLine("W - Go up\n" +
                "A - Go left\n" +
                "S - Go down\n" +
                "D - Go right\n\n" +
                "Collect all the pellets ( . ) to advance to the next " +
                "level!\nCollect a big pellet ( º ) to become able to eat " +
                "ghosts\nCollect bonus fruits ( F ) to earn a higher amount " +
                "of score, increased with every level passed\n" +
                "Try to get the highest score you can and appear in the " +
                "highscore list!\n\n" +
                "Press any key to return to the Main Menu.");
            // Asks for user input before exiting the method
            Console.ReadKey(true);
        }
        /// <summary>
        /// Private method that quits the game
        /// </summary>
        private void Quit()
        {
            // Clears the console
            Console.Clear();
            // Writes a "Goodbye" message before exiting the game
            Console.WriteLine("Thank you for playing! Come back any time!");
        }
    }
}
