using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    class BigPellet : Object
    {
        private const int scoreVal = 50;

        public override int ScoreVal => scoreVal;

        public BigPellet(int x, int y)
        {
            Pos = new Position(x, y);
            Visuals = 'o';
            BoxCollider = new int[4] { x, y, x + 1, y + 1 };
        }
    }
}
