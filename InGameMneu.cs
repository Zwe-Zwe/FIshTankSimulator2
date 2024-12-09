using Raylib_cs;
using System;
using System.Numerics;

namespace FishTankSimulator
{
    public class InGameMenu
    {
        private Rectangle[] buttonBounds;
        private string[] buttonLabels = { "Resume", "Quit" }; // Removed "Save"
        private Texture2D buttonBg;
        private Font customFont; // Declare a font variable

        public bool isMenuVisible;

        public InGameMenu()
        {
            isMenuVisible = false;

            // Load button background sprite
            buttonBg = Raylib.LoadTexture("sprites/button.png");

            // Load custom font
            customFont = Raylib.LoadFont("font/font.ttf"); // Make sure to use the correct path to the font file

            // Define button positions and sizes
            buttonBounds = new Rectangle[buttonLabels.Length]; // Adjusted length for two buttons
            for (int i = 0; i < buttonBounds.Length; i++)
            {
                float buttonWidth = 200; // Adjust to your sprite's width
                float buttonHeight = 50; // Adjust to your sprite's height
                buttonBounds[i] = new Rectangle(
                    Program.windowWidth / 2 - buttonWidth / 2,
                    Program.windowWidth / 2 - (buttonHeight * buttonBounds.Length) / 2 + i * (buttonHeight + 10),
                    buttonWidth,
                    buttonHeight
                );
            }
        }

        public void Update()
        {
            if (!isMenuVisible) return;

            for (int i = 0; i < buttonBounds.Length; i++)
            {
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), buttonBounds[i]))
                {
                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        HandleButtonClick(i); // Handle the button click directly
                    }
                }
            }
        }

        private void HandleButtonClick(int buttonIndex)
        {
            switch (buttonIndex)
            {
                case 0: // Resume
                    Console.WriteLine("Resume clicked");
                    isMenuVisible = false; // Hide the menu
                    break;
                case 1: // Quit
                    Console.WriteLine("Quit clicked");
                    QuitGame();
                    break;
            }
        }

        private void QuitGame()
        {
            Console.WriteLine("Quit button clicked!");
            isMenuVisible = false; // Hide the menu
            Game.IsQuitClicked = true;  // Signal that the Quit button was clicked
        }

        public void Draw()
        {
            if (!isMenuVisible) return;

            for (int i = 0; i < buttonBounds.Length; i++)
            {
                // Check if the button is hovered
                bool isHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), buttonBounds[i]);

                // Set frame color based on hover state
                Color frameColor = isHovered ? Color.Gray : Color.White;

                // Draw the button background (texture)
                Raylib.DrawTexturePro(
                    buttonBg,
                    new Rectangle(0, 0, buttonBg.Width, buttonBg.Height),
                    buttonBounds[i],
                    new Vector2(0, 0),
                    0,
                    frameColor // Apply the frame color based on hover state
                );

                // Draw the button label using the custom font
                string label = buttonLabels[i];
                int textWidth = (int)Raylib.MeasureTextEx(customFont, label, 20, 0).X; // Measure text width using custom font
                int textHeight = (int)Raylib.MeasureTextEx(customFont, label, 20, 0).Y; // Measure text height using custom font
                Raylib.DrawTextEx(
                    customFont, // Use the custom font
                    label,
                    new Vector2(
                        buttonBounds[i].X + buttonBounds[i].Width / 2 - textWidth / 2,
                        buttonBounds[i].Y + buttonBounds[i].Height / 2 - textHeight / 2
                    ),
                    20, // Font size
                    0, // Spacing
                    Color.White // Ensure the text color is black
                );
            }
        }


        public bool IsMenuVisible
        {
            get { return isMenuVisible; }
            set { isMenuVisible = value; }
        }

        public void Unload()
        {
            Raylib.UnloadTexture(buttonBg);
            Raylib.UnloadFont(customFont); // Unload the custom font
        }
    }
}
