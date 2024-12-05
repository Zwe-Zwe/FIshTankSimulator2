using System;
using System.Collections.Generic;
using Raylib_cs;

namespace FishTankSimulator{
    public class Animator
{
    private List<Texture2D> frames;
    private int currentFrame = 0;
    private float frameTime;
    private float timer = 0;

    public Animator(List<Texture2D> textures, float frameDuration)
    {
        frames = textures;
        frameTime = frameDuration;
    }

    public Texture2D GetCurrentFrame(float deltaTime)
    {
        timer += deltaTime;
        if (timer >= frameTime)
        {
            timer = 0;
            currentFrame = (currentFrame + 1) % frames.Count;
        }
        return frames[currentFrame];
    }

    public void UnloadTextures()
    {
        foreach (var texture in frames)
        {
            Raylib.UnloadTexture(texture);
        }
    }
}


}