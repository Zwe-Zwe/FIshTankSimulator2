using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class CoinFish : Fish
    {
 
        private float coinDropTimer;

        private Growth growth;
    
        private Food targetFood;
        private CoinFishType fishType;

        public event Action<Coin> OnCoinDropped;

        public CoinFish(Vector2 initialPosition, CoinFishType type, Tank tank) : base(tank)
        {
            Position = initialPosition;
            fishType = type;
            growth = new Growth();

            // Set max health based on fish type
            float baseMaxHealth = type switch
            {
                CoinFishType.Guppy => 100f,      // Guppy has low health
                CoinFishType.Snapper => 150f,   // Snapper has medium health
                CoinFishType.Flounder => 200f,  // Flounder has high health
                _ => 100f
            };

            Health = new Health(baseMaxHealth * growth.GetHealthMultiplier());
            coinDropTimer = GetRandomDropInterval();

            // Load appropriate textures based on fish type
            string basePath = fishType switch
            {
                CoinFishType.Guppy => "sprites/guppy",
                CoinFishType.Snapper => "sprites/snapper",
                CoinFishType.Flounder => "sprites/flounder",
                _ => "sprites/guppy"
            };

            LeftAnimator = new Animator(LoadTextures($"{basePath}/swim_to_left"), 0.1f);
            RightAnimator = new Animator(LoadTextures($"{basePath}/swim_to_right"), 0.1f);
            IsMovingLeft = Speed.X < 0;
        }

        public float CoinDropTimer
        {
            get { return coinDropTimer; }
            set { coinDropTimer = value; }
        }
        public float GetRandomDropInterval()
        {
            // Adjust drop rate based on fish type
            return fishType switch
            {
                CoinFishType.Guppy => Raylib.GetRandomValue(7, 13),      // Guppy drops coins more often
                CoinFishType.Snapper => Raylib.GetRandomValue(6, 10),   // Snapper has a moderate drop rate
                CoinFishType.Flounder => Raylib.GetRandomValue(8, 14),  // Flounder drops coins less often
                _ => Raylib.GetRandomValue(7, 13)
            };
        }

        

        public override void Update(float deltaTime, int windowWidth, int windowHeight)
        {
            Position += Speed * deltaTime;

            // Bounce fish off tank walls
            if (Position.X <= 0 || Position.X + 80 >= windowWidth)
            {
                Speed = new Vector2(-Speed.X, Speed.Y);
                IsMovingLeft = Speed.X < 0;
            }
            if (Position.Y <= 0 || Position.Y + 80 >= windowHeight)
            {
                Speed = new Vector2(Speed.X, -Speed.Y);
            }

            // Decrease health over time
            Health.Reduce(4f * deltaTime);

            // Hunger logic: seek food
            if (Health.Current < Health.Max / 2)
            {
                targetFood = Tank.FindNearestFood(Position);
                if (targetFood != null)
                {
                    FollowAndEatFood(targetFood, deltaTime);
                }
            }
            else
            {
                // If no food is targeted, ensure the fish continues swimming naturally
                Speed = new Vector2(
                    IsMovingLeft ? -Math.Abs(Speed.X) : Math.Abs(Speed.X),
                    Speed.Y
                );
            }

            // Drop coin logic
            coinDropTimer -= deltaTime;
            if (coinDropTimer <= 0)
            {
                DropCoin();
                coinDropTimer = GetRandomDropInterval();
            }
        }

        private void DropCoin()
        {
            CoinType coinType = growth.CurrentStage switch
            {
                AgeStage.Hatchling => CoinType.Copper,
                AgeStage.Juvenile => CoinType.Silver,
                AgeStage.Adult => CoinType.Gold,
                _ => CoinType.Copper
            };

            Coin newCoin = new Coin(Position, coinType);
            OnCoinDropped?.Invoke(newCoin);
        }

        private void FollowAndEatFood(Food food, float deltaTime)
        {
            // Calculate direction towards the food
            Vector2 direction = Vector2.Normalize(food.Position - Position);

            // Update speed to follow the direction
            Speed = direction * Speed.Length();

            // Update fish position based on the new speed
            Position += Speed * deltaTime;

            // Adjust head direction
            IsMovingLeft = Speed.X < 0;

            // Check if the fish is close enough to eat the food
            if (Vector2.Distance(Position, food.Position) < 20f) // Reduced threshold for precision
            {
                EatFood(food);
                targetFood = null; // Reset target food after eating
            }
        }

        public void EatFood(Food food)
        { 
            food.OnPickup();
            Health.Increase(food.GetValue());
            growth.Eat();
        }

        public override void Draw(float deltaTime)
        {
            Texture2D currentSprite = IsMovingLeft
                ? LeftAnimator.GetCurrentFrame(deltaTime)
                : RightAnimator.GetCurrentFrame(deltaTime);

            // Draw fish with scaling
            float scaleFactor = growth.CurrentStage switch
            {
                AgeStage.Hatchling => 0.2f,
                AgeStage.Juvenile => 0.3f,
                AgeStage.Adult => 0.4f,
                _ => 0.2f
            };

            Rectangle srcRect = new Rectangle(0, 0, currentSprite.Width, currentSprite.Height);
            Rectangle destRect = new Rectangle(Position.X, Position.Y, currentSprite.Width * scaleFactor, currentSprite.Height * scaleFactor);

            Raylib.DrawTexturePro(currentSprite, srcRect, destRect, Vector2.Zero, 0.0f, Color.White);

            // Draw health bar
            Health.Draw(new Vector2(Position.X, Position.Y - 10), 50, 5);
        }

    }
}