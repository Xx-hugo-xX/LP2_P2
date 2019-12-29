using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    public class Ghost : Object
    {
        public Ghost()
        {
            Pos = new Position(10, 13);
            OldPos = new Position(1, 1);
            Visuals = 'G';
            BoxCollider = new int[4] { 0, 0, 0 + 1, 0 + 1 };
        }
    }
}
