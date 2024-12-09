using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Food : DropItems
    {
        private static Texture2D sharedTexture;
        private Player _player;
        private int nutrientValue;
        public Food(Vector2 position, Player player)
            : base(position, 100f) // Pass position, texture path, and fall speed to the parent class
        {
            _player = player;
            if (sharedTexture.Id == 0)
            {
                sharedTexture = Raylib.LoadTexture("sprites/fishfood.png");
            }

            texture = sharedTexture;
        }

        public override void Update(float deltaTime, int windowHeight)
        {
            
            nutrientValue = 40 + ((_player.FoodLevel - 1) * 5); // Calculate nutrient value based on food level
            
            if (!isActive) return;

            position = new Vector2(position.X, position.Y + fallSpeed * deltaTime);

            if (position.Y >= windowHeight - texture.Height)
            {
                isActive = false;
            }
        }

        public void Draw()
        {
            if (isActive)
            {
                Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
                float scaleFactor = 0.5f;
                Rectangle destRect = new Rectangle(
                    position.X,
                    position.Y,
                    texture.Width * scaleFactor,
                    texture.Height * scaleFactor
                );

                Raylib.DrawTexturePro(
                    texture,
                    sourceRect,
                    destRect,
                    Vector2.Zero,
                    0.0f,
                    Color.White
                );
            }
        }

        // Override the GetValue method to return the value of the food item
        public override int GetValue()
        {
            return nutrientValue; // Example value for food item
        }
    }
}
