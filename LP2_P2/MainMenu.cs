﻿using System;
using System.IO;

namespace LP2_P2
{
    public class MainMenu
    {
        GameLoop game;
        HighScoreManager highScoreManager;

        public MainMenu()
        {
            game = new GameLoop();
            highScoreManager = new HighScoreManager();
            highScoreManager.CreateFile();
        }

        public void RunMenu()
        {
            ConsoleKey key;
            bool menuRunning = true;

            while (menuRunning)
            {
                Console.Clear();
                Console.WriteLine("1. Play" +
                "\n2. High Scores" +
                "\nQ. Quit");

                while((key = Console.ReadKey(true).Key) != ConsoleKey.Q)
                {
                    switch (key)
                    {
                        case ConsoleKey.D1:
                            game.Loop();
                            break;
                        case ConsoleKey.D2:
                            ShowHighScores();
                            break;
                        case ConsoleKey.Q:
                            Quit();
                            menuRunning = false;
                            break;
                    }
                }
            }

        }

        private void ShowHighScores()
        {
            using StreamReader sr = new StreamReader(highScoreManager.fileNameFull);

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

        private void Quit()
        {
            Console.Clear();
            Console.WriteLine("Thank you for playing! Come back any time!");
        }
    }
}