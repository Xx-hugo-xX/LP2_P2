using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    public class Ghost : Object
    {
        public Ghost()
        {
            Pos = new Position(10, 13);
            OldPos = new Position(1, 1);
            Visuals = 'G';
            boxCollider = new int[4] { 0, 0, 0 + 1, 0 + 1 };
        }

        public void Move(List<Object> physicsObjects, Physics col)
        {
            for(int i = 0; i < physicsObjects.Count; i++)
            {
                col.Collision(this, physicsObjects[i].Pos.X,
                    physicsObjects[i].Pos.Y);
            }
        }

        private void CheckPerifery(List<Object> physicsObjects, Physics col)
        {
            
        }
    }
}
