using Raylib_cs;

namespace FishTankSimulator
{
    public class Program
    {
        public static int windowWidth = 2560;
        public static int windowHeight = 1440;
        public static void Main()
        {
            // Create and run the game
            Game game = new Game(windowWidth, windowHeight);
            game.Run(windowWidth);
        }
    }
}
