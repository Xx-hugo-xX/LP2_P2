using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LP2_P2
{
    class InputSystem
    {
        public Direction Dir { get => dir; }

        private BlockingCollection<ConsoleKey> inputCol;
        private Direction dir;
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
            dir = new Direction();

        }

        public void ProcessInput()
        {
            ConsoleKey key;
            if (inputCol.TryTake(out key))
            {
                switch (key)
                {
                    case ConsoleKey.W:
                        dir = Direction.Up;
                        break;
                    case ConsoleKey.S:
                        dir = Direction.Down;
                        break;
                    case ConsoleKey.A:
                        dir = Direction.Left;
                        break;
                    case ConsoleKey.D:
                        dir = Direction.Right;
                        break;
                }
            }
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
