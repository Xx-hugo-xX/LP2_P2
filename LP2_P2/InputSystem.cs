using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LP2_P2
{
    /// <summary>
    /// Responsible for handlind the user's input during the game's GameLoop.
    /// </summary>
    public class InputSystem
    {
        // Declare public property of bool type with a private set, used to
        // indicate to the InputThread if the game IsRunning and, by extension,
        // if it should continue reading the user's input
        public bool IsRunning { get; private set; }
        // Declare public property of Direction type with a private set, used
        // to indicate to the GameLoop what direction the player selected
        // through the input
        public Direction Dir { get; private set; }
        // Declare public property of Direction type, used to indicate to the
        // game loop the last direction the player had selected
        public Direction LastDir { get; set; }
        // Declare private readonly variable of type
        // BlockingCollection<ConsoleKey> used to save the player's input, used 
        // to determine what direction the player chooses
        private readonly BlockingCollection<ConsoleKey> inputCol;
        /// <summary>
        /// InputSystem class constructor with no parameters, responsible for
        /// creating an instance of the class.
        /// </summary>
        public InputSystem()
        { 
            // Instantiate inputCol as a new BlockingCollection<ConsoleKey>
            inputCol = new BlockingCollection<ConsoleKey>();
            // Instantiate Dir as a new Direction
            Dir = new Direction();
            // Instantiate LastDir as a new Direction
            LastDir = new Direction();
            // Instantiate IsRunning as true
            IsRunning = true;
        }
        /// <summary>
        /// Sets IsRunning bool variable to false, effectively communicating to
        /// the ReadKeys method to no longer accept inputs from the plahyer and
        /// to the gameloop, which uses it as a check to run the loop itself, 
        /// that the input is no longer being fed to it and the game ended.
        /// </summary>
        public void CloseInputReading()
        {
            // Assign false value to IsRunning property
            IsRunning = false;
        }
        /// <summary>
        /// Sets Dir and LastDir to Direction.None values and takes everything
        /// saved in the inputCol BlockingCollection, effectively resetting any
        /// and all input given by the player up to that point.
        /// </summary>
        public void ResetInput()
        {
            // Assign Direction.None value to Dir
            Dir = Direction.None;
            // Assign Direction.None value to LastDir
            LastDir = Direction.None;
            // for cycle that takes all ConsoleKeys stored in inputCol
            for (int i = 0; i < inputCol.Count; i++) inputCol.Take();
        }
        /// <summary>
        /// Processes the ConsoleKeys given to the inputCol BlockingCollection
        /// and assigns a direction to Dir based on which key was pressed,
        /// assigning the LastDir to Dir if not ConsoleKey can be taken from
        /// inputCol.
        /// </summary>
        public void ProcessInput()
        {
            // if statement that checks if it is possible to take an element
            // from inputCol
            if (inputCol.TryTake(out ConsoleKey key))
            {
                // switch statement that assigns a Direction value to Dir
                // based on the ConsoleKey taken from inputCol
                switch (key)
                {
                    // In case key taken is W
                    case ConsoleKey.W:
                        // Assign Direction.Up to Dir
                        Dir = Direction.Up;
                        break;
                    // In case key taken is S
                    case ConsoleKey.S:
                        // Assign Direction.Down to Dir
                        Dir = Direction.Down;
                        break;
                    // In case key taken is A
                    case ConsoleKey.A:
                        // Assign Direction.Left to Dir
                        Dir = Direction.Left;
                        break;
                    // In case key taken is D
                    case ConsoleKey.D:
                        // Assign Direction.Right to Dir
                        Dir = Direction.Right;
                        break;
                }
            }
            // else statement that assigns LastDir's value to Dir if no
            // element can be taken from inputCol
            else Dir = LastDir;
        }
        /// <summary>
        /// Public method to be ran in it's own thread, constantly accepting
        /// input from the player and adding the ConsoleKey to the inputCol
        /// BlockingCollection, to be processed by the ProcessInput method.
        /// </summary>
        public void ReadKeys()
        {
            // Declare key variable of ConsoleKey type
            ConsoleKey key;

            // while loop that runs as long as the IsRunning property is true
            while(IsRunning)
            {
                // Assigns a ConsoleKey value, returned from 
                // Console.Readkey().Key to variable key
                key = Console.ReadKey(true).Key;
                // Adds key read to the inputCol BlockingCollection
                inputCol.Add(key);
            }
        }
    }
}
