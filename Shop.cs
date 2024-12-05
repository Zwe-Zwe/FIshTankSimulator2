using System.Collections.Generic;
using Raylib_cs;
using System.Numerics;

namespace FishTankSimulator
{
    public class Shop
    {
        private List<Texture2D> fishIcons; // Icons for fish in the shop
        private List<Rectangle> fishIconRects; // Icon positioning
        private List<int> fishCosts; // List to store the cost of each fish
        private int iconSize = 70; // Size of each fish icon in pixels
        private int padding = 10; // Padding between icons
        private int yPosition = 10; // Vertical position for the grid
        private int goldAmount; // Player's current gold
        private Texture2D frameTexture; // Frame texture for the fish icon
        private Weapon weapon; // Weapon instance
        private Texture2D weaponIcon; // Icon for the weapon
        private Rectangle weaponFrameRect; // Frame position for the weapon
        private Font customFont = Raylib.LoadFont("font/font.ttf");

        // Fish types
        private List<string> fishNames;

        public Shop(int startingGold, Weapon weaponInstance)
        {
            goldAmount = startingGold;
            weapon = weaponInstance;

            // Load icons for fish and snail
            fishIcons = new List<Texture2D>
            {
                Raylib.LoadTexture("sprites/guppy/swim_to_left/1.png"), // Icon for Guppy
                Raylib.LoadTexture("sprites/blue/swim_to_left/1.png"), // Icon for Snapper
                Raylib.LoadTexture("sprites/red/swim_to_left/1.png"), // Icon for Flounder
                Raylib.LoadTexture("sprites/shark/swim_to_left/1.png") // Icon for Snail
            };

            // Load frame texture
            frameTexture = Raylib.LoadTexture("sprites/frame.png");

            // Define costs and names for fish and snail
            fishCosts = new List<int> { 100, 150, 200, 300 }; // Last cost is for Snail
            fishNames = new List<string> { "Guppy", "Snapper", "Flounder", "Snail" }; // Last name is Snail

            // Define positions for the icons
            fishIconRects = new List<Rectangle>();
            int frameSize = iconSize + 20; // Frame is slightly larger than the icon
            int totalPadding = padding + frameSize; // Spacing considers frame size and padding

            for (int i = 0; i < fishIcons.Count; i++)
            {
                fishIconRects.Add(new Rectangle(
                    10 + (totalPadding) * i, // Adjust X position for consistent spacing
                    yPosition,
                    frameSize, // Frame width
                    frameSize  // Frame height
                ));
            }

            // Load weapon icon and define its frame inline with fish/snail icons
            weaponIcon = Raylib.LoadTexture("sprites/harpoon.png");
            int weaponFrameX = 10 + (totalPadding) * fishIcons.Count; // Positioned after the last fish/snail icon
            weaponFrameRect = new Rectangle(
                weaponFrameX,
                yPosition,
                frameSize,
                frameSize
            );
        }
        public Rectangle WeaponFrameRect
        {
            get { return weaponFrameRect; }
        }
        public List<Texture2D> FishIcons
        {
            get { return fishIcons; }
        }

        public void Draw()
        {
            float scale = 0.8f; // Scale factor (reduce to 80% of original size)

            // Draw fish frames
            for (int i = 0; i < fishIcons.Count; i++)
            {
                Rectangle frameRect = fishIconRects[i];
                bool isHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), frameRect);
                Color frameColor = isHovered ? Color.Gray : Color.White;

                // Draw frame and fish icon
                DrawFrame(frameRect, frameColor, fishIcons[i], scale);

                // Draw fish cost below frame
                string costText = $"${fishCosts[i]}";
                Vector2 costTextPosition = new Vector2(
                    frameRect.X + (frameRect.Width / 2) - (Raylib.MeasureText(costText, 25) / 2),
                    frameRect.Y + frameRect.Height + 5
                );
                Raylib.DrawTextEx(customFont, costText, costTextPosition, 25, 2, Color.Gold);

                // Highlight frame on hover
                if (isHovered)
                {
                    Raylib.DrawRectangleLinesEx(frameRect, 2, Color.Gold);
                }
            }

