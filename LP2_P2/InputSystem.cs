using System;
using System.Collections.Concurrent;

namespace LP2_P2
{
    public class InputSystem
    {
        public Direction Dir { get; private set; }
        public Direction LastDir { get; set; }

        private readonly BlockingCollection<ConsoleKey> inputCol;
        private bool run = true;

        public InputSystem()
        {
            inputCol = new BlockingCollection<ConsoleKey>();
            Dir = new Direction();
            LastDir = new Direction();

        }

        public void SetDirection(Direction dir)
        {
            Dir = dir;
        }

        public void ProcessInput()
        {
            if (inputCol.TryTake(out ConsoleKey key))
            {
                switch (key)
                {
                    case ConsoleKey.W:
                        Dir = Direction.Up;
                        break;
                    case ConsoleKey.S:
                        Dir = Direction.Down;
                        break;
                    case ConsoleKey.A:
                        Dir = Direction.Left;
                        break;
                    case ConsoleKey.D:
                        Dir = Direction.Right;
                        break;
                }
            }
            else
            {
                Dir = LastDir;
            }
        }

        public void ReadKeys()
        {
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
                inputCol.Add(key);
            } while (run);
        }
        public void Stop()
        {
            run = false;
        }
    }
}
