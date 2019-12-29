using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    class Teleporter : Object
    {
        public Teleporter(int x, int y)
        {
            Pos = new Position(x, y);
            Visuals = '.';
            BoxCollider = new int[4] { x, y, x + 1, y + 1 };
        }
    }
}
