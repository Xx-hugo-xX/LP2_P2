using System;
using System.Collections.Generic;
using System.IO;

namespace LP2_P2
{
    class HighScoreManager
    {
        private readonly string appName;
        private readonly string filePath;
        private readonly string fileName;

        private int maxHS;


        public readonly string fileNameFull;

        public List<Score> highScores;

        public HighScoreManager()
        {
            appName = "PacMan";
            filePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                LocalApplicationData), appName);

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);


            fileName = "HighScores";
            fileNameFull = Path.Combine(filePath, fileName);

            maxHS = 10;

            highScores = new List<Score>();
        }

        public void CreateFile()
        {
            if (!File.Exists(fileNameFull))
                using (FileStream fs = File.Create(fileNameFull)) {}

            ReadHighScores();
        }

        private void ReadHighScores()
        {
            using StreamReader sr = new StreamReader(fileNameFull);

            string s;
            char separator = '\t';

            while ((s = sr.ReadLine()) != null)
            {
                string[] nameAndScore = s.Split(separator);
                string name = nameAndScore[0];

                int.TryParse(nameAndScore[1], out int score);

                highScores.Add(new Score(name, score));
            }
        }

        public void AddHighScore(Score score)
        {
            if(IsHighScore(score))
            {
                Console.Clear();
                Console.WriteLine("New HighScore! What should we call you?\n");

                if (score.Name == default) score.InsertName(Console.ReadLine());

                Console.WriteLine($"\nYour score of {score.TotalScore} was " +
                    $"added to the HighScores!");

                highScores.Add(score);
                highScores.Sort();
            }
        }

        private bool IsHighScore(Score score)
        {
            if (highScores.Count < maxHS) return true;

            if (score.TotalScore > highScores[maxHS-1].TotalScore)
            {
                highScores.RemoveAt(maxHS - 1);
                return true;
            }

            return false;
        }

        public void SaveHighScores()
        {
            using (StreamWriter sw = new StreamWriter(fileNameFull))
                foreach (Score hs in highScores)
                    sw.WriteLine(hs.Name + "\t" + hs.TotalScore);
        }
    }
}
