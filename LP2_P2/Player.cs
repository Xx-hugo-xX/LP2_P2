using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    public class Player
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int PosXOld { get; set; }
        public int PosYOld { get; set; }
        public char visuals { get; }

        public Player()
        {
            PosX = 0;
            PosY = 0;
            visuals = 'P';
        }
    }
}
