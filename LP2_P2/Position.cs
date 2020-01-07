namespace LP2_P2
{
    /// <summary>
    /// Defines a position for objects in the game
    /// </summary>
    public class Position
    {
        // Column where the object is located on the map
        public int X { get; set; }
        // Line where the object is located on the map
        public int Y { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="x">Value given to the column of the object</param>
        /// <param name="y">Value given to the line of the object</param>
        public Position(int x, int y)
        {
            // Sets 'X' as the given 'x' value
            X = x;
            // Sets 'Y' as the given 'y' value
            Y = y;
        }

        /// <summary>
        /// Sets how the '==' operator will be used to compare two Positions
        /// </summary>
        /// <param name="A">First Position to be compared</param>
        /// <param name="B">Second Position to be compared</param>
        /// <returns>Returns if the positions are equal or not</returns>
        public static bool operator ==(Position A, Position B)
        {
            // Checks if the 'X' value AND the 'Y'
            // value of both Positions are the same
            return A.X == B.X && A.Y == B.Y;
        }

        /// <summary>
        /// Sets how the '!=' operator will be used to compare two Positions
        /// </summary>
        /// <param name="A">First Position to be compared</param>
        /// <param name="B">Second Position to be compared</param>
        /// <returns>>Returns boolean that tells us
        /// if the positions are different or not</returns>
        public static bool operator !=(Position A, Position B)
        {
            // Checks if the 'X' value OR the 'Y'
            // value of both Positions are different
            return A.X != B.X || A.Y != B.Y;
        }

        /// <summary>
        /// Mandatory override method of 'Equals'
        /// </summary>
        /// <param name="obj">Object to be compared with</param>
        /// <returns>Returns if the current object and
        /// the specified object have the same reference or not</returns>
        public override bool Equals(object obj)
        {
            // Compares the references of the two objects
            // and returns true if they are equal
            if (ReferenceEquals(this, obj)) return true;
            // returns false if the references are different
            else return false;
        }

        /// <summary>
        /// Mandatory override method of 'GetHashCode'
        /// </summary>
        /// <returns>Returns the base GetHashCode</returns>
        public override int GetHashCode()
        {
            // Returns the base GetHashCode
            return base.GetHashCode();
        }
    }
}
