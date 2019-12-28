using System;
using System.Linq;

namespace LP2_P2
{
    public interface IPickable
    {
        public int ScoreVal { get; }
        public bool isPicked { get; set; }

        public void PickUp();
    }
}
