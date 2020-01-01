using System;

namespace LP2_P2
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Position A, Position B)
        {
            return A.X == B.X && A.Y == B.Y;
        }
        public static bool operator !=(Position A, Position B)
        {
            return A.X != B.X && A.Y != B.Y;
        }
    }
}
