using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace LP2_P2
{
    public class GameLoop
    {
        private Player player;
        private DoubleBuffer2D<char> db;
        private InputSystem inputSys;
        private bool running;
        private Thread keyReader;

        private readonly List<Object> physicsObjects = new List<Object>();
        private readonly Physics col;
        private readonly char[,] mapVisuals = new char[28, 23];
        private readonly string mapBuilder =
        "OOOOOOOOOOOOOOOOOOOOOOOOOOO" +
        "O............O............O" +
        "O-OOOO.OOOOO.O.OOOOO.OOOO-O" +
        "O.........................O" +
        "O.OOOO.OO.OOOOOOO.OO.OOOO.O" +
        "O......OO....O....OO......O" +
        "OOOOOO.OOOOO.O.OOOOO.OOOOOO" +
        "     O.OO         OO.O     " +
        "     O.OO         OO.O     " +
        "OOOOOO.OO         OO.OOOOOO" +
        "T     .             .     T" +
        "OOOOOO.OO         OO.OOOOOO" +
        "     O.OO         OO.O     " +
        "     O.OO         OO.O     " +
        "OOOOOO.OO OOOOOOO OO.OOOOOO" +
        "O............O............O" +
        "O.OOOO.OOOOO.O.OOOOO.OOOO.O" +
        "O-..OO....... .......OO..-O" +
        "OOO.OO.OO.OOOOOOO.OO.OO.OOO" +
        "O......OO....O....OO......O" +
        "O.OOOOOOOOOO.O.OOOOOOOOOO.O" +
        "O.........................O" +
        "OOOOOOOOOOOOOOOOOOOOOOOOOOO";
        public GameLoop()
        {
            player = new Player();

            physicsObjects.Add(player);
            ConvertMapToDoubleArray();
            GenerateMap();
            col = new Physics(physicsObjects);

            db = new DoubleBuffer2D<char>(30, 30);
            inputSys = new InputSystem(player, db, physicsObjects);
            keyReader = new Thread(inputSys.ReadKeys);
            Console.CursorVisible = false;
        }

        public void Loop()
        {
            keyReader.Start();
            running = true;
            while (running)
            {
                long start = DateTime.Now.Ticks;
                inputSys.ProcessInput();
                Update(mapVisuals);
                Render();
                Thread.Sleep(Math.Abs(
                    (int)(start / 20000)
                    + 20
                    - (int)(DateTime.Now.Ticks / 20000)));
            }
        }

        public void Update(char[,] mapVisuals)
        {
            if (inputSys.Dir != Direction.None)
            {
                player.OldPos = player.Pos;
                switch (inputSys.Dir)
                {
                    case Direction.Up:
                        // Checks if the next position up is not a wall
                        if (col.Collision(player, 0, -1) != typeof(MapPiece))
                        {
                            // Decreases the y of the player by 1
                            player.Pos.Y = Math.Max(0, player.Pos.Y - 1);
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;

                    case Direction.Left:
                        // Checks if the next position left is not a wall
                        if (col.Collision(player, -1, 0) != typeof(MapPiece))
                        {
                            // Decreases the x of the player by 1
                            player.Pos.X = Math.Max(0, player.Pos.X - 1);
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;

                    case Direction.Down:
                        // Checks if the next position down is not a wall
                        if (col.Collision(player, 0, 1) != typeof(MapPiece))
                        {
                            // Increases the y of the player by 1
                            player.Pos.Y =
                                Math.Min(db.YDim - 1, player.Pos.Y + 1);
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;

                    case Direction.Right:
                        // Checks if the next position right is not a wall
                        if (col.Collision(player, 1, 0) != typeof(MapPiece))
                        {
                            // Increases the X of the player by 1
                            player.Pos.X =
                                Math.Min(db.XDim - 1, player.Pos.X + 1);
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;
                }
            }

            // Updates the collider of the player to his current position
            player.UpdatePhysics();

            // Checks if the player is on a Pellet
            if (col.Collision(player) == typeof(SmallPellet) ||
                col.Collision(player) == typeof(BigPellet))
            {
                // Checks all the physicsObjects
                for (int i = 0; i < physicsObjects.Count; i++)
                {
                    // If the object has the same postition has the player
                    if (physicsObjects[i].Pos.X == player.Pos.X &&
                        physicsObjects[i].Pos.Y == player.Pos.Y)
                    {
                        // Add picked up item's score value to player's score
                        player.plyrScore.AddScore(physicsObjects[i].ScoreVal);
                        // Removes that object from the list
                        physicsObjects.RemoveAt(i);
                        // Updates visual for position player was in if there was a
                        // a pickable on it
                        mapVisuals[player.OldPos.X, player.OldPos.Y] = ' ';
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

        public void Render()
        {
            Console.WriteLine(player.plyrScore);
            // Loop for the amount of chars in the second position of the array
            for (int y = 0; y < 23; y++)
            {
                // Loop for the amount of chars in the first position of the array
                for (int x = 0; x < 28; x++)
                {
                    // Assigns the corresponding visual to the buffer
                    db[x, y] = mapVisuals[x, y];
                }
            }

            // Puts the player position on the buffer
            db[player.Pos.X, player.Pos.Y] = player.Visuals;

            db.Swap();

            // -------------UnComment the commented sections to generate colors
            // -------------Impacts performace ALOT!!!-------------------------
            for (int y = 0; y < db.YDim; y++)
            {
                for (int x = 0; x < db.XDim; x++)
                {
                    if (db[x, y] == 'O')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    if (db[x, y] == 'c' || db[x, y] == 'o')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    Console.Write(db[x, y]);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
            Console.SetCursorPosition(0, 0);
            db.Clear();
        }

        /// <summary>
        /// Assembles the map by creating the necessary things and adding them
        /// to a list
        /// </summary>
        private void GenerateMap()
        {
            // Loop for the amount of lines in the char array
            for (int y = 0; y < 23; y++)
            {
                // Loop for the amount of characters in the char array
                for (int x = 0; x < 28; x++)
                {
                    // If the current char is a O creates a wall
                    if (mapVisuals[x, y] == 'O')
                    {
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new MapPiece(x, y, x + 1, y + 1));
                    }
                    // If the current char is a . creates a Pellet
                    if (mapVisuals[x, y] == '.')
                    {
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new SmallPellet(x, y));
                    }
                    // If the current char is a . creates a Pellet
                    if (mapVisuals[x, y] == '-')
                    {
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new BigPellet(x, y));
                    }
                    // If the current char is a T creates a teleporter
                    if (mapVisuals[x, y] == 'T')
                    {
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new Teleporter(x, y));
                    }
                }
            }
        }

        /// <summary>
        /// Converts the string with the visuals to a double char array to be
        /// easier to acess and manipulate
        /// </summary>
        private void ConvertMapToDoubleArray()
        {
            // Kepps track of the current char of the string
            int charcount = 0;

            // Loop for the amount of lines in the char array
            for (int y = 0; y < 23; y++)
            {
                // Loop for the amount of characters in the char array
                for (int x = 0; x < 27; x++)
                {
                    // Assigns that position on the array the current char
                    // on the string
                    mapVisuals[x, y] = mapBuilder[charcount];
                    // Adds 1 to charcount to acess the next char on the string
                    charcount++;
                }
            }
        }
    }
}
