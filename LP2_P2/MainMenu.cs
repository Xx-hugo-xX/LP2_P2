﻿using System;
using System.IO;
using System.Threading;

namespace LP2_P2
{
    public class MainMenu
    {
        private GameLoop Game { get; set; }
        private HighScoreManager HSManager { get; }

        public MainMenu()
        {
            HSManager = new HighScoreManager();
            HSManager.CreateFile();
        }

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

        public void ShowControls()
        {
            Console.Clear();
            Console.WriteLine("W - Go up\n" +
                "A - Go left\n" +
                "S - Go down\n" +
                "D - Go right\n" +
                "Press any key to return to the Main Menu.");
            Console.ReadKey(true);
        }

        private void Quit()
        {
            Console.Clear();
            Console.WriteLine("Thank you for playing! Come back any time!");
        }
    }
}
