using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    public class SmallPellet : Object
    {
        private const int scoreVal = 10;

        public override int ScoreVal => scoreVal;

        public SmallPellet(int x, int y)
        {
            Pos = new Position(x, y);
            Visuals = '.';
            boxCollider = new int[4] { x, y, x + 1, y + 1 };
        }
    }
}
