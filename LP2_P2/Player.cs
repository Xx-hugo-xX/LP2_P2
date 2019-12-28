using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    public class Player
    {
        public Position Pos { get; set; }

        public Position OldPos { get; set; }

        public char visuals { get; }

        public Player()
        {
            Pos.X = 0;
            Pos.Y = 0;
            visuals = 'P';
        }
    }
}
