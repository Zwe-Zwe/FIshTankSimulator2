using System;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Game
    {
        private Player player;
        private LevelManagement levelManagement;
        private Menu _menu;
        private Shop _shop;
        private Tank _tank;
        private Weapon _weapon;
        private Upgrade upgrade;
        private Texture2D _tankBackground;
        private Rectangle _srcRect;
        private Rectangle _destRect;
        private Vector2 _origin;
        private bool _inGame = false;
        private bool _clickConsumed = false;
        public static bool isQuitClicked = false;
        public Game()
        {
            Raylib.InitWindow(Program.windowWidth, Program.windowHeight, "Fish Tank Simulator");
            Raylib.SetTargetFPS(60);

            player = new Player();
            _weapon = new Weapon();
            _shop = new Shop(_weapon, player);
            levelManagement = new LevelManagement(player, _shop);
            _menu = new Menu();
            upgrade = new Upgrade(levelManagement, player);
            _tank = new Tank(_shop, player, upgrade);
            _tankBackground = Raylib.LoadTexture("sprites/tank.png");
            if (_tankBackground.Id == 0)
            {
                Console.WriteLine("Error loading tank background texture!");
            }
            _srcRect = new Rectangle(0, 0, _tankBackground.Width, _tankBackground.Height);
            _destRect = new Rectangle(0, 0, Program.windowWidth, Program.windowHeight);
            _origin = new Vector2(0, 0);
        }

        public static bool IsQuitClicked{
            get { return isQuitClicked; }
            set { isQuitClicked = value; }
        }
       
        public void Run()
        {
            while (!Raylib.WindowShouldClose() && !isQuitClicked)
            {
                if (!_inGame)
                {
                    HandleMenu();
                }
                else
                {
                    HandleGame();
                }
            }

            UnloadResources();
        }

        private void HandleMenu()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);
            _menu.Draw();
            Raylib.EndDrawing();

            if (_menu.IsStartButtonClicked())
            {
                _inGame = true;
                _clickConsumed = true;
            }
            else if (_menu.IsQuitButtonClicked())
            {
                Raylib.CloseWindow();
            }
        }

        // In Game.HandleGame()
        private void HandleGame()
        {
            float deltaTime = Raylib.GetFrameTime();

            // Pass clickConsumed to ensure clicks are not processed multiple times
            _tank.Update(deltaTime, _clickConsumed);
            _clickConsumed = false; // Reset the clickConsumed flag for the next frame

            int? clickedFishIndex = _shop.GetClickedFishIndex();
            if (clickedFishIndex != null)
            {
                int fishIndex = clickedFishIndex.Value;
                if (_tank.TryBuyFish(fishIndex))
                {
                    Console.WriteLine($"Fish bought: {_shop.GetFishName(fishIndex)}");
                    _clickConsumed = true; // Mark the click as consumed for buying fish
                }
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            // Draw the tank background
            Raylib.DrawTexturePro(_tankBackground, _srcRect, _destRect, _origin, 0.0f, Color.White);

            _shop.Draw();
            _tank.Draw(deltaTime);

            Raylib.EndDrawing();
        }



        private void UnloadResources()
        {
            _menu.UnloadTextures();
            _shop.UnloadTextures();
            _tank.UnloadAllTextures();
            Raylib.UnloadTexture(_tankBackground);
            Raylib.CloseWindow();
        }
    }
}
