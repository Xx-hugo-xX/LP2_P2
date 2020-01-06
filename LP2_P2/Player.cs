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
            // Assgins the Pos from the class it inherits from a new position
            Pos = new Position(13, 18);
            // Assgins the OldPos from the class it inherits from a new position
            OldPos = new Position(1, 1);
            // Gives it a visual for when it's to be displayed
            Visuals = 'C';
            // Creates a square collider for the object
            BoxCollider = new int[4] { 0, 0, 0 + 1, 0 + 1 };
            // Sets the value to a new Score class
            plyrScore = new Score();
        }

        public void Death(InputSystem inputSys, HighScoreManager HSManager)
        {
            inputSys.CloseInputReading();

            Console.Clear();
            Console.WriteLine("Game Over!");
            Console.ReadKey(true);

            HSManager.AddHighScore(plyrScore);
        }
    }
}
