using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    public class MapPiece : Object
    {
        public MapPiece(int x, int y, int l, int w)
        {
            Pos = new Position(x, y);
            Visuals = '.';
            BoxCollider = new int[4] { x, y, l, w };
        }
    }
}
