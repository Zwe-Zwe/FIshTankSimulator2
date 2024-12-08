using System;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Snail : Fish
    {
        private float bottomMargin; // Distance from the bottom of the tank
        private float collectionRange; // Range within which the snail can collect coins
        private float movementSpeed; // Horizontal movement speed
        private Player _player;

        public Snail(Tank tank, int windowHeight, float startPosition, Player player)
            : base(tank)
        {
            bottomMargin = 70f;
            collectionRange = 50f;
            _player = player;

            // Set position to the bottom of the tank
            Position = new Vector2(startPosition, windowHeight - bottomMargin); // Start at bottom-left
            IsMovingLeft = true; // Starts moving left by default

            // Initialize animators with textures and frame durations
            LeftAnimator = new Animator(LoadTextures("sprites/guppy/swim_to_left"), 0.1f); // Assuming assets are in this folder
            RightAnimator = new Animator(LoadTextures("sprites/guppy/swim_to_right"), 0.1f);
        }

        public override void Update(float deltaTime, int windowWidth, int windowHeight)
        {
            movementSpeed = 100f + (float)(28.57 * (_player.SnailLevel - 1));
            Speed = new Vector2(movementSpeed, 0); // Horizontal speed
            Coin closestCoin = null;
            float closestDistance = float.MaxValue;
            float movementThreshold = 5f; // Small threshold to prevent jittering

            // Find the nearest coin
            foreach (Coin coin in Tank.CoinList)
            {
                float distance = Vector2.Distance(coin.Position, Position);
                if (distance < closestDistance)
                {
                    closestCoin = coin;
                    closestDistance = distance;
                }
            }

            // Move toward the nearest coin, no matter the distance
            if (closestCoin != null)
            {
                float coinX = closestCoin.Position.X;

                // Check if the snail needs to move left or right, avoiding jitter near the coin
                if (Math.Abs(coinX - Position.X) > movementThreshold)
                {
                    if (coinX < Position.X) // Coin is to the left
                    {
                        Position = new Vector2(Position.X - Speed.X * deltaTime, Position.Y);
                        IsMovingLeft = true;
                    }
                    else if (coinX > Position.X) // Coin is to the right
                    {
                        Position = new Vector2(Position.X + Speed.X * deltaTime, Position.Y);
                        IsMovingLeft = false;
                    }
                }
            }
            else
            {
                // Default horizontal bouncing movement if no coins exist
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
            }

            // Collect coins if they are within range
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
            Coin closestCoin = null;
            float closestDistance = float.MaxValue;

            // Find the closest coin within the collection range
            foreach (Coin coin in Tank.CoinList)
            {
                float distance = Vector2.Distance(coin.Position, Position);
                if (distance <= collectionRange && distance < closestDistance)
                {
                    closestCoin = coin;
                    closestDistance = distance;
                }
            }

            // Collect the closest coin if it exists
            if (closestCoin != null)
            {
                Tank.CollectCoin(closestCoin);
            }
        }

    }
}
