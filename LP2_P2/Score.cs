using System;

namespace LP2_P2
{
    public class Score : IComparable<Score>
    {
        public string Name { get; private set; }

        public int TotalScore { get; private set; }

        public Score()
        {
            Name = "";
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

        public void InsertName(string nameInserted)
        {
            Name = nameInserted;
        }

        public int CompareTo(Score otherScore)
        {
            return - TotalScore.CompareTo(otherScore.TotalScore);
        }

        public override string ToString() => $"Score: {TotalScore}";
    }
}
