using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace LP2_P2
{
    class GameLoop
    {
        private Player player;
        private DoubleBuffer2D<char> db;
        private InputSystem inputSys;
        private bool running;

        private readonly List<Object> physicsObjects = new List<Object>();
        private readonly char[,] mapVisuals = new char[28, 23];
        private readonly string mapBuilder =
        "OOOOOOOOOOOOOOOOOOOOOOOOOOO" +
        "O............O............O" +
        "O.OOOO.OOOOO.O.OOOOO.OOOO.O" +
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
        "O...OO...............OO...O" +
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


            db = new DoubleBuffer2D<char>(30, 30);
            inputSys = new InputSystem(player, db, physicsObjects);
            Thread keyReader = new Thread(inputSys.ReadKeys);
            keyReader.Start();
            Console.CursorVisible = false;
        }

        public void Loop()
        {
            int msPerUpdate = 300000;
            long previous = Math.Abs(DateTime.Now.Ticks);
            long lag = 0L;

            running = true;
            while (running)
            {
                long current = Math.Abs(DateTime.Now.Ticks);
                long elapsed = current - previous;
                previous = current;
                lag += elapsed;

                inputSys.ProcessInput();
                while (lag >= msPerUpdate)
                {
                    inputSys.Update();
                    lag -= msPerUpdate;
                }
                Render();
            }
        }

        public void Render()
        {
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
                    //if (db[x, y] == 'O')
                    //{
                    //    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    //    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    //}
                    //if (db[x, y] == 'c' || db[x, y] == 'o')
                    //{
                    //    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    //}
                    Console.Write(db[x, y]);
                    //Console.ResetColor();
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
                        physicsObjects.Add(new TempPellet(x, y));
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
