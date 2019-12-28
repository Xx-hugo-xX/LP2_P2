using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    public class Player
    {
        public Position Pos { get; set; }

        public Position OldPos { get; set; }

        public Score Score { get; }

        public char visuals { get; }

        public Player()
        {
            Pos = new Position(0, 0);
            OldPos = Pos;

            visuals = 'P';
            Score = new Score();
        }
    }
}
