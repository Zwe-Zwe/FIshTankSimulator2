using System;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class ExoticFish : Fish
    {
        private float appearanceTimer;
        private float activeDuration;
        private bool isLeaving;
        private bool hasDroppedTreasure;
        private Animator LeftAnimator;
        private Animator RightAnimator;

        public event Action<Treasure> OnTreasureDropped;

        public ExoticFish(Vector2 initialPosition, Tank tank) : base(tank)
        {
            Position = initialPosition;
            Speed = new Vector2(700, 100); // Exotic fish move slower and more gracefully
            LeftAnimator = new Animator(LoadTextures("sprites/red/swim_to_left"), 0.1f);
            RightAnimator = new Animator(LoadTextures("sprites/red/swim_to_right"), 0.1f);

            IsMovingLeft = Speed.X < 0;

            ResetAppearanceTimer();
            activeDuration = 10f; // Exotic fish stays for 5 seconds
            hasDroppedTreasure = false;
        }

        private void ResetAppearanceTimer()
        {
            appearanceTimer = Raylib.GetRandomValue(3, 8); // Random delay before appearing
            isLeaving = false;
        }

        public override void Update(float deltaTime, int windowWidth, int windowHeight)
        {
            // Appearance delay logic
            if (appearanceTimer > 0)
            {
                appearanceTimer -= deltaTime;
                return;
            }

            // Leaving the tank logic
            if (isLeaving)
            {
                LeaveTank(deltaTime, windowWidth);
                return;
            }

            // Drop treasure at the midpoint of the active duration
            if (!hasDroppedTreasure && activeDuration <= 5f) // Halfway through 10 seconds
            {
                DropTreasure();
            }

            // Regular movement logic
            Position += Speed * deltaTime;

            // Check for X-axis wall collisions with a threshold
            const float collisionThreshold = 10f;

            if (Position.X <= collisionThreshold) // Left wall collision
            {
                Position = new Vector2(collisionThreshold, Position.Y);
                Speed = new Vector2(Math.Abs(Speed.X), Speed.Y);
                IsMovingLeft = false;
            }
            else if (Position.X + 100 >= windowWidth - collisionThreshold) // Right wall collision
            {
                Position = new Vector2(windowWidth - 100 - collisionThreshold, Position.Y);
                Speed = new Vector2(-Math.Abs(Speed.X), Speed.Y);
                IsMovingLeft = true;
            }

            // Check for Y-axis wall collisions
            if (Position.Y <= 0 || Position.Y + 100 >= windowHeight)
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
                SetLeaveDirection(windowWidth);
            }
        }

        private void LeaveTank(float deltaTime, int windowWidth)
        {
            Position += Speed * deltaTime;

            // Check if the fish has exited the screen
            if (Position.X < -200 || Position.X > windowWidth + 200)
            {
                ResetAppearanceTimer();
                activeDuration = 20f;
                Position = GetRandomPosition(windowWidth);
                hasDroppedTreasure = false;
            }

            // Ensure the direction is updated continuously while leaving
            IsMovingLeft = Speed.X < 0;
        }

        private void SetLeaveDirection(int windowWidth)
        {
            if (Position.X < windowWidth / 2)
            {
                Speed = new Vector2(-Math.Abs(Speed.X), 0); // Maintain original speed magnitude
            }
            else
            {
                Speed = new Vector2(Math.Abs(Speed.X), 0); // Maintain original speed magnitude
            }

            IsMovingLeft = Speed.X < 0;
        }

        private Vector2 GetRandomPosition(int windowHeight)
        {
            int y = Raylib.GetRandomValue(100, windowHeight - 100);
            return new Vector2(-100, y);
        }

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
                (int)Position.X,
                (int)Position.Y,
                (int)(currentSprite.Width * scale),
                (int)(currentSprite.Height * scale)
            );

            Raylib.DrawTexturePro(currentSprite, srcRect, destRect, Vector2.Zero, 0.0f, Color.White);
        }

        public bool IsWithinBounds(Vector2 point)
        {
            float width = RightAnimator.GetCurrentFrame(0).Width * 0.5f;
            float height = RightAnimator.GetCurrentFrame(0).Height * 0.5f;

            Rectangle bounds = new Rectangle(Position.X, Position.Y, width, height);

            return Raylib.CheckCollisionPointRec(point, bounds);
        }
    }
}
