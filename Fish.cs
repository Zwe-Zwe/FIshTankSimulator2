 using System;
 using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator{
    public abstract class Fish
{
    private Tank _tank;
    private Vector2 position ;
    private Vector2 speed ;
    private bool isMovingLeft ;
    private Health health;
    private Animator leftAnimator;
    private Animator rightAnimator;

    public Fish(Tank tank)
    {
        _tank = tank;
        position = new Vector2(0, 0);
        isMovingLeft = true;
    }

        protected Fish()
        {
        }

        public abstract void Update(float deltaTime, int windowWidth, int windowHeight);

    public abstract void Draw(float deltaTime);

    public Tank Tank
    {
        get { return _tank; }
        set { _tank = value; }
    }

    public List<Texture2D> LoadTextures(string folder)
        {
            List<Texture2D> textures = new();
            for (int i = 1; i <= 6; i++)
            {
                textures.Add(Raylib.LoadTexture($"{folder}/{i}.png"));
            }
            return textures;
        }
    public void UnloadTextures()
        {
            LeftAnimator.UnloadTextures();
            RightAnimator.UnloadTextures();
        }
    public Animator LeftAnimator
    {
        get { return leftAnimator; }
        set { leftAnimator = value; }
    }

    public Animator RightAnimator
    {
        get { return rightAnimator; }
        set { rightAnimator = value; }
    }
    
    public Health Health
    {
        get { return health; }
        set { health = value; }
    }
    public Vector2 Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }
    public bool IsMovingLeft
    {
        get { return isMovingLeft; }
        set { isMovingLeft = value; }
    }

}
 
 }