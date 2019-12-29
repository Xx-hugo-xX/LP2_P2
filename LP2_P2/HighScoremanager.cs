using System;
using System.Collections.Generic;
using System.IO;

namespace LP2_P2
{
    /// <summary>
    /// Manages the file that contains the High Scores, adding and removing
    /// Scores from the list, and saving them on the file when prompted
    /// </summary>
    class HighScoreManager
    {
        // Declares the readonly string that will store the directory name
        private readonly string appName;

        // Declares the readonly string that will store the path to the file
        private readonly string filePath;

        // Declares the readonly string that will store the name of the file
        private readonly string fileName;

        // Declares the readonly string that will
        // store the full path to the file
        public readonly string fileNameFull;

        // Sets the maximum number of High Scores to be saved in the file
        private int maxHS = 10;

        // Declares the list that will store the High Scores
        public List<Score> highScores;

        public HighScoreManager()
        {
            // Defines the name of the directory where
            // the High Scores will be saved
            appName = "PacMan";

            // Combines path and name of the directory
            filePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                LocalApplicationData), appName);

            // Checks if the user doesn't have the wanted directory
            if (!Directory.Exists(filePath))
                // Creates the directory for the user
                Directory.CreateDirectory(filePath);

            // Sets the name of the file where the High Scores will be saved
            fileName = "HighScores";

            // Combines the path and the name of the file in one single string
            fileNameFull = Path.Combine(filePath, fileName);

            // Initializes the list that will store the High Scores
            highScores = new List<Score>();
        }

        /// <summary>
        /// Creates the High Score file if it didn't already exist
        /// and calls method "ReadHighScores"
        /// </summary>
        public void CreateFile()
        {
            // Checks if the file does not exist 
            if (!File.Exists(fileNameFull))
                // Creates the file for the user, using "FileStream"
                using (FileStream fs = File.Create(fileNameFull)) {}

            // Calls method "ReadHighScores"
            ReadHighScores();
        }

        /// <summary>
        /// Reads the information on the file, separates the names from
        /// the scores, creates a new Score with that information and
        /// adds that score to the "highScores" list
        /// </summary>
        private void ReadHighScores()
        {
            // Uses StreamReader to read the High Scores file
            using StreamReader sr = new StreamReader(fileNameFull);

            // Creates new string that will store each file line
            string s;

            // Sets how the information on the file is separated
            char separator = '\t';

            // Runs loop until the StreaReader reads an empty line
            while ((s = sr.ReadLine()) != null)
            {
                // Splits the information using the previously set separator
                // and stores it in an array of strings
                string[] nameAndScore = s.Split(separator);

                // Creates a new "name" string that stores the value of the
                // first element in the "nameAndScore" array
                string name = nameAndScore[0];

                // Tries to parse the second element in the
                // "nameAndScore" array and returns the int "score"
                int.TryParse(nameAndScore[1], out int score);

                // Creates a new Score with the "name" and "score" variables
                // and adds that score to the "highScores" list
                highScores.Add(new Score(name, score));
            }
        }

        /// <summary>
        /// Checks if any given Score is to be added to the "highScores" List
        /// and adds said Score if it is
        /// </summary>
        /// <param name="score">Score that will be added to
        /// the "highScores" list</param>
        public void AddHighScore(Score score)
        {
            // Calls method "IsHighScore" to check if the given Score
            // is to be added to the "highScores" list
            if(IsHighScore(score))
            {
                // Clears the console
                Console.Clear();

                // Checks if the Score does not have a name associated
                if (score.Name == default)
                {
                    // Asks user to input their name
                    Console.WriteLine("New High Score!" +
                        "What should we call you?\n");

                    // Sets Score's name based on user input
                    score.InsertName(Console.ReadLine());
                }


                // Tells the user that their score
                // has been added to the High Scores
                Console.WriteLine($"\nYour score of {score.TotalScore} was " +
                    $"added to the High Scores!");

                // Adds the Score to the "highScores" list
                highScores.Add(score);

                // Sorts the "highScores" list
                highScores.Sort();
            }
        }

        /// <summary>
        /// Checks if the given score is to be added to the "highScores" list
        /// </summary>
        /// <param name="score">Score that will be analyzed</param>
        /// <returns></returns>
        private bool IsHighScore(Score score)
        {
            // Returns true if the "highScores" list has less
            // Scores than the maximum ammount of Scores allowed
            if (highScores.Count < maxHS) return true;

            // Checks if the given Scores "TotalScore" is higher
            // than the lowest "TotalScore"
            if (score.TotalScore > highScores[maxHS-1].TotalScore)
            {
                // Removes the last Score from the "highScores" list
                highScores.RemoveAt(maxHS - 1);

                // Returns true
                return true;
            }

            // Returns false
            return false;
        }

        /// <summary>
        /// Reads the "highScores" list and writes its values
        /// on the High Scores file, separated by a "tab"
        /// </summary>
        public void SaveHighScores()
        {
            // Uses StreamWriter to write on the High Scores file
            using StreamWriter sw = new StreamWriter(fileNameFull);

            // Loops through each element of the "highScores" list
            foreach (Score hs in highScores)

                // Writes the Names and TotalScores of each element
                // of the "highScores" list on the High Scores File
                sw.WriteLine(hs.Name + "\t" + hs.TotalScore);
        }
    }
}