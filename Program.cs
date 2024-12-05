using Raylib_cs;

namespace FishTankSimulator
{
    public class Program
    {
        public static void Main()
        {
            int windowWidth = 1920;
            int windowHeight = 1080;

            // Create and run the game
            Game game = new Game(windowWidth, windowHeight);
            game.Run();
        }
    }
}
