using System;

namespace LP2_P2
{
    /// <summary>
    /// Responsible for running the program.
    /// </summary>
    public class Program
    {
        // Declares and initializes a constant variable of type int used as the
        // window's width
        private const int WindowWidth = 50;
        // Declares and initializes a constant variable of type int used as the
        // window's height
        private const int WindowHeight = 50;
        /// <summary>
        /// Runs program and it's methods.
        /// </summary>
        private static void Main()
        {
            // Set window's width and height to the values specified by the
            // class constant variables WindowWidth and WindowHeight
            Console.WindowWidth = WindowWidth * 2;
            Console.WindowHeight = WindowHeight;

            // Declare and instantiate the mainMenu variable of type MainMenu
            MainMenu mainMenu = new MainMenu();
            // Run main menu
            mainMenu.RunMenu();
        }
    }
}