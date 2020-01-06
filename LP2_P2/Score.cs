using System;
using System.Collections.Generic;

namespace LP2_P2
{
    /// <summary>
    /// Responsible for handling score related logic
    /// </summary>
    public class Score : IComparable<Score>
    {
        // Declare public property Name of type string with a private set, 
        // responsible for containing the player's name
        public string Name { get; private set; }
        // Declare public property TotalScore of type int with a private set,
        // responsible for containing the value of the total score of a player
        public int TotalScore { get; private set; }
        
        /// <summary>
        /// Score class constructor with no parameters, responsible for 
        /// creating an instance of the class with a default string value Name,
        /// to be assigned to later and TotalScore with value 0, to be added to
        /// while the game runs.
        /// </summary>
        public Score()
        {
            // Assign default string value to Name property
            Name = default;
            // Assign int value 0 to TotalScore property
            TotalScore = 0;
        }
        /// <summary>
        /// Score class constructor overload, that takes params name and score
        /// in order to create an instance of the Score class when reading the
        /// Highscores file.
        /// </summary>
        /// <param name="name"> The name given </param>
        /// <param name="score"> The score given </param>
        public Score(string name, int score)
        {
            // Assign the value of the string name, given as a parameter to the
            // Name property
            Name = name;
            // Assign the value of the int score, given as a parameter to the
            // TotalScore property
            TotalScore = score;
        }
        /// <summary>
        /// Public method that adds the score given to the Score class 
        /// instance's TotalScore property, making use of it's private set,
        /// effectively acting as a middleman.
        /// </summary>
        /// <param name="scoreToAdd"></param>
        public void AddScore(int scoreToAdd)
        {
            // Add score given as a parameter to the TotalScore property
            TotalScore += scoreToAdd;
        }
        
        // ---------------------------DO XML LATER-----------------------------
        public void InsertName()
        {
            int maxLength = 3;
            List<char> name = new List<char>();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("New High Score! " +
                        "What should we call you?\n");


                for (int i = 0; i < name.Count; i++)
                    Console.Write(name[i]);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (name.Count > 0 && keyInfo.Key == ConsoleKey.Backspace)
                    name.RemoveAt(name.Count - 1);

                else if (name.Count < maxLength &&
                    char.IsLetter(keyInfo.KeyChar) && keyInfo.KeyChar != 'º')
                    name.Add(char.ToUpper(keyInfo.KeyChar));

                else if (name.Count == maxLength &&
                    keyInfo.Key == ConsoleKey.Enter)
                    break;
            }

            Name = new string(name.ToArray());
        }
        /// <summary>
        /// Public method that compares the instance by which it is called with
        /// another instance given through it's parameters, used as the default
        /// CompareTo that is used when the Sort() method is called by a list 
        /// that hold elements of type Score.
        /// </summary>
        /// <param name="otherScore"> The score to be compared to </param>
        /// <returns></returns>
        public int CompareTo(Score otherScore)
        {
            // Returns the negative of the int returned from the CompareTo of
            // the TotalScore property of the instance it is called from with
            // the TotalScore from a different instance of Score
            return - TotalScore.CompareTo(otherScore.TotalScore);
        }
        /// <summary>
        /// Override of ToString method, which returns a simple string that
        /// displays the TotalScore of the Score instance it is called from,
        /// using lambdas
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Score: {TotalScore}";
    }
}
