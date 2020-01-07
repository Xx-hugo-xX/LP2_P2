using System;
using System.Collections.Generic;
using System.Threading;

namespace LP2_P2
{
    public class GameLoop
    {
        #region -- variables ---

        // A plubic HighScoreManager 
        public HighScoreManager HSManager;

        // Creates a variable of type player
        private readonly Player player;

        // Creates a variable of DoubleBuffer for when rendering
        private readonly DoubleBuffer2D<char> db;
        // Creates a variable of InputSystem for use on a different thread
        private readonly InputSystem inputSys;
        // Creates the thread for the InputSystem variable
        private readonly Thread keyReader;
        
        // Creates a variable of Ghost for all of the 4 ghosts
        private readonly Ghost redGhost;
        private readonly Ghost pinkGhost;
        private readonly Ghost orangeGhost;
        private readonly Ghost blueGhost;

        // Creates 3 Targets for the the AI of the different ghosts
        private readonly DefaultObject pinkTarget = 
            new DefaultObject(0, 0, ' ', ObjectType.target);
        private readonly DefaultObject orangeTarget = 
            new DefaultObject(0, 0, ' ', ObjectType.target);
        private readonly DefaultObject blueTarget = 
            new DefaultObject(0, 0, ' ', ObjectType.target);

        // Int for keeping track of the current level
        private int level;
        // Bool for resetting the stateSwapTimer when true
        private bool updateSwapTimer = false;
        // Interval when the ghosts behaviour and position should be updated
        private int ghostUpdateTimer = 0;
        // Timers for when to switch states
        private int stateSwapTimer = DateTime.Now.Second;
        private int frightenTimer = DateTime.Now.Second;

        // List containing all the objects in the map
        private readonly List<Object> physicsObjects = new List<Object>();
        // Creates a variable of type Physics for cheking collisions
        private readonly Physics col;
        // A char array for saving the converted mapBuilder string
        private readonly char[,] mapVisuals = new char[27, 23];

        // A string of how the map should look
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
        "T     .   O     O   .     T" +
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
        #endregion

