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

        public void Update()
        {
            if(dir != Direction.None)
            {
                player.OldPos.X = player.Pos.X;
                player.OldPos.Y = player.Pos.Y;
                switch (dir)
                {
                    case Direction.Up:
                        // Checks if the next position up is not a wall
                        if (col.Collision(player, 0, -1) != typeof(MapPiece))
                            // Decreases the y of the player by 1
                            player.Pos.Y = Math.Max(0, player.Pos.Y - 1);
                        break;

                    case Direction.Left:
                        // Checks if the next position left is not a wall
                        if (col.Collision(player, -1, 0) != typeof(MapPiece))
                            // Decreases the x of the player by 1
                            player.Pos.X = Math.Max(0, player.Pos.X - 1);
                        break;

                    case Direction.Down:
                        // Checks if the next position down is not a wall
                        if (col.Collision(player, 0, 1) != typeof(MapPiece))
                            // Increases the y of the player by 1
                            player.Pos.Y = Math.Min(db.YDim - 1, player.Pos.Y + 1);
                        break;

                    case Direction.Right:
                        // Checks if the next position right is not a wall
                        if (col.Collision(player, 1, 0) != typeof(MapPiece))
                            // Increases the X of the player by 1
                            player.Pos.X = Math.Min(db.XDim - 1, player.Pos.X + 1);
                        break;
                }
            }
            // Updates the collider of the player to his current position
            player.UpdatePhysics();

            // Checks if the player is on a Pellet
            if (col.Collision(player) == typeof(TempPellet))
            {
                // Checks all the physicsObjects
                for (int i = 0; i < physicsObjects.Count; i++)
                {
                    // If the object has the same postition has the player
                    if (physicsObjects[i].Pos == player.Pos)
                    {
                        // Removes that object from the list
                        physicsObjects.RemoveAt(i);
                    }
                }
            }
            // Checks if the player is on a Teleporter
            if (col.Collision(player) == typeof(Teleporter))
            {
                // If his postition is 0 teleports him to 26 else teleports him
                // to 1
                player.Pos.X = player.Pos.X == 0 ? 26 : 1;
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
