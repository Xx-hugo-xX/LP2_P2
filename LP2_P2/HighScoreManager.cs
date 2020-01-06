using System;
using System.Collections.Generic;
using System.IO;

namespace LP2_P2
{
    /// <summary>
    /// Manages the file that contains the High Scores, adding and removing
    /// Scores from the list, and saving them on the file when prompted
    /// </summary>
    public class HighScoreManager
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
        private List<Score> highScores;

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
            fileName = "HighScores.txt";

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

                    // Sets Score's Name based on user input
                    score.InsertName(SetPlayerName());

                // Clears the console
                Console.Clear();

                // Tells the user that their score
                // has been added to the High Scores
                Console.WriteLine($"Your score of {score.TotalScore} was " +
                    $"added to the High Scores!");

                // Only let's user advance after he presses any key
                Console.ReadKey(true);

                // Adds the Score to the "highScores" list
                highScores.Add(score);

                // Sorts the "highScores" list
                highScores.Sort();

                // Runs method that saves all High Scores
                // in the High Scores file
                SaveHighScores();
            }
        }

        /// <summary>
        /// Checks if the given score is to be added to the "highScores" list
        /// </summary>
        /// <param name="score">Score that will be analyzed</param>
        /// <returns>Returns a boolean that defines if the analyzed score
        /// is a HighScore or not</returns>
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
        /// on the High Scores file
        /// </summary>
        private void SaveHighScores()
        {
            // Uses StreamWriter to write on the High Scores file
            using StreamWriter sw = new StreamWriter(fileNameFull);

            // Loops through each element of the "highScores" list
            foreach (Score hs in highScores)

                // Writes the Names and TotalScores (separated by a 'tab')
                // of each element of the "highScores" list
                // on the High Scores file
                sw.WriteLine(hs.Name + "\t" + hs.TotalScore);
        }

        /// <summary>
        /// Asks the player for a name to save for the HighScores file
        /// </summary>
        /// <returns>Returns a string with the name the
        /// player chose fo the HighScores file</returns>
        private string SetPlayerName()
        {
            // Sets the maximum number of characters the name can have
            int maxLength = 3;

            // Declares and instantiates a list of chars that
            // will save each valid char the user inputs
            List<char> name = new List<char>();

            // Loops until the conditions have been met
            while (true)
            {
                // Clears the console
                Console.Clear();
                // Writes a message asking for the player to input a name
                Console.WriteLine("New High Score! " +
                        "What should we call you?\n");

                // Loops until 'i' is equal to the number
                // of chars in the variable 'name'
                for (int i = 0; i < name.Count; i++)
                    // Writes each value already in the variable 'name'
                    Console.Write(name[i]);

                // Sets new ConsoleKeyInfo variable based on user input 
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Checks if the 'name' list of chars is not empty
                // and the variable 'keyInfo'.Key is the Backspace key
                if (name.Count > 0 && keyInfo.Key == ConsoleKey.Backspace)
                    // Removes the last char in the 'name' list of chars
                    name.RemoveAt(name.Count - 1);

                // Checks if the number of chars in the 'name' list is less
                // than the 'maxLength' variable, if the variable
                // 'keyInfo'.KeyChar is a letter, and if the variable
                // 'keyInfo'.KeyChar is diiferent than 'º' 
                else if (name.Count < maxLength &&
                    char.IsLetter(keyInfo.KeyChar) && keyInfo.KeyChar != 'º')
                    // Sets the variable 'keyInfo'.KeyChar to UpperCase
                    // and adds it to the 'name' list of chars
                    name.Add(char.ToUpper(keyInfo.KeyChar));

                // Checks if the number of chars in the 'name'
                // list is equal to the 'maxLength' variable, and
                // if the the variable 'keyInfo'.Key is Enter key
                else if (name.Count == maxLength &&
                    keyInfo.Key == ConsoleKey.Enter)
                    // Leaves the current loop
                    break;
            }
            // Returns a new string formed by the
            // 'name' list transformed in an array
            return new string(name.ToArray());
        }
    }
}