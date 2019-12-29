using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    public class Physics
    {
        // Creates a list of all the objects in game
        private readonly List<Object> colliders;
        // Constructor of Physics class
        public Physics(List<Object> newColliders)
        {
            // Assigns the list of colliders the colliders on main loop
            colliders = newColliders;
        }

        /// <summary>
        /// Checks if the Object given is colliding with any other object and
        /// returns the type of the first object it collided with
        /// </summary>
        /// <param name="col"> The Object to be checked </param>
        /// <param name="x"> The X value it wants to check </param>
        /// <param name="y"> The Y value it wants to check</param>
        /// <returns> The type of the object it collided </returns>
        public Type Collision(Object col, int x = 0, int y = 0)
        {
            // Runs the loop acording to how many Objects there are
            for (int i = 0; i < colliders.Count; i++)
            {
                // Checks if the Object being checked is not itself
                if (colliders[i].GetType() != col.GetType())
                {
                    // Checks if the next X and Y values are inside any
                    // collider of all the Objects
                    if (col.boxCollider[0] + (x) >= colliders[i].boxCollider[0]
                        && col.boxCollider[2] + (x) <= colliders[i].boxCollider[2] &&
                        col.boxCollider[1] + (y) >= colliders[i].boxCollider[1]
                        && col.boxCollider[3] + (y) <= colliders[i].boxCollider[3])
                    {
                        // If it is inside retruns the type of that Object
                        return colliders[i].GetType();
                    }
                }
            }
            // If it isn't colliding with anything returns null
            return null;
        }        
        //-----------------Un-Check to check collisions with everything--------
        //public bool CheckCollisions()
        //{

        //    for (int b = 0; b < colliders.Count; b++)
        //    {
        //        for (int i = 1; i < colliders.Count -1; i++)
        //        {
        //            if (i != b &&
        //                colliders[b].boxCollider[0] ==
        //                colliders[i].boxCollider[0] &&
        //                colliders[b].boxCollider[1] ==
        //                colliders[i].boxCollider[1])
        //                return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
