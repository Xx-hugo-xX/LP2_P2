using System;
using System.Collections.Concurrent;

namespace LP2_P2
{
    public class InputSystem
    {
        public Direction Dir { get => dir; }

        private BlockingCollection<ConsoleKey> inputCol;
        private Direction dir;
        private Player player;
        private DoubleBuffer2D<char> db;

        public InputSystem(Player player, DoubleBuffer2D<char> db)
        {
            inputCol = new BlockingCollection<ConsoleKey>();
            dir = new Direction();
            this.player = player;
            this.db = db;
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

        public void Update()
        {
            if(dir != Direction.None)
            {
                player.OldPos.X = player.Pos.X;
                player.OldPos.Y = player.Pos.Y;
                switch (dir)
                {
                    case Direction.Up:
                        player.Pos.Y = Math.Max(0, player.Pos.Y - 1);
                        break;
                    case Direction.Left:
                        player.Pos.X = Math.Max(0, player.Pos.X - 1);
                        break;
                    case Direction.Down:
                        player.Pos.Y = Math.Min(db.YDim - 1, player.Pos.Y + 1);
                        break;
                    case Direction.Right:
                        player.Pos.X = Math.Min(db.XDim - 1, player.Pos.X + 1);
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
