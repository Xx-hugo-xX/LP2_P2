using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    public class Player : Object
    {
        public Score plyrScore;

        public Player()
        {
            Pos = new Position(13, 18);
            OldPos = new Position(1, 1);
            Visuals = 'P';
            BoxCollider = new int[4] { 0, 0, 0 + 1, 0 + 1 };
            plyrScore = new Score();
        }
    }
}
