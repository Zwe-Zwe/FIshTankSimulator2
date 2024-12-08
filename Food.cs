using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Food : DropItems
    {
        private Player _player;
        private int nutrientValue;
        public Food(Vector2 position, Player player)
            : base(position, "sprites/fishfood.png", 100f) // Pass position, texture path, and fall speed to the parent class
        {
            _player = player;
        }

        public override void OnPickup()
        {
            IsActive = false; // Mark the food as consumed
            // Additional logic, such as increasing fish health, can be implemented here.
        }

        public override void Update(float deltaTime, int windowHeight)
        {
            
            nutrientValue = 40 + ((_player.FoodLevel - 1) * 5); // Calculate nutrient value based on food level
            
            if (!IsActive) return;

            Position = new Vector2(Position.X, Position.Y + fallSpeed * deltaTime);

            if (Position.Y >= windowHeight - texture.Height)
            {
                IsActive = false;
            }
        }

        // Override the GetValue method to return the value of the food item
        public override int GetValue()
        {
            return nutrientValue; // Example value for food item
        }
    }
}
