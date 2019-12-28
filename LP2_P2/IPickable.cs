using System;
using System.Linq;

namespace LP2_P2
{
    public interface IPickable
    {
        int Value { get; }
        bool PickedUp { get; set; }
    }
}
