using System;
using System.IO;

namespace LP2_P2
{
    public class MainMenu
    {
        //GameLoop game;
        HighScoreManager highScoreManager;

        public MainMenu()
        {
            //game = new GameLoop();
            highScoreManager = new HighScoreManager();
            highScoreManager.CreateFile();
        }

        public void RunMenu()
        {
            Console.WriteLine("1. Play" +
                            "\n2. High Scores" +
                            "\nQ. Quit");

            ProcessInput();
        }
        private void ProcessInput()
        {
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.D1:
                    //game.Loop();
                    //TestHighScores();
                    break;
                case ConsoleKey.D2:
                    ShowHighScores();
                    break;
                case ConsoleKey.Q:
                    Quit();
                    break;
            }
        }

        private void ShowHighScores()
        {
            using StreamReader sr = new StreamReader(highScoreManager.fileNameFull);

            string s;
            char separator = '\t';

            Console.WriteLine($"+++++++++ HighScores +++++++++\n");

            while ((s = sr.ReadLine()) != null)
            {
                string[] nameAndScore = s.Split(separator);
                string name = nameAndScore[0];
                string score = nameAndScore[1];

                Console.WriteLine($"Player: {name}\tScore: {score, 4}");
            }
        }

        private void Quit()
        {
            Console.WriteLine("Thank you for playing! Come back any time!");
            Environment.Exit(0);
        }


        /*
        private void TestHighScores()
        {
            Score s1  = new Score("HUG", 10);
            Score s2  = new Score("JUS", 25);
            Score s3  = new Score("ABC", 20);
            Score s4  = new Score("DEF", 56);
            Score s5  = new Score("GHI", 78);
            Score s6  = new Score("JKL", 19);
            Score s7  = new Score("MNO", 45);
            Score s8  = new Score("PQR", 33);
            Score s9  = new Score("STU", 67);
            Score s10 = new Score("VWX", 96);
            Score s11 = new Score("YZA", 38);
            Score s12 = new Score("BCD", 29);
            Score s13 = new Score("EFG", 12);
            Score s14 = new Score("HIJ", 74);
            Score s15 = new Score("HUG", 99);

            highScoreManager.AddHighScore(s1);
            highScoreManager.AddHighScore(s2);
            highScoreManager.AddHighScore(s3);
            highScoreManager.AddHighScore(s4);
            highScoreManager.AddHighScore(s5);
            highScoreManager.AddHighScore(s6);
            highScoreManager.AddHighScore(s7);
            highScoreManager.AddHighScore(s8);
            highScoreManager.AddHighScore(s9);
            highScoreManager.AddHighScore(s10);
            highScoreManager.AddHighScore(s11);
            highScoreManager.AddHighScore(s12);
            highScoreManager.AddHighScore(s13);
            highScoreManager.AddHighScore(s14);
            highScoreManager.AddHighScore(s15);

            highScoreManager.SaveHighScores();
        }*/
    }
}
