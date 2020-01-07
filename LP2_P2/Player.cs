using System;

namespace LP2_P2
{
    /// <summary>
    /// Creates a Player to be used by the user
    /// </summary>
    public class Player : Object
    {
        // A variable to store a Score class
        public Score plyrScore;
        /// <summary>
        /// Constructor of the Player class
        /// </summary>
        /// <param name="x"> The starting X position </param>
        /// <param name="y"> The starting Y position </param>
        public Player()
        {
            // Assigns the Pos from the class it inherits from a new position
            Pos = new Position(13, 18);
            // Assigns the OldPos from the class
            // it inherits from a new position
            OldPos = new Position(1, 1);
            // Gives it a visual for when it's to be displayed
            Visuals = 'C';
            // Creates a square collider for the object
            BoxCollider = new int[4] { 0, 0, 0 + 1, 0 + 1 };
            // Sets the value to a new Score class
            plyrScore = new Score();
        }

        /// <summary>
        /// Displays "Death Message" and runs the 'AddHighScore'
        /// method of the 'HighScoreManager' class
        /// </summary>
        /// <param name="inputSys">Game's InputSystem that will be
        /// closed once the method is called</param>
        /// <param name="HSManager">Game's HighScoreManager that will be used
        /// to add the players score to the HighScores file</param>
        public void Death(InputSystem inputSys, HighScoreManager HSManager)
        {
            // Calls method 'CloseInputReading' from the 'InputSystem' class
            inputSys.CloseInputReading();

            // Clears the console
            Console.Clear();
            // Writes the "GameOver Message"
            Console.WriteLine("Game Over!\n\nPress any key to continue...");
            // Asks for user input before continuing
            Console.ReadKey(true);
            
            // Calls method 'AddHighScore' from the 'HighScoreManager'
            // class to add the players score to the HighScores
            // (if it is a High Score)
            HSManager.AddHighScore(plyrScore);
        }
    }
}
