using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    struct Score
    {
        public string Name
        {
            get => Name;
            set
            {
                Name = value.ToUpper();
            }
        }
        public int TotalScore
        {
            get => TotalScore;
            set
            {
                TotalScore += value;
            }
        }
    }
}
