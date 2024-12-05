using Raylib_cs;
using System.Numerics;

namespace FishTankSimulator
{
    public class Menu
    {
        private Texture2D startButton;
        private Texture2D quitButton;
        private Texture2D background;

        private Rectangle startButtonRect;
        private Rectangle quitButtonRect;

        private Color startButtonColor = Color.White;
        private Color quitButtonColor = Color.White;

        private float buttonScale = 0.7f; // Scale for smaller buttons

        public Menu()
        {
            background = Raylib.LoadTexture("sprites/Menu_background.png");
            if (background.Id == 0)
            {
                Console.WriteLine("Error loading Menu background texture!");
            }
            startButton = Raylib.LoadTexture("sprites/Menu_start.png");
            quitButton = Raylib.LoadTexture("sprites/Menu_quit.png");
             if (startButton.Id == 0 || quitButton.Id == 0)
                {
                    Console.WriteLine("Error loading buttons!");
                }

            int buttonWidth = (int)(startButton.Width * buttonScale);
            int buttonHeight = (int)(startButton.Height * buttonScale);

            // Center buttons horizontally and space them vertically
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();

            startButtonRect = new Rectangle(
                screenWidth / 2 - buttonWidth / 2,
                screenHeight / 2 - buttonHeight - 20,
                buttonWidth,
                buttonHeight
            );

            quitButtonRect = new Rectangle(
                screenWidth / 2 - buttonWidth / 2,
                screenHeight / 2 + 20,
                buttonWidth,
                buttonHeight
            );
        }

        public void Draw()
        {
            // Draw scaled background to fit the window
            Rectangle srcRect = new Rectangle(0, 0, background.Width, background.Height);
            Rectangle destRect = new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            Raylib.DrawTexturePro(background, srcRect, destRect, new Vector2(0, 0), 0.0f, Color.White);

            // Draw buttons with hover effects
            Raylib.DrawTexturePro(
                startButton,
                new Rectangle(0, 0, startButton.Width, startButton.Height),
                startButtonRect,
                new Vector2(0, 0),
                0.0f,
                startButtonColor
            );

            Raylib.DrawTexturePro(
                quitButton,
                new Rectangle(0, 0, quitButton.Width, quitButton.Height),
                quitButtonRect,
                new Vector2(0, 0),
                0.0f,
                quitButtonColor
            );

            UpdateButtonColors(); // Update hover colors
        }

        public bool IsStartButtonClicked()
        {
            return Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                   Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), startButtonRect);
        }

        public bool IsQuitButtonClicked()
        {
            return Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                   Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), quitButtonRect);
        }

        public void UnloadTextures()
        {
            Raylib.UnloadTexture(background);
            Raylib.UnloadTexture(startButton);
            Raylib.UnloadTexture(quitButton);
        }

        private void UpdateButtonColors()
        {
            // Check hover for Start button
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), startButtonRect))
            {
                startButtonColor = Color.Gray; // Change color on hover
            }
            else
            {
                startButtonColor = Color.White;
            }

            // Check hover for Quit button
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), quitButtonRect))
            {
                quitButtonColor = Color.Gray; // Change color on hover
            }
            else
            {
                quitButtonColor = Color.White;
            }
        }
    }
}
