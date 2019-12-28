using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    class Player : Object
    {
        public Player()
        {
            Pos = new Position(1, 1);
            OldPos = new Position(1, 1);
            Visuals = 'P';
            BoxCollider = new int[4] { 0, 0, 0 + 1, 0 + 1 };
        }
    }
}
