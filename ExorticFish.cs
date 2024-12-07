using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class ExoticFish : Fish
    {
        private float appearanceTimer;
        private float activeDuration; // Duration the Exotic Fish stays in the tank
        private bool isReturning;     // Indicates if the fish is leaving the tank
        private float speedMultiplier;
        private float treasureDropInterval;
        private float treasureDropTimer;
        private event Action<Treasure> onTreasureDropped;

        public ExoticFish(Vector2 initialPosition) : base()
        {
            Position = initialPosition;
            speedMultiplier = 3.0f; // Exotic fish is faster
            Health = new Health(100f); // Exotic fish has moderate health

            // Load exotic fish textures
            LeftAnimator = new Animator(LoadTextures("sprites/exotic/swim_to_left"), 0.1f);
            RightAnimator = new Animator(LoadTextures("sprites/exotic/swim_to_right"), 0.1f);

            // Set random appearance interval and active duration
            ResetAppearanceTimer();
            activeDuration = Raylib.GetRandomValue(20, 30); // Active duration for 2 rounds (~20-30s)
            treasureDropInterval = 5f; // Drops treasure every 5 seconds
            treasureDropTimer = treasureDropInterval;
        }

        public float ActiveDuration
        {
            get { return activeDuration; }
        }
        public float TreasureDropInterval
        {
            get { return treasureDropInterval; }
        }
        public float TreasureDropTimer
        {
            get { return treasureDropTimer; }
        }
        public event Action<Treasure> OnTreasureDropped
        {
            add { onTreasureDropped += value; }
            remove { onTreasureDropped -= value; }
        }
        private void ResetAppearanceTimer()
        {
            appearanceTimer = Raylib.GetRandomValue(60, 100); // Random delay before appearing
            isReturning = false; // Reset return state
        }

        public override void Update(float deltaTime, int windowWidth, int windowHeight)
        {
            // Countdown until the exotic fish appears
            if (appearanceTimer > 0)
            {
                appearanceTimer -= deltaTime;
                return;
            }

            if (isReturning)
            {
                ReturnToOrigin(deltaTime, windowWidth, windowHeight);
                return;
            }

            activeDuration -= deltaTime;

            // Exotic fish leaves after active duration
            if (activeDuration <= 0)
            {
                isReturning = true;
                SetReturnDirection(windowWidth);
                return;
            }

            Position += Speed * speedMultiplier * deltaTime;

            // Bounce off tank walls
            if (Position.X <= 0 || Position.X + 100 >= windowWidth)
            {
                Speed = new Vector2(-Speed.X, Speed.Y);
                IsMovingLeft = Speed.X < 0;
            }
            if (Position.Y <= 0 || Position.Y + 100 >= windowHeight)
            {
                Speed = new Vector2(Speed.X, -Speed.Y);
            }

            // Drop treasure periodically
            treasureDropTimer -= deltaTime;
            if (treasureDropTimer <= 0)
            {
                DropTreasure();
                treasureDropTimer = treasureDropInterval; // Reset timer for next drop
            }
        }

        private void DropTreasure()
        {
            Vector2 treasurePosition = Position;
            //TreasureType treasureType = (TreasureType)Raylib.GetRandomValue(0, Enum.GetValues(typeof(TreasureType)).Length - 1);
            TreasureType treasureType = TreasureType.Ruby;
            Treasure treasure = new Treasure(treasurePosition, treasureType);
            Tank.AddTreasure(treasure); // Assuming AddTreasure is implemented in Tank
        }

        private void ReturnToOrigin(float deltaTime, int windowWidth, int windowHeight)
        {
            Position += Speed * speedMultiplier * deltaTime;

            // Check if the fish has exited the screen
            if (Position.X < -100 || Position.X > windowWidth + 100)
            {
                ResetAppearanceTimer(); // Reset for next appearance
                activeDuration = Raylib.GetRandomValue(20, 30); // Reset active duration
                Position = GetRandomPosition(windowHeight); // Reposition off-screen to start
            }
        }

        private void SetReturnDirection(int windowWidth)
        {
            // Choose a direction to exit (left or right)
            if (Position.X < windowWidth / 2)
            {
                Speed = new Vector2(-300, 0); // Exit to the left faster
            }
            else
            {
                Speed = new Vector2(300, 0); // Exit to the right faster
            }
        }

        private Vector2 GetRandomPosition(int windowHeight)
        {
            int y = Raylib.GetRandomValue(100, windowHeight - 100);
            return new Vector2(-100, y); // Start off-screen to the left for next appearance
        }

        public override void Draw(float deltaTime)
        {
            if (appearanceTimer > 0)
            {
                return; // Exotic fish is not visible yet
            }

            Texture2D currentSprite = IsMovingLeft
                ? LeftAnimator.GetCurrentFrame(deltaTime)
                : RightAnimator.GetCurrentFrame(deltaTime);

            Rectangle srcRect = new Rectangle(0, 0, currentSprite.Width, currentSprite.Height);
            Rectangle destRect = new Rectangle(Position.X, Position.Y, currentSprite.Width * 0.5f, currentSprite.Height * 0.5f);

            Raylib.DrawTexturePro(currentSprite, srcRect, destRect, Vector2.Zero, 0.0f, Color.White);

            // Draw health bar
            Health.Draw(new Vector2(Position.X, Position.Y - 10), 60, 5);
        }

        
    }
}
