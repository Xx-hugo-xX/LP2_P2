using System;
using System.Collections.Generic;

namespace LP2_P2
{
    public class Score : IComparable<Score>
    {
        public string Name { get; private set; }

        public int TotalScore { get; private set; }

        public Score()
        {
            Name = default;
            TotalScore = 0;
        }

        public Score(string name, int score)
        {
            Name = name;
            TotalScore = score;
        }

        public void AddScore(int scoreToAdd)
        {
            TotalScore += scoreToAdd;
        }

        public void InsertName()
        {
            List<char> name = new List<char>();

            bool valid = false;
            while (!valid)
            {
                Console.Clear();
                Console.WriteLine("New High Score! " +
                        "What should we call you?\n");


                foreach (char c in name) Console.Write(c);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (name.Count > 0 && keyInfo.Key == ConsoleKey.Backspace)
                    name.RemoveAt(name.Count - 1);

                else if (name.Count < 3 && char.IsLetter(keyInfo.KeyChar))
                    name.Add(char.ToUpper(keyInfo.KeyChar));

                else if (name.Count == 3 && keyInfo.Key == ConsoleKey.Enter)
                    valid = true;
            }

            Name = new string(name.ToArray());
        }

        public int CompareTo(Score otherScore)
        {
            return - TotalScore.CompareTo(otherScore.TotalScore);
        }

        public override string ToString() => $"Score: {TotalScore}";
    }
}
