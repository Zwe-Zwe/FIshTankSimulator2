using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator{
    public class Health
{
    private float current;
    private float max;

    public Health(float maxHealth)
    {
        max = maxHealth;
        current = maxHealth;
    }
    public float Current{
        get { return current; }
        set { current = value; }
    }

    public float Max{
        get { return max; }
        set { max = value; }
    }

    public void Reduce(float amount) => current = Math.Max(0, current - amount);
    public void Increase(float amount) => current = Math.Min(max, current + amount);

    public void Draw(Vector2 position, float width, float height)
    {
        float percentage = current / max;

        Raylib.DrawRectangle((int)position.X, (int)position.Y, (int)width, (int)height, Color.Gray);
        Raylib.DrawRectangle((int)position.X, (int)position.Y, (int)(width * percentage), (int)height, Color.Green);
        Raylib.DrawRectangleLines((int)position.X, (int)position.Y, (int)width, (int)height, Color.Black);
    }

      
}

}