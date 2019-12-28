using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    /// <summary>
    /// Creates a new Object with the necessery thing to be used by Physics
    /// and the program in general.
    /// </summary>
    class Object
    {
        // Stores 4 cordinates for the colider
        public int[] BoxCollider;
        // Creates a Position for storing the current position
        public Position Pos { get; set; }
        // Creates a Position for storing the players previous position
        public Position OldPos { get; set; }
        // What character should the Object be displayed as
        public char Visuals { get; protected set; }

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
