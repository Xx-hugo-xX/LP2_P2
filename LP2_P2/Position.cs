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

        public static bool operator ==(Position A, Position B) => 
            A.X == B.X && A.Y == B.Y;
        public static bool operator !=(Position A, Position B) =>
            A.X != B.X && A.Y != B.Y;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
