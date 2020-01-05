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
        "T......   O     O   ......T" +
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

        public HighScoreManager HSManager;

        public GameLoop(HighScoreManager hsManager)
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

            HSManager = hsManager;

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


                // Updates the current position and the colider of the
                // ghost acording to the path calculates above
                pinkGhost.UpdatePosition();
                redGhost.UpdatePosition();
                orangeGhost.UpdatePosition();
                blueGhost.UpdatePosition();
            }

            // Updates the collider of the player to his current position
            player.UpdatePhysics();

            List<Object> obj = col.Collision(player);

            if (obj != null)
            {
                for (int i = 0; i < obj.Count; i++)
                {
                    if (obj[i].ObjType == ObjectType.ghost)
                    {
                        Ghost ghost = obj[i] as Ghost;
                        if (ghost.state == GhostState.frightened)
                            ghost.state = GhostState.eaten;
                    }
                    // Checks if the player is on a Pellet
                    if (obj[i].ObjType == ObjectType.pellet ||
                            obj[i].ObjType == ObjectType.bigPellet)
                    {
                        if (obj[i].ObjType == ObjectType.bigPellet)
                        {
                            redGhost.state = GhostState.frightened;
                            pinkGhost.state = GhostState.frightened;
                            orangeGhost.state = GhostState.frightened;
                            blueGhost.state = GhostState.frightened;
                        }

                        physicsObjects[physicsObjects.IndexOf(obj[i])] =
                            new DefaultObject(player.Pos.X, player.Pos.Y, ' ',
                            ObjectType.emptySpace);

                        // Updates visual for position player was in if there was a
                        // a pickable on it
                        mapVisuals[obj[i].Pos.X, obj[i].Pos.Y] = ' ';
                    }
                    // Checks if the player is on a Teleporter
                    if (obj[i].ObjType == ObjectType.teleporter)
                    {
                        // If his postition is 0 teleports him to 26 else teleports him
                        // to 1
                        player.Pos.X = player.Pos.X == 0 ? 26 : 1;
                    }

                    // Add picked up item's score value to player's score
                    player.plyrScore.AddScore(obj[i].ScoreVal);
                }

                if (obj.GetType() == typeof(Ghost))
                {
                    KillPlayer();
                }
            }
        }

        public void Render()
        {
            Console.WriteLine(player.plyrScore.ToString());
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
                    Console.ForegroundColor = ConsoleColor.White;

                    if (db[x, y] == 'O')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    else if (db[x, y] == 'f')
                    {
                        Console.BackgroundColor = ConsoleColor.Cyan;
                    }
                    else if (db[x, y] == 'C')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else if (db[x, y] == 'R')
                    {
                        db[x, y] = '"';
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    }
                    else if (db[x, y] == 'P')
                    {
                        db[x, y] = '"';
                        Console.BackgroundColor = ConsoleColor.Magenta;
                    }
                    else if (db[x, y] == 'G')
                    {
                        db[x, y] = '"';
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    else if (db[x, y] == 'B')
                    {
                        db[x, y] = '"';
                        Console.BackgroundColor = ConsoleColor.Blue;
                    }
                    Console.Write(db[x, y]);
                    Console.ResetColor();
                }
                Console.Write('\n');
            }
            Console.SetCursorPosition(0, 0);
            db.Clear();
        }
        /// <summary>
        /// Changes the visuals of the ghosts will appear acording to their
        /// state
        /// </summary>
        /// <param name="ghost"> The current ghost being checked </param>
        private void SetBufferGhostVisuals(Ghost ghost)
        {
            // Checks if the ghost is in frightened state
            if (ghost.state == GhostState.frightened)
                // Changes their visual to an f
                db[ghost.Pos.X, ghost.Pos.Y] = 'f';
            // Checks if the ghost is in a eaten state
            else if (ghost.state == GhostState.eaten)
                // Changes their visual to an "
                db[ghost.Pos.X, ghost.Pos.Y] = '"';
            else
                // If it's none of the above sets it to it's usual visual
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
                    // If the current char is empty creates an empty space
                    if (mapVisuals[x, y] == ' ')
                    {
                        // Creates and Adds that Object to the list
                        physicsObjects.Add(new DefaultObject(x, y,
                            mapVisuals[x, y], ObjectType.emptySpace));
                    }
                }
            }
        }

        private void KillPlayer()
        {
            running = false;
            HSManager.AddHighScore(player.plyrScore);
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
        /// <summary>
        /// Updates the state of the ghost acording to a timer or a collision
        /// </summary>
        /// <param name="ghost"></param>
        private void UpdateGhostState(Ghost ghost)
        {
            // Checks if the timer is bigger than 20 and is in chase mode
            if (stateSwapTimer >= 20 && ghost.state == GhostState.chase)
            {
                // switches the state back to scatter
                ghost.state = GhostState.scatter;
                // Queues the timer update
                updateTimer = true;
            }
            // Checks if the timer is bigger than 7 and is in scatter mode
            else if (stateSwapTimer >= 7 && ghost.state == GhostState.scatter)
            {
                // switches the state back to chase mode
                ghost.state = GhostState.chase;
                // Queues the timer update
                updateTimer = true;
            }
            // Checks if the current state is frightened
            if (ghost.state == GhostState.frightened)
            {
                // Checks if the current timer is bigger than the number
                if (frightenTimer >= 100000000)
                {
                    // Queues the timer update
                    updateTimer = true;
                    // Switches the state back to chase mode
                    ghost.state = GhostState.chase;
                }
                // Adds 1 to the timer
                frightenTimer++;
            }
        }
        /// <summary>
        /// Responsible for most of the AI logic and different behaviours
        /// </summary>
        private void UpdateGhostBehaviour()
        {
            // A while loop with no end
            while (true)
            {
                // Checks if the current state of the passed ghost needs to be
                // changed
                UpdateGhostState(redGhost);
                UpdateGhostState(pinkGhost);
                UpdateGhostState(orangeGhost);
                UpdateGhostState(blueGhost);

                    // Adds 1 to the state swaping timer
                    stateSwapTimer++;

                    // If the update timer bool is true (was queued)
                    if (updateTimer)
                    {
                        // Resets both timers used on UpdateGhostState to 0
                        stateSwapTimer = 0;
                        frightenTimer = 0;
                        // Resets the bool back to false;
                        updateTimer = false;
                    }
                    // Checks if the distance between the player and the 
                    // orange ghost is greater than 7 units
                    if ((int)Math.Sqrt(Math.Pow(Math.Abs(
                        player.Pos.X - orangeGhost.Pos.X), 2) + Math.Pow(
                        Math.Abs(player.Pos.Y - orangeGhost.Pos.Y), 2)) >= 7)
                    {
                        // The target for the orange ghost become the player
                        orangeTarget.Pos = player.Pos;
                    }
                    else
                        // The target is the coordinates of its corner
                        orangeTarget.Pos = new Position(1, 21);

                    // checks if the old position of the player is not the
                    // current position and the the current position of the
                    // pink ghost is not it's target
                    if (!(player.OldPos == player.Pos) ||
                        pinkGhost.Pos == pinkTarget.Pos)
                    {
                        // Finds the position the player is moving towards and
                        // multiplies it by 3 to get the poisition 3 squares 
                        // ahead of the player
                        pinkTarget.Pos = new Position(
                            ((player.Pos.X - player.OldPos.X) * 3)
                            + player.Pos.X,
                            ((player.Pos.Y - player.OldPos.Y) * 3)
                            + player.Pos.Y);
                    }
                    // Finds the position at the middle of the player and the
                    // red ghost and flips it 180 degrees while clamping it
                    // inside the drwawable area
                    blueTarget.Pos = new Position(Math.Max
                        (1, Math.Min(mapVisuals.GetLength(0) - 2,
                        (player.Pos.X - redGhost.Pos.X) + player.Pos.X)),
                        Math.Max(1, Math.Min(mapVisuals.GetLength(1) - 2,
                        (player.Pos.Y - redGhost.Pos.Y) + player.Pos.Y)));


                    // Calculates the path of the ghost giving it a target
                    pinkGhost.CalcuatePath(pinkTarget);
                    redGhost.CalcuatePath(player);
                    orangeGhost.CalcuatePath(orangeTarget);
                    blueGhost.CalcuatePath(blueTarget);
                
            }
        }
    }
}