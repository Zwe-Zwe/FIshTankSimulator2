using Raylib_cs;

namespace FishTankSimulator
{
    public class Program
    {
        public static void Main()
        {
            int windowWidth = 2560;
            int windowHeight = 1440;

            // Create and run the game
            Game game = new Game(windowWidth, windowHeight);
            game.Run(windowWidth);
        }
    }
}
