using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LP2_P2
{
    public class InputSystem
    {
        public Direction Dir { get; private set; }
        public Direction LastDir { get; set; }

        private BlockingCollection<ConsoleKey> inputCol;
        private Player player;
        private DoubleBuffer2D<char> db;
        private List<Object> physicsObjects;
        private readonly Physics col;

        public InputSystem(Player player, DoubleBuffer2D<char> db,
            List<Object> objects)
        {
            this.player = player;
            this.db = db;

            physicsObjects = objects;
            col = new Physics(physicsObjects);

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
            ConsoleKey key;
            if (inputCol.TryTake(out key))
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
            do
            {
                key = Console.ReadKey(true).Key;
                inputCol.Add(key);
            } while (key != ConsoleKey.Escape);
        }
    }
}
