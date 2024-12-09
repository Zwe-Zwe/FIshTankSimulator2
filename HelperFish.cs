using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class HelperFish : Fish
    {
        private static readonly Dictionary<string, List<Texture2D>> TextureCache = new();

        private float bottomMargin; // Distance from the bottom of the tank
        private float collectionRange; 
        private float movementSpeed; // Horizontal movement speed
        private Player _player;
        private int priorityOffset; // Unique offset for coin prioritization
        private Coin assignedCoin; // Coin currently being targeted

        public HelperFish(Tank tank, float startPosition, Player player, int helperId)
            : base(tank)
        {
            bottomMargin = 70f;
            collectionRange = 50f;
            _player = player;

            // Set position to the bottom of the tank
            Position = new Vector2(startPosition, Program.windowHeight - bottomMargin); // Start at bottom-left
            IsMovingLeft = true; // Starts moving left by default


            priorityOffset = helperId * 10;

            // Initialize animators with cached textures
            LeftAnimator = new Animator(GetCachedTextures("sprites/helper/swim_to_left"), 0.1f);
            RightAnimator = new Animator(GetCachedTextures("sprites/helper/swim_to_right"), 0.1f);
        }

        private static List<Texture2D> GetCachedTextures(string basePath)
        {
            if (!TextureCache.ContainsKey(basePath))
            {
                TextureCache[basePath] = LoadTextures(basePath);
            }

            return TextureCache[basePath];
        }

        public override void Update(float deltaTime)
        {
            movementSpeed = 100f + (float)(28.57 * (_player.HelperLevel - 1));
            Speed = new Vector2(movementSpeed, 0); // Horizontal speed

            // Find a coin to target
            UpdateTargetCoin();

            // Move toward the assigned coin, or bounce if no coins exist
            if (assignedCoin != null)
            {
                float coinX = assignedCoin.Position.X;
                float movementThreshold = 5f; // Small threshold to prevent jittering

                
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
                    if (Position.X >= Program.windowWidth) // Bounce off the right wall
                    {
                        IsMovingLeft = true;
                    }
                }
            }

            // Collect coins if they are within range
            CollectCoins();
        }

        private void UpdateTargetCoin()
        {
            Coin closestCoin = null;
            float closestDistance = float.MaxValue;

            foreach (Coin coin in Tank.CoinList)
            {
                float distanceToCoin = Vector2.Distance(coin.Position, Position);

                // Add the priority offset to the distance calculation
                float adjustedDistance = distanceToCoin + priorityOffset;

                // Check if this coin is the closest based on adjusted distance
                if (adjustedDistance < closestDistance && !IsCoinTargetedByOtherHelper(coin))
                {
                    closestCoin = coin;
                    closestDistance = adjustedDistance;
                }
            }

            assignedCoin = closestCoin;
        }

        private bool IsCoinTargetedByOtherHelper(Coin coin)
        {
            foreach (HelperFish otherHelper in Tank.HelperList)
            {
                if (otherHelper == this) continue; // Skip self

                if (otherHelper.assignedCoin == coin)
                {
                    return true;
                }
            }

            return false;
        }

        private void CollectCoins()
        {
            if (assignedCoin != null && Vector2.Distance(assignedCoin.Position, Position) <= collectionRange)
            {
                Tank.CollectCoin(assignedCoin);
                assignedCoin = null; // Clear the assignment after collection
            }
        }

        public override void Draw(float deltaTime)
        {
            // Get the current frame based on the direction of movement
            Texture2D currentFrame = IsMovingLeft
                ? LeftAnimator.GetCurrentFrame(deltaTime)
                : RightAnimator.GetCurrentFrame(deltaTime);

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
    }
}
