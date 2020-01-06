using System;
using System.IO;
using System.Threading;

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
            HSManager = new HighScoreManager();
            HSManager.CreateFile();
        }
        /// <summary>
        /// The Main Menu where the user can start the game, show the high
        /// scores, show the controls or quit the game
        /// </summary>
        public void RunMenu()
        {
            ConsoleKey key = ConsoleKey.D0;
            while(key != ConsoleKey.Q)
            {
                Console.Clear();
                Console.WriteLine("1. Play\n" +
                    "2. High Scores\n" +
                    "3. Controls\n" +
                    "Q. Quit");
                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                        Game = new GameLoop(HSManager);
                        Game.Loop();
                        break;
                    case ConsoleKey.D2:
                        ShowHighScores();
                        break;
                    case ConsoleKey.D3:
                        ShowControls();
                        break;
                    case ConsoleKey.Q:
                        Quit();
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
            using StreamReader sr = new StreamReader(HSManager.fileNameFull);

            string s;
            char separator = '\t';

            Console.Clear();
            Console.WriteLine($"+++++++++ HighScores +++++++++\n");

            while ((s = sr.ReadLine()) != null)
            {
                string[] nameAndScore = s.Split(separator);
                string name = nameAndScore[0];
                string score = nameAndScore[1];

                Console.WriteLine($"Player: {name}\tScore: {score, 4}");
            }

            Console.ReadKey(true);
        }
        /// <summary>
        /// Private method that displays the controls to the player
        /// </summary>
        private void ShowControls()
        {
            Console.Clear();
            Console.WriteLine("W - Go up\n" +
                "A - Go left\n" +
                "S - Go down\n" +
                "D - Go right\n" +
                "Press any key to return to the Main Menu.");
            Console.ReadKey(true);
        }
        /// <summary>
        /// Private method that quits the game
        /// </summary>
        private void Quit()
        {
            Console.Clear();
            Console.WriteLine("Thank you for playing! Come back any time!");
        }
    }
}
