using System;

namespace LP2_P2
{
    public class Program
    {
        private const int WindowWidth = 50;
        private const int WindowHeight = 50;
        static void Main()
        {
            Console.WindowWidth = WindowWidth * 2;
            Console.WindowHeight = WindowHeight;

            _ = new MainMenu();
        }
    }
}