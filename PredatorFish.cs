using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class PredatorFish : Fish
    {
        private float appearanceTimer;
        private float activeDuration;
        private bool isLeaving;
        private CoinFish targetFish;
        private bool isEating;
        private float eatingTimer = 1f;
        private Animator LeftAnimatorSnapping;
        private Animator RightAnimatorSnapping;
        private bool isStunned;
        private float stunTimer;
        private float stunDuration;
        public event Action<Fish> OnFishEaten;
        

        public PredatorFish(Vector2 initialPosition, Tank tank) : base(tank)
        {
            Position = initialPosition;
            Health = new Health(200f);
            Speed = new Vector2(70, 30); 
            isStunned = false;
            stunDuration = 0f;
            stunTimer = 0f;
            // Load predator textures for swimming
            LeftAnimator = new Animator(LoadTextures("sprites/shark/swim_to_left"), 0.1f);
            RightAnimator = new Animator(LoadTextures("sprites/shark/swim_to_right"), 0.1f);

            // Load predator textures for snapping (eating)
            LeftAnimatorSnapping = new Animator(LoadTextures("sprites/shark/swim_to_left_snapping"), 0.1f);
            RightAnimatorSnapping = new Animator(LoadTextures("sprites/shark/swim_to_right_snapping"), 0.1f);

            IsMovingLeft = Speed.X < 0;

            ResetAppearanceTimer();
            activeDuration = 10f;

            
        }

        private void ResetAppearanceTimer()
        {
            appearanceTimer = Raylib.GetRandomValue(5, 10); // Random delay before appearing
            isLeaving = false;
        }

        public override void Update(float deltaTime, int windowWidth, int windowHeight)
        {
            
            // Handle stun logic
            if (isStunned)
            {
                stunTimer += deltaTime;  // Increase the stun timer by deltaTime
                if (stunTimer >= stunDuration)  // If the stun duration is over
                {
                    isStunned = false;  // Remove the stun effect
                }
            }
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

            // Active duration logic
            activeDuration -= deltaTime;
            if (activeDuration <= 0)
            {
                isLeaving = true;
                SetLeaveDirection(windowWidth);
                return;
            }

            // Regular movement logic
            Position += Speed * deltaTime;

            // Bounce off the walls
            if (Position.X <= 0 || Position.X + 100 >= windowWidth)
            {
                Speed = new Vector2(-Speed.X, Speed.Y);
                IsMovingLeft = Speed.X < 0;
            }

            if (Position.Y <= 0 || Position.Y + 100 >= windowHeight)
            {
                Speed = new Vector2(Speed.X, -Speed.Y);
            }

            // Target and eat fish logic
            targetFish = Tank.FindNearestFish(Position);
            if (targetFish != null)
            {
                FollowAndEatFish(targetFish, deltaTime);
            }
            else
            {
                isEating = false;
            }
        }


        private void LeaveTank(float deltaTime, int windowWidth)
        {
            Position += Speed * deltaTime;

            // Check if the fish has exited the screen
            if (Position.X < -100 || Position.X > windowWidth + 100)
            {
                ResetAppearanceTimer();
                activeDuration = 10f;
                Position = GetRandomPosition(windowWidth);
            }

            // Ensure the direction is updated continuously while leaving
            IsMovingLeft = Speed.X < 0;
        }


        private void SetLeaveDirection(int windowWidth)
        {
            if (Position.X < windowWidth / 2)
            {
                Speed = new Vector2(-200, 0); // Move left to exit
            }
            else
            {
                Speed = new Vector2(200, 0); // Move right to exit
            }

            // Update direction-facing property
            IsMovingLeft = Speed.X < 0;
        }


        private Vector2 GetRandomPosition(int windowWidth)
        {
            int y = Raylib.GetRandomValue(100, 1080 - 100);
            return new Vector2(-100, y);
        }

        private void FollowAndEatFish(CoinFish fish, float deltaTime)
        {
            float baseSpeed = 100f; // Consistent speed for chasing
            Vector2 direction = Vector2.Normalize(fish.Position - Position);
            Speed = direction * baseSpeed; // Apply consistent speed

            Position += Speed * deltaTime;

            IsMovingLeft = Speed.X < 0;

            if (Vector2.Distance(Position, fish.Position) < 30f)
            {
                isEating = true;
                EatFish(fish);
            }
        }


        public void EatFish(CoinFish fish)
        {
            OnFishEaten?.Invoke(fish);
            Tank.RemoveFish(fish);
            Health.Increase(50f);
            isEating = true;
        }

        public override void Draw(float deltaTime)
        {
            if (appearanceTimer > 0) return;

            Texture2D currentSprite = isEating
                ? (IsMovingLeft ? LeftAnimatorSnapping.GetCurrentFrame(deltaTime) : RightAnimatorSnapping.GetCurrentFrame(deltaTime))
                : (IsMovingLeft ? LeftAnimator.GetCurrentFrame(deltaTime) : RightAnimator.GetCurrentFrame(deltaTime));

            Rectangle srcRect = new Rectangle(0, 0, currentSprite.Width, currentSprite.Height);
            Rectangle destRect = new Rectangle(Position.X, Position.Y, currentSprite.Width * 0.5f, currentSprite.Height * 0.5f);

            Raylib.DrawTexturePro(currentSprite, srcRect, destRect, Vector2.Zero, 0.0f, Color.White);

            Health.Draw(new Vector2(Position.X, Position.Y - 10), 60, 5);

            if (isEating)
            {
                ResetEatingAnimation(deltaTime);
            }
        }

        public bool IsWithinBounds(Vector2 point)
        {
            float width = RightAnimator.GetCurrentFrame(0).Width * 0.5f;
            float height = RightAnimator.GetCurrentFrame(0).Height * 0.5f;

            Rectangle bounds = new Rectangle(Position.X, Position.Y, width, height);

            return Raylib.CheckCollisionPointRec(point, bounds);
        }

        private void ResetEatingAnimation(float deltaTime)
        {
            eatingTimer += deltaTime;

            if (eatingTimer >= 1.0f)
            {
                isEating = false;
                eatingTimer = 0f;
            }
        }

        public void GetAttacked(float damage, float stunTime)
        {
            Health.Reduce(damage);
            isStunned = true;
            stunDuration = stunTime;
            stunTimer = 0f;
        }


    }
}
