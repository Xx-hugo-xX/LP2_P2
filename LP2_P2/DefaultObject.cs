namespace LP2_P2
{
    /// <summary>
    /// Creates a EmptySpace Object to be used as target for the ghost or to
    /// replace consumed pellets so the AI can find a path
    /// </summary>
    class DefaultObject : Object
    {
        /// <summary>
        /// Constructor of the EmptySpace class
        /// </summary>
        /// <param name="x"> The starting X position </param>
        /// <param name="y"> The starting Y position </param>
        public DefaultObject(int x, int y, char visual, ObjectType objType,
            int value = 0)
        {
            // Assign parameter objType to ObjType
            ObjType = objType;
            // Assign a new Position, using parameters x and y, to Pos
            Pos = new Position(x, y);
            // Assign a new Position, using parameters x and y, to OldPos
            OldPos = new Position(x - 1, y - 1);
            // Assign parameter visual to Visual
            Visuals = visual;
            // Assign new array of ints that acts and the object's collider,
            // using parameters x and y, to BoxCollider
            BoxCollider = new int[4] { x, y, x + 1, y + 1 };
            // Assign parameter value to ScoreVal
            ScoreVal = value;
        }
    }
}
