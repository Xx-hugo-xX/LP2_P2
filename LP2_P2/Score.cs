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

        public int CompareTo(Score otherScore)
        {
            return - TotalScore.CompareTo(otherScore.TotalScore);
        }

        public override string ToString() => $"Score: {TotalScore}";
    }
}
