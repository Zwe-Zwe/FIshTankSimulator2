using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class ExoticFish : Fish
    {
        // Static cache to store textures, shared across all instances of ExoticFish
        private static readonly Dictionary<string, List<Texture2D>> TextureCache = new();

        private float appearanceTimer; // Time until the fish appears
        private float activeDuration; // How long the fish remains active
        private bool isLeaving; // Whether the fish is leaving the tank
        private bool hasDroppedTreasure; // Whether the fish has already dropped a treasure

        private Animator LeftAnimator; // Animator for leftward movement
        private Animator RightAnimator; // Animator for rightward movement

        // Event triggered when the fish drops a treasure
        public event Action<Treasure> OnTreasureDropped;

        public ExoticFish(Vector2 initialPosition, Tank tank) : base(tank)
        {
            Position = initialPosition;
            Speed = new Vector2(700, 100); // Exotic fish move slower and more gracefully

            // Load animators using cached textures
            LeftAnimator = new Animator(GetTextures("sprites/gold/swim_to_left"), 0.1f);
            RightAnimator = new Animator(GetTextures("sprites/gold/swim_to_right"), 0.1f);

            IsMovingLeft = Speed.X < 0;

            ResetAppearanceTimer();
            activeDuration = 5f; // Exotic fish stays for 5 seconds
            hasDroppedTreasure = false;
        }

        /// <summary>
        /// Retrieves textures from the cache or loads them if not already cached.
        /// </summary>
        private static List<Texture2D> GetTextures(string basePath)
        {
            if (!TextureCache.ContainsKey(basePath))
            {
                List<Texture2D> textures = LoadTextures(basePath);
                TextureCache[basePath] = textures;
            }
            return TextureCache[basePath];
        }       
        /// <summary>
        /// Resets the appearance timer to a random value and marks the fish as inactive.
        /// </summary>
        private void ResetAppearanceTimer()
        {
            appearanceTimer = Raylib.GetRandomValue(5, 10); // Random delay before appearing
            isLeaving = false;
        }

        public override void Update(float deltaTime)
        {
            // Handle appearance delay
            if (appearanceTimer > 0)
            {
                appearanceTimer -= deltaTime;
                return;
            }

            // Handle leaving logic
            if (isLeaving)
            {
                LeaveTank(deltaTime);
                return;
            }

            // Drop treasure at midpoint of the active duration
            if (!hasDroppedTreasure && activeDuration <= 3f)
            {
                DropTreasure();
            }

            // Regular movement logic
            Position += Speed * deltaTime;

            // Handle X-axis wall collisions
            const float collisionThreshold = 10f;

            if (Position.X <= collisionThreshold) // Left wall collision
            {
                Position = new Vector2(collisionThreshold, Position.Y);
                Speed = new Vector2(Math.Abs(Speed.X), Speed.Y);
                IsMovingLeft = false;
            }
            else if (Position.X + 100 >= Program.windowWidth - collisionThreshold) // Right wall collision
            {
                Position = new Vector2(Program.windowWidth - 100 - collisionThreshold, Position.Y);
                Speed = new Vector2(-Math.Abs(Speed.X), Speed.Y);
                IsMovingLeft = true;
            }

            // Handle Y-axis wall collisions
            if (Position.Y <= 0 || Position.Y + 100 >= Program.windowHeight)
            {
                Speed = new Vector2(Speed.X, -Speed.Y);
            }

            // Reduce active duration
            if (activeDuration > 0)
            {
                activeDuration -= deltaTime;
            }
            else if (!isLeaving)
            {
                isLeaving = true;
                SetLeaveDirection();
            }
        }

        /// <summary>
        /// Handles the logic for the fish leaving the tank.
        /// </summary>
        private void LeaveTank(float deltaTime)
        {
            Position += Speed * deltaTime;

            // Check if the fish has exited the screen
            if (Position.X < -100 || Position.X > Program.windowWidth + 100)
            {
                ResetAppearanceTimer();
                activeDuration = 20f;
                Position = GetRandomPosition(Program.windowWidth);
                hasDroppedTreasure = false;
            }

            // Ensure the direction is updated continuously while leaving
            IsMovingLeft = Speed.X < 0;
        }

        /// <summary>
        /// Sets the direction for the fish to leave the tank.
        /// </summary>
        private void SetLeaveDirection()
        {
            if (Position.X < Program.windowWidth / 2)
            {
                Speed = new Vector2(-Math.Abs(Speed.X), 0); // Maintain original speed magnitude
            }
            else
            {
                Speed = new Vector2(Math.Abs(Speed.X), 0); // Maintain original speed magnitude
            }

            IsMovingLeft = Speed.X < 0;
        }

        /// <summary>
        /// Generates a random starting position for the fish.
        /// </summary>
        private Vector2 GetRandomPosition(int windowHeight)
        {
            int y = Raylib.GetRandomValue(100, windowHeight - 100);
            return new Vector2(-100, y);
        }

        /// <summary>
        /// Drops a treasure at the fish's current position.
        /// </summary>
        private void DropTreasure()
        {
            if (hasDroppedTreasure) return;
            Treasure treasure = new Treasure(Position);
            OnTreasureDropped?.Invoke(treasure);
            hasDroppedTreasure = true;
        }

        public override void Draw(float deltaTime)
        {
            if (appearanceTimer > 0) return;

            float scale = 0.5f;

            Texture2D currentSprite = IsMovingLeft
                ? LeftAnimator.GetCurrentFrame(deltaTime)
                : RightAnimator.GetCurrentFrame(deltaTime);

            Rectangle srcRect = new Rectangle(0, 0, currentSprite.Width, currentSprite.Height);
            Rectangle destRect = new Rectangle(
                Position.X,
                Position.Y,
                currentSprite.Width * scale,
                currentSprite.Height * scale
            );

            Raylib.DrawTexturePro(currentSprite, srcRect, destRect, Vector2.Zero, 0.0f, Color.White);
        }
    }
}
