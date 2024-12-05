using System;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Treasure : DropItems
    {
        public TreasureType Type { get; private set; }
        public int Value { get; private set; }

        public Treasure(Vector2 position, TreasureType type)
            : base(position, "sprites/treasure.png", 50f)
        {
            Type = type;
            Value = GetTreasureValue(type);
        }

        public override void OnPickup()
        {
            IsActive = false;
            Console.WriteLine($"Treasure collected! Type: {Type}, Value: {Value}");
        }

        public override int GetValue()
        {
            return Value;
        }

        private int GetTreasureValue(TreasureType type)
        {
            return type switch
            {
                TreasureType.Diamond => 200,
                TreasureType.Ruby => 150,
                TreasureType.Jade => 100,
                _ => 0
            };
        }

        // New IsClicked method to check if the treasure was clicked
        public bool IsClicked(Vector2 mousePosition)
        {
            // Assuming the texture's width and height can be used to determine the treasure's size
            Rectangle treasureRect = new Rectangle(Position.X, Position.Y, texture.Width, texture.Height);
            return Raylib.CheckCollisionPointRec(mousePosition, treasureRect);
        }
    }
}
