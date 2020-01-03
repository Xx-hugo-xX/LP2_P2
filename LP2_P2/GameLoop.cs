using System;
using System.Collections.Generic;
using System.Threading;

namespace LP2_P2
{
    public class GameLoop
    {
        private readonly Player player;
        private readonly DoubleBuffer2D<char> db;
        private readonly InputSystem inputSys;
        private bool running;
        private readonly Thread keyReader;

        private readonly Ghost redGhost;
        private readonly Ghost pinkGhost;

        private readonly EmptySpace pinkTarget = new EmptySpace(0, 0);

        private bool updateTimer = false;
        private int timer = 0;
        private int stateSwapTimer = 0;

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
        "     O.OO OOOOOOO OO.O     " +
        "OOOOOO.OO O     O OO.OOOOOO" +
        "T     .   O     O   .     T" +
        "OOOOOO.OO O     O OO.OOOOOO" +
        "     O.OO OOOOOOO OO.O     " +
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

            redGhost = new Ghost(2, 1, physicsObjects, 1, 1);
            pinkGhost = new Ghost(24, 1, physicsObjects, 25, 1);
            physicsObjects.Add(redGhost);
            physicsObjects.Add(pinkGhost);

            col = new Physics(physicsObjects);

            db = new DoubleBuffer2D<char>(30, 30);
            inputSys = new InputSystem();
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
                Object wallDetection;
                player.OldPos.X = player.Pos.X;
                player.OldPos.Y = player.Pos.Y;

                switch (inputSys.Dir)
                {
                    case Direction.Up:
                        wallDetection = col.Collision(player, 0, -1);
                        // Checks if the next position up is not a wall
                        if (wallDetection == null || wallDetection.GetType() 
                            != typeof(MapPiece))
                        {
                            // Decreases the y of the player by 1
                            player.Pos.Y = Math.Max(0, player.Pos.Y - 1);
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;

                    case Direction.Left:
                        wallDetection = col.Collision(player, -1, 0);
                        // Checks if the next position left is not a wall
                        if (wallDetection == null || wallDetection.GetType() 
                            != typeof(MapPiece))
                        {
                            // Decreases the x of the player by 1
                            player.Pos.X = Math.Max(0, player.Pos.X - 1);
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;

                    case Direction.Down:
                        wallDetection = col.Collision(player, 0, 1);
                        // Checks if the next position down is not a wall
                        if (wallDetection == null || wallDetection.GetType()
                            != typeof(MapPiece))
                        {
                            // Increases the y of the player by 1
                            player.Pos.Y =
                                Math.Min(db.YDim - 1, player.Pos.Y + 1);
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;

                    case Direction.Right:
                        wallDetection = col.Collision(player, 1, 0);
                        // Checks if the next position right is not a wall
                        if (wallDetection == null || wallDetection.GetType()
                            != typeof(MapPiece))
                        {
                            // Increases the X of the player by 1
                            player.Pos.X =
                                Math.Min(db.XDim - 1, player.Pos.X + 1);
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;
                }

                stateSwapTimer++;
                timer++;

                UpdateGhost(redGhost);
                UpdateGhost(pinkGhost);

                if (updateTimer)
                {
                    stateSwapTimer = 0;
                    updateTimer = false;
                }

                if (timer > 1)
                {
                    if (!(player.OldPos == player.Pos) 
                        || pinkGhost.Pos == pinkTarget.Pos)
                        pinkTarget.Pos = new Position(
                            ((player.Pos.X - player.OldPos.X) * 3) 
                            + player.Pos.X, 
                            ((player.Pos.Y - player.OldPos.Y) * 3)
                            + player.Pos.Y);

                    pinkGhost.CalcuatePath(pinkTarget);
                    redGhost.CalcuatePath(player);

                    redGhost.UpdatePosition();
                    pinkGhost.UpdatePosition();
                }
                Console.WriteLine(pinkGhost.state);
                Console.WriteLine(redGhost.state);
            }

            // Updates the collider of the player to his current position
            player.UpdatePhysics();

            Object obj = col.Collision(player);

            if (obj != null)
            {
                if (obj.GetType() == typeof(Ghost))
                {
                    Ghost ghost = obj as Ghost;
                    if (ghost.state == GhostState.frightened)
                        ghost.state = GhostState.eaten;
                }

                // Checks if the player is on a Pellet
                if (obj.GetType() == typeof(SmallPellet) ||
                    obj.GetType() == typeof(BigPellet))
                {
                    if (obj.GetType() == typeof(BigPellet))
                    {
                        pinkGhost.state = GhostState.frightened;
                        redGhost.state = GhostState.frightened;
                    }
                    // Add picked up item's score value to player's score
                    player.plyrScore.AddScore(obj.ScoreVal);

                    if (physicsObjects.Contains(obj))
                        physicsObjects[physicsObjects.IndexOf(obj)] = new EmptySpace(player.Pos.X, player.Pos.Y);
                    // Updates visual for position player was in if there was a
                    // a pickable on it
                    mapVisuals[obj.Pos.X, obj.Pos.Y] = ' ';
                }

                // Checks if the player is on a Teleporter
                if (obj.GetType() == typeof(Teleporter))
                {
                    // If his postition is 0 teleports him to 26 else teleports him
                    // to 1
                    player.Pos.X = player.Pos.X == 0 ? 26 : 1;
                }
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
            db[redGhost.Pos.X, redGhost.Pos.Y] = redGhost.Visuals;
            db[pinkGhost.Pos.X, pinkGhost.Pos.Y] = 'G';

            db.Swap();

            // -------------UnComment the commented sections to generate colors
            // -------------Impacts performace ALOT!!!-------------------------
            for (int y = 0; y < db.YDim; y++)
            {
                for (int x = 0; x < db.XDim; x++)
                {
                    if (db[x, y] == 'O')                                    // <------------------------------
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    if (db[x, y] == 'c' || db[x, y] == 'o')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }                                                       // <-------------------------------
                    Console.Write(db[x, y]);
                    Console.ResetColor();                                   // <-------------------------------
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
                    if (mapVisuals[x, y] == ' ')
                    {
                        physicsObjects.Add(new EmptySpace(x, y));
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

        private void UpdateGhost(Ghost ghost)
        {
            if (stateSwapTimer >= 20 && ghost.state == GhostState.chase)
            {
                ghost.state = GhostState.scatter;
                updateTimer = true;
            }
            else if (stateSwapTimer >= 7 && ghost.state == GhostState.scatter)
            {
                ghost.state = GhostState.chase;
                updateTimer = true;
            }
        }
    }
}