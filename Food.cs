using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Food : DropItems
    {
        public Food(Vector2 position)
            : base(position, "sprites/fishfood.png", 100f) // Pass position, texture path, and fall speed to the parent class
        {
        }

        public override void OnPickup()
        {
            IsActive = false; // Mark the food as consumed
            // Additional logic, such as increasing fish health, can be implemented here.
        }

        // Override the GetValue method to return the value of the food item
        public override int GetValue()
        {
            return 50; // Example value for food item
        }
    }
}
