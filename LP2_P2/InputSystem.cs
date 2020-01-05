using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LP2_P2
{
    public class InputSystem
    {
        public bool IsRunning { get; private set; }
        public Direction Dir { get; private set; }
        public Direction LastDir { get; set; }

        private readonly BlockingCollection<ConsoleKey> inputCol;

        public InputSystem()
        { 
            inputCol = new BlockingCollection<ConsoleKey>();
            Dir = new Direction();
            LastDir = new Direction();
            IsRunning = true;
        }

        public void CloseInputReading()
        {
            IsRunning = false;
        }

        public void ResetInput()
        {
            Dir = Direction.None;
            LastDir = Direction.None;
            for (int i = 0; i < inputCol.Count; i++) inputCol.Take();
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
            else Dir = LastDir;
        }

        public void ReadKeys()
        {
            ConsoleKey key;

            IsRunning = true;
            while(IsRunning)
            {
                key = Console.ReadKey(true).Key;
                inputCol.Add(key);
            }
        }
    }
}
