using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    class BonusFruit : Object
    {
        private readonly int scoreVal;

        public override int ScoreVal => scoreVal;

        public BonusFruit(int x, int y, int scoreVal)
        {
            Pos = new Position(x, y);
            this.scoreVal = scoreVal;
            Visuals = 'B';
            BoxCollider = new int[4] { x, y, x + 1, y + 1 };
        }
    }
}
