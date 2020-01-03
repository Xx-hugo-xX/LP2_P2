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
            ObjType = objType;
            Pos = new Position(x, y);
            OldPos = new Position(x - 1, y - 1);
            Visuals = visual;
            BoxCollider = new int[4] { x, y, x + 1, y + 1 };
            ScoreVal = value;
        }
    }
}
