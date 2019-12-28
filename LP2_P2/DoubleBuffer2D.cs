using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    public class DoubleBuffer2D<T>
    {
        private T[,] current;
        private T[,] next;

        public int XDim { get => next.GetLength(0); }
        public int YDim { get => next.GetLength(1); }

        public T this[int x, int y]
        {
            get => current[x, y];
            set => next[x, y] = value;
        }

        public void Clear()
        {
            Array.Clear(next, 0, XDim * YDim - 1);
        }

        public DoubleBuffer2D(int x, int y)
        {
            current = new T[x, y];
            next = new T[x, y];
            Clear();
        }

        public void Swap()
        {
            T[,] auxNext = next;
            current = next;
            next = auxNext;
        }
    }
}
