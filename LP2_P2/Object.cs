using System;

namespace LP2_P2
{
    /// <summary>
    /// Creates a new Object with the necessery thing to be used by Physics
    /// and the program in general.
    /// </summary>
    public class Object : IPickable
    {
        // Stores 4 cordinates for the colider
        public int[] boxCollider;
        // Creates a Position for storing the current position
        public Position Pos { get; set; }
        // Creates a Position for storing a moving object's previous position
        public Position OldPos { get; set; }
        // What character should the Object be displayed as
        public char Visuals { get; protected set; }

        public virtual int ScoreVal { get; }

        // Updates the collider cordinates to match the current position
        public void UpdatePhysics()
        {
            boxCollider[0] = Pos.X;
            boxCollider[1] = Pos.Y;
            boxCollider[2] = Pos.X + 1;
            boxCollider[3] = Pos.Y + 1;
        }
    }
}
