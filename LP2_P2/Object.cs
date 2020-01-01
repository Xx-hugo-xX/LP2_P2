namespace LP2_P2
{
    /// <summary>
    /// Creates a new Object with the necessery thing to be used by Physics
    /// and the program in general.
    /// </summary>
    public class Object : IPickable
    {
        // Stores 4 cordinates for the colider
        public int[] BoxCollider;
        // Creates a Position for storing the current position
        public Position Pos { get; set; }
        // Creates a Position for storing a moving object's previous position
        public Position OldPos { get; set; }
        // What character should the Object be displayed as
        public char Visuals { get; protected set; }
        // Distance from the star position
        public int distanceCost = 0;
        // Distance from the current to the end position
        public int closenessCost = 0;
        // Combination of the two costs
        public int combinedCost => distanceCost + closenessCost;
        // The Object found before this one
        public Object parent;
        public virtual int ScoreVal { get; }

        // Updates the collider cordinates to match the current position
        public void UpdatePhysics()
        {
            BoxCollider[0] = Pos.X;
            BoxCollider[1] = Pos.Y;
            BoxCollider[2] = Pos.X + 1;
            BoxCollider[3] = Pos.Y + 1;
        }
    }
}