            // Draw weapon frame
            bool isWeaponHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), weaponFrameRect);
            Color weaponFrameColor = isWeaponHovered ? Color.Gray : (weapon.IsActived ? Color.Green : Color.White);
            DrawFrame(weaponFrameRect, weaponFrameColor, weaponIcon, scale);

            // Draw weapon label below frame
            string weaponText = "Weapon";
            Vector2 weaponTextPosition = new Vector2(
                weaponFrameRect.X + (weaponFrameRect.Width / 2) - (Raylib.MeasureText(weaponText, 25) / 2),
                weaponFrameRect.Y + weaponFrameRect.Height + 5
            );
            Raylib.DrawTextEx(customFont, weaponText, weaponTextPosition, 25, 2, Color.White);

            // Display activation status
            string activationStatus = weapon.IsActived ? "Active" : "Inactive";
            Color statusColor = weapon.IsActived ? Color.Green : Color.Red;
            Vector2 statusTextPosition = new Vector2(
                weaponFrameRect.X + (weaponFrameRect.Width / 2) - (Raylib.MeasureText(activationStatus, 20) / 2),
                weaponTextPosition.Y + 30 // Adjust position below the weapon label
            );
            Raylib.DrawTextEx(customFont, activationStatus, statusTextPosition, 20, 2, statusColor);

            // Highlight weapon frame on hover
            if (isWeaponHovered)
            {
                Raylib.DrawRectangleLinesEx(weaponFrameRect, 2, Color.Gold);
            }
        }

        private void DrawFrame(Rectangle frameRect, Color frameColor, Texture2D icon, float scale)
        {
            // Draw frame
            Raylib.DrawTexturePro(
                frameTexture,
                new Rectangle(0, 0, frameTexture.Width, frameTexture.Height),
                frameRect,
                Vector2.Zero,
                0f,
                frameColor
            );

            // Scale and center the icon within the frame
            int scaledWidth = (int)(iconSize * scale);
            int scaledHeight = (int)(iconSize * scale);
            Rectangle iconRect = new Rectangle(
                frameRect.X + (frameRect.Width - scaledWidth) / 2,
                frameRect.Y + (frameRect.Height - scaledHeight) / 2,
                scaledWidth,
                scaledHeight
            );

            Raylib.DrawTexturePro(icon, new Rectangle(0, 0, icon.Width, icon.Height), iconRect, Vector2.Zero, 0f, Color.White);
        }

       
        public void HandleClick(bool clickConsumed)
        {
            // Exit early if the click has already been consumed
            if (clickConsumed)
                return;

            // Check if the weapon frame is clicked first
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), weaponFrameRect))
            {
                // Toggle weapon activation
                Console.WriteLine(weapon.IsActived ? "Weapon Activated" : "Weapon Deactivated");

                // Mark the click as consumed
                clickConsumed = true;
                return;
            }

            // If the weapon frame was not clicked, check the fish/snail frames
            for (int i = 0; i < fishIconRects.Count; i++)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                    Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), fishIconRects[i]))
                {
                    if (i == fishIcons.Count - 1) // Last index is for the Snail
                    {
                        TryBuySnail();
                    }
                    else
                    {
                        // Attempt to buy the fish
                        TryBuyFish(i);
                    }

                    // Mark the click as consumed and exit
                    clickConsumed = true;
                    return;
                }
            }
        }

        // Method to attempt to buy a fish
        public bool TryBuyFish(int fishIndex)
        {
            if (fishIndex >= 0 && fishIndex < fishCosts.Count)
            {
                int fishCost = fishCosts[fishIndex];
                if (goldAmount >= fishCost)
                {
                    // Deduct the gold and return true to indicate successful purchase
                    goldAmount -= fishCost;
                    return true;
                }
            }
            return false;
        }

        public bool TryBuySnail()
        {
            int snailCost = fishCosts[^1]; // Last cost in the list is for Snail
            if (goldAmount >= snailCost)
            {
                goldAmount -= snailCost;
                Console.WriteLine("Snail purchased!");
                return true;
            }
            Console.WriteLine("Not enough gold to buy Snail!");
            return false;
        }

        // Get the index of the clicked fish icon
        public int? GetClickedFishIndex()
        {
            for (int i = 0; i < fishIconRects.Count; i++)
            {
                Rectangle frameRect = fishIconRects[i];

                // Check if the mouse clicked within a fish frame
                if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                    Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), frameRect))
                {
                    return i; // Return the index of the clicked fish
                }
            }

            // Exclude the weapon frame from this method
            return null; // No fish frame was clicked
        }



        public void UnloadTextures()
        {
            foreach (var texture in fishIcons)
            {
                Raylib.UnloadTexture(texture);
            }
            Raylib.UnloadTexture(frameTexture); // Unload frame texture
            Raylib.UnloadTexture(weaponIcon); // Unload weapon icon
        }

        // Method to get the cost of a fish at a specific index
        public int GetFishCost(int fishIndex)
        {
            if (fishIndex >= 0 && fishIndex < fishCosts.Count)
            {
                return fishCosts[fishIndex];
            }
            return 0;
        }

        // Optional: You can also create a method to get the name of a fish by index
        public string GetFishName(int fishIndex)
        {
            if (fishIndex >= 0 && fishIndex < fishNames.Count)
            {
                return fishNames[fishIndex];
            }
            return string.Empty;
        }

        public int GoldAmount
        {
            get { return goldAmount; }
            set { goldAmount = value; }
        }

        public bool IsWeaponActivated()
        {
             return weapon.IsActived; 
        }

        public void ActivateWeapon(){
            weapon.IsActived = !weapon.IsActived;
        }

        public Weapon Weapon
        {
            get { return weapon; }
        }
    }
}
