using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator{
    public class Health
{
    public float Current { get; private set; }
    public float Max { get; private set; }

    public Health(float maxHealth)
    {
        Max = maxHealth;
        Current = maxHealth;
    }

    public void Reduce(float amount) => Current = Math.Max(0, Current - amount);
    public void Increase(float amount) => Current = Math.Min(Max, Current + amount);

    public void Draw(Vector2 position, float width, float height)
    {
        float percentage = Current / Max;

        Raylib.DrawRectangle((int)position.X, (int)position.Y, (int)width, (int)height, Color.Gray);
        Raylib.DrawRectangle((int)position.X, (int)position.Y, (int)(width * percentage), (int)height, Color.Green);
        Raylib.DrawRectangleLines((int)position.X, (int)position.Y, (int)width, (int)height, Color.Black);
    }

      
}

}