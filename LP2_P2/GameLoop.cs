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
        private Thread keyReader;

        private readonly Ghost redGhost;
        private readonly Ghost pinkGhost;
        private readonly Ghost orangeGhost;
        private readonly Ghost blueGhost;

        private readonly DefaultObject pinkTarget = new DefaultObject(0, 0, ' ', ObjectType.target);
        private readonly DefaultObject orangeTarget = new DefaultObject(0, 0, ' ', ObjectType.target);
        private readonly DefaultObject blueTarget = new DefaultObject(0, 0, ' ', ObjectType.target);

        private bool updateTimer = false;
        private int level;
        private int ghostUpdateTimer = 0;
        private bool updateSwapTimer = false;
        private int stateSwapTimer = DateTime.Now.Second;
        private int frightenTimer = DateTime.Now.Second;

        private readonly List<Object> physicsObjects = new List<Object>();
        private readonly Physics col;
        private readonly char[,] mapVisuals = new char[27, 23];

        private readonly string mapBuilder =
        "OOOOOOOOOOOOOOOOOOOOOOOOOOO" +
        "O............O............O" +
        "OºOOOO.OOOOO.O.OOOOO.OOOOºO" +
        "O.........................O" +
        "O.OOOO.OO.OOOOOOO.OO.OOOO.O" +
        "O......OO....O....OO......O" +
        "OOOOOO.OOOOO.O.OOOOO.OOOOOO" +
        "     O.OO         OO.O     " +
        "     O.OO OOO-OOO OO.O     " +
        "OOOOOO.OO O     O OO.OOOOOO" +
        "T......   O     O   ......T" +
        "OOOOOO.OO O     O OO.OOOOOO" +
        "     O.OO OOOOOOO OO.O     " +
        "     O.OO    F    OO.O     " +
        "OOOOOO.OO OOOOOOO OO.OOOOOO" +
        "O............O............O" +
        "O.OOOO.OOOOO.O.OOOOO.OOOO.O" +
        "Oº..OO....... .......OO..ºO" +
        "OOO.OO.OO.OOOOOOO.OO.OO.OOO" +
        "O......OO....O....OO......O" +
        "O.OOOOOOOOOO.O.OOOOOOOOOO.O" +
        "O.........................O" +
        "OOOOOOOOOOOOOOOOOOOOOOOOOOO";

        public HighScoreManager HSManager;

        public GameLoop(HighScoreManager hsManager)
        {
            level = 1;
            player = new Player();

            physicsObjects.Add(player);
            ConvertMapToDoubleArray();
            GenerateMap();
            GeneratePickables();

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
            HSManager = hsManager;
            Console.CursorVisible = false;
        }

        public void Loop()
        {
            keyReader = new Thread(inputSys.ReadKeys);
            keyReader.Name = "InputThread";
            keyReader.Start();
            while (inputSys.IsRunning)
            {
                inputSys.ProcessInput();
                Update(mapVisuals);
                Render();
            }
            inputSys.ResetInput();
            keyReader.Join(0);
        }

        public void Update(char[,] mapVisuals)
        {
            // Checks for level finish (all pellets collected) and passes
            // to the next level if the conditions are met, generating the
            // map and pickables again and resets the player's position
            CheckForLevelFinish();
            // Updates the player's direction
            UpdatePlayerDirection();
            // Updates the collider of the player to his current position
            player.UpdatePhysics();
            // Updates the colliders of the Ghosts to their current position
            redGhost.UpdatePhysics();
            pinkGhost.UpdatePhysics();
            orangeGhost.UpdatePhysics();
            blueGhost.UpdatePhysics();
            // Checks for collisions and executes the logic of the interactions
            // of the player with other objects
            CheckForCollisions();
        }

        public void Render()
        {
            Console.WriteLine($"{player.plyrScore}\t\tLevel: {level}");
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

            // Sets the visuals of the various ghosts according to their
            // current states
            SetBufferGhostVisuals(redGhost);
            SetBufferGhostVisuals(pinkGhost);
            SetBufferGhostVisuals(orangeGhost);
            SetBufferGhostVisuals(blueGhost);

            // Swap the current frame for the next frame of the DoubleBuffer
            db.Swap();

            // Update objects displayed
            // Loop throught the doublebuffers YDim, starting at 0
            for (int y = 0; y < db.YDim; y++)
            {
                // Loop throught the doublebuffers XDim, starting at 0
                for (int x = 0; x < db.XDim; x++)
                {
                    // Sets ForegroundColor to White
                    Console.ForegroundColor = ConsoleColor.White;

                    // if statement that checks if the char in the specified
                    // position of the doublebuffer is 'O'
                    if (db[x, y] == 'O')
                    {
                        // Sets BackgroundColor to DarkBlue
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        // Sets ForegroundColor to DarkBlue
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    // else if statement that checks if the char in the 
                    // specified position of the doublebuffer is 'O'
                    else if (db[x, y] == 'C')
                    {
                        // Sets ForegroundColor to DarkYellow
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    // else if statement that checks if the char in the 
                    // specified position of the doublebuffer is 'O'
                    else if (db[x, y] == 'f')
                    {
                        // Sets BackgroundColor to Cyan
                        Console.BackgroundColor = ConsoleColor.Cyan;
                    }
                    // else if statement that checks if the char in the 
                    // specified position of the doublebuffer is 'O'
                    else if (db[x, y] == 'R')
                    {
                        // Sets BackgroundColor to DarkRed
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    }
                    // else if statement that checks if the char in the 
                    // specified position of the doublebuffer is 'O'
                    else if (db[x, y] == 'P')
                    {
                        // Sets BackgroundColor to Magenta
                        Console.BackgroundColor = ConsoleColor.Magenta;
                    }
                    // else if statement that checks if the char in the 
                    // specified position of the doublebuffer is 'O'
                    else if (db[x, y] == 'G')
                    {
                        // Sets BackgroundColor to Red
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    // else if statement that checks if the char in the 
                    // specified position of the doublebuffer is 'O'
                    else if (db[x, y] == 'B')
                    {
                        // Sets BackgroundColor to Blue
                        Console.BackgroundColor = ConsoleColor.Blue;
                    }
                    // else if statement that checks if the char in the 
                    // specified position of the doublebuffer is 'O'
                    else if (db[x, y] == 'T')
                    {
                        // Sets ForegroundColor to Black
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    // Writes the doublebuffer's current char of index x and y
                    Console.Write(db[x, y]);
                    // Reset the console's color
                    Console.ResetColor();
                }
                Console.Write('\n');
            }
            // Sets CursorPosition to 0 in x and 0 in y
            Console.SetCursorPosition(0, 0);
            // Clears the doublebuffer
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
        /// to the physicsObjects list
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
                    // If the current char is a - creates a door
                    if (mapVisuals[x, y] == '-')
                    {
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new DefaultObject(x, y,
                            mapVisuals[x, y], ObjectType.door));
                    }
                }
            }
        }
        /// <summary>
        /// Generates the pickables to be placed in the map and adding them to
        /// the physicsObjects list
        /// </summary>
        private void GeneratePickables()
        {
            // Loop for the amount of lines in the char array
            for (int y = 0; y < mapVisuals.GetLength(1); y++)
            {
                // Loop for the amount of characters in the char array
                for (int x = 0; x < mapVisuals.GetLength(0); x++)
                {
                    // If the current char is a . creates a Pellet
                    if (mapVisuals[x, y] == '.')
                    {
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new DefaultObject(x, y,
                            mapVisuals[x, y], ObjectType.pellet, 10));
                    }
                    // If the current char is a º creates a Pellet
                    if (mapVisuals[x, y] == 'º')
                    {
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new DefaultObject(x, y,
                            mapVisuals[x, y], ObjectType.bigPellet, 50));
                    }
                    // If the current char is a B creates a BonusFruit
                    if (mapVisuals[x, y] == 'F')
                    {
                        if (physicsObjects.Exists(obj => obj.ObjType ==
                        ObjectType.bonusFruit))
                        {
                            physicsObjects.Remove(physicsObjects.Find(
                                obj => obj.ObjType == ObjectType.bonusFruit));
                        }
                        // Creates and adds that Object to the list
                        physicsObjects.Add(new DefaultObject(x, y, 'F',
                            ObjectType.bonusFruit,
                            Math.Min(100 * level, 5000)));
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
        /// <summary>
        /// Updates the player's Direction using the Dir variable of inputSys
        /// and updates the Ghost's behaviour in return if inputSys.Dir is 
        /// different than Direction.None
        /// </summary>
        private void UpdatePlayerDirection()
        {
            // if statement that checks if the Dir variable's value is not
            // Direction.None
            if (inputSys.Dir != Direction.None)
            {
                // Declare wallDetection object, used to determine if the
                // position the player is trying to move to is occupied by a
                // wall or a door
                Object wallDetection;
                // Update player.OldPos.X, assigning player.Pos.X to it
                player.OldPos.X = player.Pos.X;
                // Update player.OldPos.Y, assigning player.Pos.Y to it
                player.OldPos.Y = player.Pos.Y;

                // switch statement that determines the position the player
                // will attempt to move to based on the inputSys.Dir variable
                switch (inputSys.Dir)
                {
                    // In case Direction.Up
                    case Direction.Up:
                        // Assigns value returned by col.Collision of the
                        // position desired to wall detection
                        wallDetection = col.Collision(player, 0, -1);
                        // Checks if the next position left is not a wall
                        // or a door
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall && wallDetection.ObjType
                            != ObjectType.door)
                        {
                            // Decreases the y of the player by 1
                            player.Pos.Y = Math.Max(0, player.Pos.Y - 1);
                            // Assigns the value of inputSys.Dir to
                            // inputSys.LastDir
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;
                    // In case Direction.Left
                    case Direction.Left:
                        // Assigns value returned by col.Collision of the
                        // position desired to wall detection
                        wallDetection = col.Collision(player, -1, 0);
                        // Checks if the next position left is not a wall
                        // or a door
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall && wallDetection.ObjType
                            != ObjectType.door)
                        {
                            // Decreases the x of the player by 1
                            player.Pos.X = Math.Max(0, player.Pos.X - 1);
                            // Assigns the value of inputSys.Dir to
                            // inputSys.LastDir
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;
                    // In case Direction.Down
                    case Direction.Down:
                        // Assigns value returned by col.Collision of the
                        // position desired to wall detection
                        wallDetection = col.Collision(player, 0, 1);
                        // Checks if the next position left is not a wall
                        // or a door
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall && wallDetection.ObjType
                            != ObjectType.door)
                        {
                            // Increases the y of the player by 1
                            player.Pos.Y =
                                Math.Min(db.YDim - 1, player.Pos.Y + 1);
                            // Assigns the value of inputSys.Dir to
                            // inputSys.LastDir
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;
                    // In case Direction.Right
                    case Direction.Right:
                        // Assigns value returned by col.Collision of the
                        // position desired to wall detection
                        wallDetection = col.Collision(player, 1, 0);
                        // Checks if the next position left is not a wall
                        // or a door
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall && wallDetection.ObjType
                            != ObjectType.door)
                        {
                            // Increases the X of the player by 1
                            player.Pos.X =
                                Math.Min(db.XDim - 1, player.Pos.X + 1);
                            // Assigns the value of inputSys.Dir to
                            // inputSys.LastDir
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;
                }
                // Updates the ghosts' behaviours
                UpdateGhostBehaviour();
            }
        }
        /// <summary>
        /// Checks for player's collisions with other objects and, in case
        /// anything happens upon the colision, realises the interaction's
        /// logic
        /// </summary>
        private void CheckForCollisions()
        {
            List<Object> obj = col.Collision(player);

            if (obj != null)
            {
                for (int i = 0; i < obj.Count; i++)
                {
                    if (obj[i].ObjType == ObjectType.ghost)
                    {
                        Ghost ghost = obj[i] as Ghost;
                        if (ghost.state == GhostState.frightened)
                        {
                            ghost.state = GhostState.eaten;
                    
                            // Add picked up item's score value to player's score
                            player.plyrScore.AddScore(obj[i].ScoreVal);
                        }
                    
                        else player.Death(inputSys, HSManager);
                    }
                    // Checks if the player is on a Pellet
                    if (obj[i].ObjType == ObjectType.pellet ||
                            obj[i].ObjType == ObjectType.bigPellet ||
                            obj[i].ObjType == ObjectType.bonusFruit)
                    {
                        if (obj[i].ObjType == ObjectType.bigPellet)
                        {
                            redGhost.state = GhostState.frightened;
                            pinkGhost.state = GhostState.frightened;
                            orangeGhost.state = GhostState.frightened;
                            blueGhost.state = GhostState.frightened;

                            frightenTimer = DateTime.Now.Second;
                        }

                        physicsObjects[physicsObjects.IndexOf(obj[i])] =
                            new DefaultObject(player.Pos.X, player.Pos.Y, ' ',
                            ObjectType.emptySpace);

                        // Updates visual for position player was in if there was a
                        // a pickable on it
                        mapVisuals[obj[i].Pos.X, obj[i].Pos.Y] = ' ';

                        // Add picked up item's score value to player's score
                        player.plyrScore.AddScore(obj[i].ScoreVal);
                    }
                    // Checks if the player is on a Teleporter
                    if (obj[i].ObjType == ObjectType.teleporter)
                    {
                        // If his postition is 0 teleports him to 26 else teleports him
                        // to 1
                        player.Pos.X = player.Pos.X == 0 ?
                            mapVisuals.GetLength(0) - 2 : 1;
                    }
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
            if (Math.Abs(stateSwapTimer - DateTime.Now.Second) >= 20 && ghost.state == GhostState.chase)
            {
                // switches the state back to scatter
                ghost.state = GhostState.scatter;
                // Queues the timer update
                updateSwapTimer = true;
            }
            // Checks if the timer is bigger than 7 and is in scatter mode
            else if (Math.Abs(stateSwapTimer - DateTime.Now.Second) >= 5 && ghost.state == GhostState.scatter)
            {
                // switches the state back to chase mode
                ghost.state = GhostState.chase;
                // Queues the timer update
                updateSwapTimer = true;
            }
            // Checks if the current state is frightened
            if (ghost.state == GhostState.frightened)
            {
                // Checks if the current timer is bigger than the number
                if ((Math.Abs(frightenTimer - DateTime.Now.Second) >= 6))
                {
                    // Switches the state back to chase mode
                    ghost.state = GhostState.chase;
                }
            }
        }
        /// <summary>
        /// Responsible for most of the AI logic and different behaviours
        /// </summary>
        private void UpdateGhostBehaviour()
        {
            // Checks if the current state of the passed ghosts needs to be
            // changed
            UpdateGhostState(redGhost);
            UpdateGhostState(pinkGhost);
            UpdateGhostState(orangeGhost);
            UpdateGhostState(blueGhost);

            //// Adds 1 to the state swaping timer
            //stateSwapTimer++;

            // If the update timer bool is true (was queued)
            if (updateSwapTimer)
            {
                // Resets both timers used on UpdateGhostState to 0
                stateSwapTimer = DateTime.Now.Second;
                frightenTimer = DateTime.Now.Second;
                // Resets the bool back to false;
                updateSwapTimer = false;
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
            pinkGhost.CalculatePath(pinkTarget);
            redGhost.CalculatePath(player);
            orangeGhost.CalculatePath(orangeTarget);
            blueGhost.CalculatePath(blueTarget);

            // Updates the current position and the colider of the
            // ghost acording to the path calculates above
            pinkGhost.UpdatePosition();
            redGhost.UpdatePosition();
            orangeGhost.UpdatePosition();
            blueGhost.UpdatePosition();
        }

        // Increments level number, clears physicsObjects list,
        // generates the map again, while keeping player's score, places player
        // back in his starting position and resets input
        private void CheckForLevelFinish()
        {
            if (!physicsObjects.Exists(obj => obj.ObjType == ObjectType.pellet))
            {
                level++;
                // Clear list of physics objects, to generate the level again
                // into that list
                physicsObjects.Clear();
                ConvertMapToDoubleArray();
                GenerateMap();
                GeneratePickables();
                player.Pos = new Position(13, 17);
                inputSys.ResetInput();
            }
        }
    }
}