using System;

namespace LP2_P2
{
    /// <summary>
    /// DoubleBuffer2D of generic type.
    /// Responsible for containing the current and next frames in bidimensional
    /// arrays of generic type, being able to swap the next for the current and
    /// clearing the next frame to be written again.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleBuffer2D<T>
    {
        // Stores the current frame in a generic type bidimensional array
        private T[,] current;
        // Stores the next frame in a generic type bidimensional array
        private T[,] next;

        // Dimension of frame in X axis
        public int XDim => next.GetLength(0);
        // Dimension of frame in Y axis
        public int YDim => next.GetLength(1);

        // Indexer to be used in a DoubleBuffer2D instance
        public T this[int x, int y]
        {
            get => current[x, y];
            set => next[x, y] = value;
        }

        // Clears the next frame, to be written over again
        public void Clear()
        {
            Array.Clear(next, 0, XDim * YDim - 1);
        }

        // Class constructor
        public DoubleBuffer2D(int x, int y)
        {
            current = new T[x, y];
            next = new T[x, y];
            Clear();
        }

        // Swaps the current frame for the next frame
        public void Swap()
        {
            T[,] auxNext = next;
            current = next;
            next = auxNext;
        }
    }
}
