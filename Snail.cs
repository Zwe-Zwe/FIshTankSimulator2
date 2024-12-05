using System;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Snail : Fish
    {
        private float bottomMargin; // Distance from the bottom of the tank
        private float collectionRange; // Range within which the snail can collect coins

        public Snail(Tank tank)
            : base(tank)
        {
            bottomMargin = 70f;
            collectionRange = 50f;

            // Set position to the bottom of the tank
            Position = new Vector2(720, 1080 - bottomMargin); // Start at bottom-left
            Speed = new Vector2(50, 0); // Horizontal speed
            IsMovingLeft = true; // Starts moving left by default

            // Initialize animators with textures and frame durations
            LeftAnimator = new Animator(LoadTextures("sprites/guppy/swim_to_left"), 0.1f); // Assuming assets are in this folder
            RightAnimator = new Animator(LoadTextures("sprites/guppy/swim_to_right"), 0.1f);
        }

        public override void Update(float deltaTime, int windowWidth, int windowHeight)
        {
            // Handle horizontal movement
            if (IsMovingLeft)
            {
                Position = new Vector2(Position.X - Speed.X * deltaTime, Position.Y);
                if (Position.X <= 0) // Bounce off the left wall
                {
                    IsMovingLeft = false;
                }
            }
            else
            {
                Position = new Vector2(Position.X + Speed.X * deltaTime, Position.Y);
                if (Position.X >= windowWidth) // Bounce off the right wall
                {
                    IsMovingLeft = true;
                }
            }

            // Collect coins at the bottom
            CollectCoins();
        }

        public override void Draw(float deltaTime)
        {
            // Get the current frame based on the direction of movement
            Texture2D currentFrame = IsMovingLeft
                ? LeftAnimator.GetCurrentFrame(deltaTime)
                : RightAnimator.GetCurrentFrame(deltaTime);

            // Scale factor to make the snail smaller (e.g., 50% smaller)
            float scale = 0.3f;

            // Draw the scaled-down texture
            Raylib.DrawTextureEx(
                currentFrame, 
                Position, 
                0f, // Rotation (0 degrees for no rotation)
                scale, // Scale factor
                Raylib_cs.Color.White // Tint (white means no tint)
            );
        }


        private void CollectCoins()
        {
            // Check and collect coins near the snail's position
            foreach (Coin coin in Tank.CoinList)
            {
                if (Vector2.Distance(coin.Position, Position) <= collectionRange)
                {
                    Tank.CollectCoin(coin); // Notify the tank to handle coin collection
                }
            }
        }
    }
}
