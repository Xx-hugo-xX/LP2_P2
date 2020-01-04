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
        private readonly Thread keyReader;
        private readonly Thread pathingAI;

        private readonly Ghost redGhost;
        private readonly Ghost pinkGhost;
        private readonly Ghost orangeGhost;
        private readonly Ghost blueGhost;

        private readonly DefaultObject pinkTarget = new DefaultObject(0, 0, ' ', ObjectType.target);
        private readonly DefaultObject orangeTarget = new DefaultObject(0, 0, ' ', ObjectType.target);
        private readonly DefaultObject blueTarget = new DefaultObject(0, 0, ' ', ObjectType.target);

        private bool running;
        private bool updateTimer = false;
        private int ghostUpdateTimer = 0;
        private int stateSwapTimer = 0;
        private int frightenTimer = 0;

        private readonly List<Object> physicsObjects = new List<Object>();
        private readonly Physics col;
        private readonly char[,] mapVisuals = new char[27, 23];

        private readonly string mapBuilder =
        "OOOOOOOOOOOOOOOOOOOOOOOOOOO" +
        "O............O............O" +
        "O-OOOO.OOOOO.O.OOOOO.OOOO-O" +
        "O.........................O" +
        "O.OOOO.OO.OOOOOOO.OO.OOOO.O" +
        "O......OO....O....OO......O" +
        "OOOOOO.OOOOO.O.OOOOO.OOOOOO" +
        "     O.OO         OO.O     " +
        "     O.OO OOO OOO OO.O     " +
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

            redGhost = new Ghost(12, 11, physicsObjects, 1, 1, 'R', 200);
            pinkGhost = new Ghost(13, 11, physicsObjects, 25, 1, 'P', 200);
            orangeGhost = new Ghost(14, 11, physicsObjects, 1, 21, 'G', 200);
            blueGhost = new Ghost(15, 11, physicsObjects, 25, 21, 'B', 200);

            physicsObjects.Add(redGhost);
            physicsObjects.Add(pinkGhost);
            physicsObjects.Add(orangeGhost);
            physicsObjects.Add(blueGhost);

            col = new Physics(physicsObjects);

            db = new DoubleBuffer2D<char>(30, 30);
            inputSys = new InputSystem();
            keyReader = new Thread(inputSys.ReadKeys);
            pathingAI = new Thread(UpdateGhostBehaviour);
            Console.CursorVisible = false;
        }

        public void Loop()
        {
            keyReader.Start();
            pathingAI.Start();
            running = true;
            while (running)
            {
                long start = DateTime.Now.Ticks;
                inputSys.ProcessInput();
                Update(mapVisuals);
                Render();
                //Thread.Sleep(Math.Abs(
                //    (int)(start / 20000)
                //    + 20
                //    - (int)(DateTime.Now.Ticks / 20000)));
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
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall)
                        {
                            // Decreases the y of the player by 1
                            player.Pos.Y = Math.Max(0, player.Pos.Y - 1);
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;

                    case Direction.Left:
                        wallDetection = col.Collision(player, -1, 0);
                        // Checks if the next position left is not a wall
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall)
                        {
                            // Decreases the x of the player by 1
                            player.Pos.X = Math.Max(0, player.Pos.X - 1);
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;

                    case Direction.Down:
                        wallDetection = col.Collision(player, 0, 1);
                        // Checks if the next position down is not a wall
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall)
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
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall)
                        {
                            // Increases the X of the player by 1
                            player.Pos.X =
                                Math.Min(db.XDim - 1, player.Pos.X + 1);
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;
                }
                // UpdateGhostBehaviour();
                ghostUpdateTimer++;
            }

            // Updates the collider of the player to his current position
            player.UpdatePhysics();

            Object obj = col.Collision(player);

            if (obj != null)
            {
                if (obj.ObjType == ObjectType.ghost)
                {
                    Ghost ghost = obj as Ghost;
                    if (ghost.state == GhostState.frightened)
                        ghost.state = GhostState.eaten;
                }

                if (obj.ObjType == ObjectType.bigPellet)
                {
                    redGhost.state = GhostState.frightened;
                    pinkGhost.state = GhostState.frightened;
                    orangeGhost.state = GhostState.frightened;
                    blueGhost.state = GhostState.frightened;

                    physicsObjects[physicsObjects.IndexOf(obj)] =
                        new DefaultObject(player.Pos.X, player.Pos.Y, ' ',
                        ObjectType.emptySpace);

                    mapVisuals[obj.Pos.X, obj.Pos.Y] = ' ';
                }
                // Checks if the player is on a Pellet
                if (obj.ObjType == ObjectType.pellet)
                {
                    physicsObjects[physicsObjects.IndexOf(obj)] =
                        new DefaultObject(player.Pos.X, player.Pos.Y, ' ',
                        ObjectType.emptySpace);

                    // Updates visual for position player was in if there was a
                    // a pickable on it
                    mapVisuals[obj.Pos.X, obj.Pos.Y] = ' ';
                }

                // Checks if the player is on a Teleporter
                if (obj.ObjType == ObjectType.teleporter)
                {
                    // If his postition is 0 teleports him to 26 else teleports him
                    // to 1
                    player.Pos.X = player.Pos.X == 0 ? 26 : 1;
                }

                // Add picked up item's score value to player's score
                player.plyrScore.AddScore(obj.ScoreVal);
            }
        }

        public void Render()
        {
            Console.WriteLine(player.plyrScore);
            // Loop for the amount of chars in the second position of the array
            for (int y = 0; y < mapVisuals.GetLength(1); y++)
            {
                // Loop for the amount of chars in the first position of the array
                for (int x = 0; x < mapVisuals.GetLength(0); x++)
                {
                    // Assigns the corresponding visual to the buffer
                    db[x, y] = mapVisuals[x, y];
                }
            }

            // Puts the player position on the buffer
            db[player.Pos.X, player.Pos.Y] = player.Visuals;

            SetBufferGhostVisuals(redGhost);
            SetBufferGhostVisuals(pinkGhost);
            SetBufferGhostVisuals(orangeGhost);
            SetBufferGhostVisuals(blueGhost);

            db.Swap();

            for (int y = 0; y < db.YDim; y++)
            {
                for (int x = 0; x < db.XDim; x++)
                {
                    if (db[x, y] == 'O')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    if (db[x, y] == 'f')
                    {
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    if (db[x, y] == 'C')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }

                    if (db[x, y] == 'R')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    if (db[x, y] == 'P')
                    {
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    }
                    if (db[x, y] == 'G')
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    if (db[x, y] == 'B')
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                    Console.Write(db[x, y]);
                    Console.ResetColor();
                }
                Console.Write('\n');
            }
            Console.SetCursorPosition(0, 0);
            db.Clear();
        }
        private void SetBufferGhostVisuals(Ghost ghost)
        {
            if (ghost.state == GhostState.frightened)
                db[ghost.Pos.X, ghost.Pos.Y] = 'f';
            else if (ghost.state == GhostState.eaten)
                db[ghost.Pos.X, ghost.Pos.Y] = '"';
            else
                db[ghost.Pos.X, ghost.Pos.Y] = ghost.Visuals;

        }
        /// <summary>
        /// Assembles the map by creating the necessary things and adding them
        /// to a list
        /// </summary>
        private void GenerateMap()
        {
            // Loop for the amount of lines in the char array
            for (int y = 0; y < mapVisuals.GetLength(1); y++)
            {
                // Loop for the amount of characters in the char array
                for (int x = 0; x < mapVisuals.GetLength(0); x++)
                {
                    // If the current char is a O creates a wall
                    if (mapVisuals[x, y] == 'O')
                    {
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new DefaultObject(x, y,
                            mapVisuals[x, y], ObjectType.wall));
                    }
                    // If the current char is a . creates a Pellet
                    if (mapVisuals[x, y] == '.')
                    {
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new DefaultObject(x, y,
                            mapVisuals[x, y], ObjectType.pellet, 10));
                    }
                    // If the current char is a . creates a Pellet
                    if (mapVisuals[x, y] == '-')
                    {
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new DefaultObject(x, y,
                            mapVisuals[x, y], ObjectType.bigPellet, 50));
                    }
                    // If the current char is a T creates a teleporter
                    if (mapVisuals[x, y] == 'T')
                    {
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new DefaultObject(x, y,
                            mapVisuals[x, y], ObjectType.teleporter));
                    }
                    if (mapVisuals[x, y] == ' ')
                    {
                        physicsObjects.Add(new DefaultObject(x, y,
                            mapVisuals[x, y], ObjectType.emptySpace));
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

        private void UpdateGhostState(Ghost ghost)
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
            if (ghost.state == GhostState.frightened)
            {
                if (frightenTimer >= 100000000)
                {
                    updateTimer = true;
                    ghost.state = GhostState.chase;
                }
                frightenTimer++;
            }
        }

        private void UpdateGhostBehaviour()
        {
            while (true)
            {
                UpdateGhostState(redGhost);
                UpdateGhostState(pinkGhost);
                UpdateGhostState(orangeGhost);
                UpdateGhostState(blueGhost);

                if (ghostUpdateTimer > 1)
                {
                    ghostUpdateTimer = 0;

                    stateSwapTimer++;

                    if (updateTimer)
                    {
                        stateSwapTimer = 0;
                        frightenTimer = 0;
                        updateTimer = false;
                    }

                    if ((int)Math.Sqrt(Math.Pow(Math.Abs(
                        player.Pos.X - orangeGhost.Pos.X), 2) + Math.Pow(
                        Math.Abs(player.Pos.Y - orangeGhost.Pos.Y), 2)) >= 7)
                    {
                        orangeTarget.Pos = player.Pos;
                    }
                    else
                        orangeTarget.Pos = new Position(1, 21);

                    if (!(player.OldPos == player.Pos) ||
                        pinkGhost.Pos == pinkTarget.Pos)
                    {
                        pinkTarget.Pos = new Position(
                            ((player.Pos.X - player.OldPos.X) * 3)
                            + player.Pos.X,
                            ((player.Pos.Y - player.OldPos.Y) * 3)
                            + player.Pos.Y);
                    }

                    blueTarget.Pos = new Position(Math.Max
                        (1, Math.Min(mapVisuals.GetLength(0) - 2,
                        (player.Pos.X - redGhost.Pos.X) + player.Pos.X)),
                        Math.Max(1, Math.Min(mapVisuals.GetLength(1) - 2,
                        (player.Pos.Y - redGhost.Pos.Y) + player.Pos.Y)));

                    pinkGhost.CalcuatePath(pinkTarget);
                    redGhost.CalcuatePath(player);
                    orangeGhost.CalcuatePath(orangeTarget);
                    blueGhost.CalcuatePath(blueTarget);

                    pinkGhost.UpdatePosition();
                    redGhost.UpdatePosition();
                    orangeGhost.UpdatePosition();
                    blueGhost.UpdatePosition();
                }
            }
        }

    }
}