        /// <summary>
        /// Constructor of the class GameLoop initializing all the variables
        /// created above
        /// </summary>
        /// <param name="hsManager"> The highScoreManager passed </param>
        public GameLoop(HighScoreManager hsManager)
        {
            level = 1;
            // Converts the string for visualizing the map into a double array
            ConvertMapToDoubleArray();
            // Creates and adds the Objects needed to form the map
            GenerateMap();

            // Generate map's pickables and add then to the physicsObjects list
            GeneratePickables();

            // Creates a new Player
            player = new Player();

            // Adds the created player to the physicsObjects list
            physicsObjects.Add(player);

            // Creates the 4 ghosts at the center position passing through
            // their cordinates, the list of physicsObjects, their corner,
            // visuals and the amount of score they should give
            redGhost = new Ghost(12, 11, physicsObjects, 1, 1, 'R', 200);
            pinkGhost = new Ghost(13, 11, physicsObjects, 25, 1, 'P', 200);
            orangeGhost = new Ghost(14, 11, physicsObjects, 1, 21, 'G', 200);
            blueGhost = new Ghost(15, 11, physicsObjects, 25, 21, 'B', 200);

            // Adds the created ghosts to the physicsObjects list
            physicsObjects.Add(redGhost);
            physicsObjects.Add(pinkGhost);
            physicsObjects.Add(orangeGhost);
            physicsObjects.Add(blueGhost);

            // Creates a new Physics passing in all the physicsObjects list
            col = new Physics(physicsObjects);

            // Creates a new DoubleBuffer with a fixed size of 30 by 30
            db = new DoubleBuffer2D<char>(30, 30);
            // Creates a new Input System
            inputSys = new InputSystem();
            // Creates a new Thread and gives it a method of InputSystem
            keyReader = new Thread(inputSys.ReadKeys)
            {
                // Assign a name to the Input Thread
                Name = "InputThread"
            };

            // Equals this HighScoreManager to the one given
            HSManager = hsManager;
            // hides the cursor
            Console.CursorVisible = false;
        }
        /// <summary>
        /// The main loop of the game
        /// </summary>
        public void Loop()
        {
            // Starts the input thread creates
            keyReader.Start();
            // Runs and executes the code while running is true
            while (inputSys.IsRunning)
            {
                // creates a long with the value of the ticks at the moment
                long start = DateTime.Now.Ticks;
                // Processes the input
                inputSys.ProcessInput();
                // Runs the Update method
                Update(mapVisuals);
                // Runs the Render method
                Render();
                Thread.Sleep(Math.Abs(
                    (int)(start / 20000)
                    + 20
                    - (int)(DateTime.Now.Ticks / 20000)));
            }
            // Resets inputSys' input, as to not have unchecked input
            // when the gameloop stops
            inputSys.ResetInput();
            // Joins the keyReader thread
            keyReader.Join(0);
            // Adds the current score to the highscore
            HSManager.AddHighScore(player.plyrScore);
        }
        /// <summary>
        /// Main method of the game responsible for movement, collision and AI
        /// </summary>
        /// <param name="mapVisuals"> The double array of the map </param>
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
        /// <summary>
        /// Assigns the visuals to the DoubleBuffer's next frame, swaps the
        /// current frame for the next and displays, and runs another loop
        /// through the doublebuffer, checking various possible objects in a 
        /// position and changing the console's background and/or foreground 
        /// colours according to the char read, then writes it.
        /// </summary>
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
        /// state.
        /// </summary>
        /// <param name="ghost"> The current ghost being checked </param>
        private void SetBufferGhostVisuals(Ghost ghost)
        {
            // Checks if the ghost is in frightened state
            if (ghost.state == GhostState.frightened)
            {
                // Changes their visual to an f
                db[ghost.Pos.X, ghost.Pos.Y] = 'f';
            }
            // Checks if the ghost is in a eaten state
            else if (ghost.state == GhostState.eaten)
            {
                // Changes their visual to an "
                db[ghost.Pos.X, ghost.Pos.Y] = '"';
            }
            else
            {
                // If it's none of the above sets it to it's usual visual
                db[ghost.Pos.X, ghost.Pos.Y] = ghost.Visuals;
            }
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
            // Checks if the inputSystem has a direction
            if (inputSys.Dir != Direction.None)
            {
                // Creates an object for saving the object the player will
                // collide with
                Object wallDetection;

                // Sets the OldPosition of the Player to it's current one
                player.OldPos.X = player.Pos.X;
                player.OldPos.Y = player.Pos.Y;

                // Switch case acording to the direction
                switch (inputSys.Dir)
                {
                    case Direction.Up:
                        // Retrives the object above the player
                        wallDetection = col.Collision(player, 0, -1);
                        // Checks if the next position up is not a wall
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall && wallDetection.ObjType
                            != ObjectType.door)
                        {
                            // Decreases the y of the player by 1
                            player.Pos.Y = Math.Max(0, player.Pos.Y - 1);
                            // Sets the last direction to the current direction
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;

                    case Direction.Left:
                        // Retrives the object left of the player
                        wallDetection = col.Collision(player, -1, 0);
                        // Checks if the next position left is not a wall
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall && wallDetection.ObjType
                            != ObjectType.door)
                        {
                            // Decreases the x of the player by 1
                            player.Pos.X = Math.Max(0, player.Pos.X - 1);
                            // Sets the last direction to the current direction
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;

                    case Direction.Down:
                        // Retrives the object below the player
                        wallDetection = col.Collision(player, 0, 1);
                        // Checks if the next position down is not a wall
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall && wallDetection.ObjType
                            != ObjectType.door)
                        {
                            // Increases the y of the player by 1
                            player.Pos.Y =
                                Math.Min(db.YDim - 1, player.Pos.Y + 1);
                            // Sets the last direction to the current direction
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;

                    case Direction.Right:
                        // Retrives the object right to the player
                        wallDetection = col.Collision(player, 1, 0);
                        // Checks if the next position right is not a wall
                        if (wallDetection == null || wallDetection.ObjType
                            != ObjectType.wall && wallDetection.ObjType
                            != ObjectType.door)
                        {
                            // Increases the X of the player by 1
                            player.Pos.X =
                                Math.Min(db.XDim - 1, player.Pos.X + 1);
                            // Sets the last direction to the current direction
                            inputSys.LastDir = inputSys.Dir;
                        }
                        break;
                }
                // increments one to the ghostUpdateTimer
                ghostUpdateTimer++;

                // Checks if that timer is bigger than a number
                if (ghostUpdateTimer > 1)
                {
                    // Updates the position and the path of the AI
                    UpdateGhostBehaviour();
                    // Resets the timer
                    ghostUpdateTimer = 0;
                }
            }
        }
        /// <summary>
        /// Checks for player's collisions with other objects and, in case
        /// anything happens upon the colision, realises the interaction's
        /// logic
        /// </summary>
        private void CheckForCollisions()
        {
            // Creates a list of objects the playes is colliding with
            List<Object> obj = col.Collision(player);

            // Checks if the list is not null
            if (obj != null)
            {
                // Runs the code acording to the amount of objects in obj
                for (int i = 0; i < obj.Count; i++)
                {
                    // Checks if the object retrived is a ghost
                    if (obj[i].ObjType == ObjectType.ghost)
                    {
                        // Creates a ghost variable and transforms the object
                        // into a ghost (since we know it's a ghost because of 
                        // the if statement it shouldn't fail)
                        Ghost ghost = obj[i] as Ghost;
                        // Checks if the current ghost is frightened
                        if (ghost.state == GhostState.frightened)
                        {
                            // Switches the state of that ghost to eaten
                            ghost.state = GhostState.eaten;

                            // Add picked up item's score value to player's score
                            player.plyrScore.AddScore(obj[i].ScoreVal);
                        }
                        // If the ghost is not in eaten state
                        else if (ghost.state != GhostState.eaten)
                        {
                            // kills the player ending the game
                            player.Death(inputSys, HSManager);
                        }
                    }
                    // Checks if the player is on a Pellet
                    if (obj[i].ObjType == ObjectType.pellet ||
                            obj[i].ObjType == ObjectType.bigPellet)
                    {
                        // Checks if it's a bigPellet
                        if (obj[i].ObjType == ObjectType.bigPellet)
                        {
                            // Switches the sate of all the ghosts to frighten
                            redGhost.state = GhostState.frightened;
                            pinkGhost.state = GhostState.frightened;
                            orangeGhost.state = GhostState.frightened;
                            blueGhost.state = GhostState.frightened;

                            // Resets the frightenTimer to the current second
                            frightenTimer = DateTime.Now.Second;
                        }

                        // Replaces that Object on physicsObjects
                        //by an empty space
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
            {
                // The target is the coordinates of its corner
                orangeTarget.Pos = new Position(1, 21);
            }
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


        /// <summary>
        /// Increments level number generates pickables again, while keeping 
        /// player's score, places the player back in his starting position 
        /// and resets input.
        /// </summary>
        private void CheckForLevelFinish()
        {
            // if statement that checks if any objects of type pellet exist
            // in the physicsObjects list
            if (!physicsObjects.Exists(obj => obj.ObjType == ObjectType.pellet))
            {
                // Increment level number
                level++;
                // Convert map string to double array
                ConvertMapToDoubleArray();
                // Generate all pickables
                GeneratePickables();
                // Reset player's position
                player.Pos = new Position(13, 17);
                // Reset all input, as to have a clean slate for the new level
                inputSys.ResetInput();
            }
        }
    }
}