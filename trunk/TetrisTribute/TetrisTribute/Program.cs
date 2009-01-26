using System;

namespace TetrisTribute
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GamePlay game = new GamePlay())
            {
                game.Run();
            }
        }
    }
}

