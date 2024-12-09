using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class PredatorFish : Fish
    {
        // Static cache for storing loaded textures to avoid reloading
        private static readonly Dictionary<string, List<Texture2D>> TextureCache = new();

        private readonly PredatorFishType _type;
        private float _appearanceTimer;
        private float _activeDuration;
        private bool _isLeaving;
        private CoinFish _targetFish;
        private bool _isEating;
        private float _eatingTimer = 1f;

        private Animator _leftAnimator;
        private Animator _rightAnimator;
        private Animator _leftAnimatorSnapping;
        private Animator _rightAnimatorSnapping;

        public event Action<Fish> OnFishEaten;

        public PredatorFish(Vector2 initialPosition, Tank tank, PredatorFishType type) : base(tank)
        {
            Position = initialPosition;
            _type = type;

            // Initialize health based on the predator type
            float baseHealth = type switch
            {
                PredatorFishType.Small => 200f,
                PredatorFishType.Medium => 400f,
                PredatorFishType.Big => 800f,
                _ => 200f
            };
            Health = new Health(baseHealth);

            // Initialize speed based on the predator type
            Vector2 baseSpeed = type switch
            {
                PredatorFishType.Small => new Vector2(70, 30),
                PredatorFishType.Medium => new Vector2(85, 40),
                PredatorFishType.Big => new Vector2(100, 50),
                _ => new Vector2(70, 30)
            };
            Speed = baseSpeed;

            // Load textures and initialize animators
            _leftAnimator = new Animator(GetCachedTextures("sprites/shark/swim_to_left"), 0.1f);
            _rightAnimator = new Animator(GetCachedTextures("sprites/shark/swim_to_right"), 0.1f);
            _leftAnimatorSnapping = new Animator(GetCachedTextures("sprites/shark/swim_to_left_snapping"), 0.1f);
            _rightAnimatorSnapping = new Animator(GetCachedTextures("sprites/shark/swim_to_right_snapping"), 0.1f);

            // Determine initial direction
            IsMovingLeft = Speed.X < 0;

            // Initialize timers and state
            ResetAppearanceTimer();
            _activeDuration = 10f;
        }

        /// <summary>
        /// Load or retrieve textures from the cache.
        /// </summary>
         private static List<Texture2D> GetCachedTextures(string basePath)
        {
            if (!TextureCache.ContainsKey(basePath))
            {
                TextureCache[basePath] = LoadTextures(basePath);
            }

            return TextureCache[basePath];
        }

        /// <summary>
        /// Reset the timer for when the predator appears.
        /// </summary>
        private void ResetAppearanceTimer()
        {
            _appearanceTimer = Raylib.GetRandomValue(7, 12); // Random delay before appearing
            _isLeaving = false;
        }

        /// <summary>
        /// Update the predator's state.
        /// </summary>
        public override void Update(float deltaTime)
        {
            // Handle appearance delay
            if (_appearanceTimer > 0)
            {
                _appearanceTimer -= deltaTime;
                return;
            }

            // Handle leaving the tank
            if (_isLeaving)
            {
                LeaveTank(deltaTime);
                return;
            }

            // Handle active duration
            _activeDuration -= deltaTime;
            if (_activeDuration <= 0)
            {
                _isLeaving = true;
                SetLeaveDirection();
                return;
            }

            // Move the predator
            Position += Speed * deltaTime;

            // Bounce off the tank walls
            if (Position.X <= 0 || Position.X + 100 >= Program.windowWidth)
            {
                Speed = new Vector2(-Speed.X, Speed.Y);
                IsMovingLeft = Speed.X < 0;
            }

            if (Position.Y <= 0 || Position.Y + 100 >= Program.windowHeight)
            {
                Speed = new Vector2(Speed.X, -Speed.Y);
            }

            // Target and chase fish
            _targetFish = Tank.FindNearestFish(Position);
            if (_targetFish != null)
            {
                FollowAndEatFish(_targetFish, deltaTime);
            }
            else
            {
                _isEating = false;
            }
        }

        /// <summary>
        /// Logic for the predator leaving the tank.
        /// </summary>
        private void LeaveTank(float deltaTime)
        {
            Position += Speed * deltaTime;

            // Check if the predator has exited the tank
            if (Position.X < -100 || Position.X > Program.windowWidth + 100)
            {
                ResetAppearanceTimer();
                _activeDuration = 10f;
                Position = GetRandomPosition();
            }

            // Continuously update direction
            IsMovingLeft = Speed.X < 0;
        }

        /// <summary>
        /// Set the direction for leaving the tank.
        /// </summary>
        private void SetLeaveDirection()
        {
            if (Position.X < Program.windowWidth / 2)
            {
                Speed = new Vector2(-200, 0); // Exit left
            }
            else
            {
                Speed = new Vector2(200, 0); // Exit right
            }

            IsMovingLeft = Speed.X < 0;
        }

        /// <summary>
        /// Get a random position for re-entering the tank.
        /// </summary>
        private Vector2 GetRandomPosition()
        {
            int y = Raylib.GetRandomValue(100, Program.windowHeight - 100);
            return new Vector2(-100, y);
        }

        /// <summary>
        /// Follow and attempt to eat a fish.
        /// </summary>
        private void FollowAndEatFish(CoinFish fish, float deltaTime)
        {
            Vector2 direction = Vector2.Normalize(fish.Position - Position);
            Speed = direction * 100f; // Consistent chase speed

            Position += Speed * deltaTime;
            IsMovingLeft = Speed.X < 0;

            if (Vector2.Distance(Position, fish.Position) < 30f)
            {
                _isEating = true;
                EatFish(fish);
            }
        }

        /// <summary>
        /// Eat the targeted fish.
        /// </summary>
        public void EatFish(CoinFish fish)
        {
            OnFishEaten?.Invoke(fish);
            Tank.RemoveFish(fish);
            Health.Increase(50f);
            _isEating = true;
        }

        /// <summary>
        /// Draw the predator fish with animations.
        /// </summary>
        public override void Draw(float deltaTime)
        {
            if (_appearanceTimer > 0) return;

            float scale = _type switch
            {
                PredatorFishType.Small => 0.3f,
                PredatorFishType.Medium => 0.45f,
                PredatorFishType.Big => 0.65f,
                _ => 0.4f
            };

            Texture2D currentSprite = _isEating
                ? (IsMovingLeft ? _leftAnimatorSnapping.GetCurrentFrame(deltaTime) : _rightAnimatorSnapping.GetCurrentFrame(deltaTime))
                : (IsMovingLeft ? _leftAnimator.GetCurrentFrame(deltaTime) : _rightAnimator.GetCurrentFrame(deltaTime));

            Rectangle srcRect = new Rectangle(0, 0, currentSprite.Width, currentSprite.Height);
            Rectangle destRect = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                (int)(currentSprite.Width * scale),
                (int)(currentSprite.Height * scale)
            );

            Raylib.DrawTexturePro(currentSprite, srcRect, destRect, Vector2.Zero, 0.0f, Color.White);
            Health.Draw(new Vector2(Position.X, Position.Y - 10), 60, 5);

            if (_isEating)
            {
                ResetEatingAnimation(deltaTime);
            }
        }

        public bool IsWithinBounds(Vector2 point)
        {
            // Scale factor based on predator type
            float scale = _type switch
            {
                PredatorFishType.Small => 0.3f,
                PredatorFishType.Medium => 0.45f,
                PredatorFishType.Big => 0.65f,
                _ => 0.4f
            };

            // Calculate the scaled width and height of the sprite
            float width = _rightAnimator.GetCurrentFrame(0).Width * scale;
            float height = _leftAnimator.GetCurrentFrame(0).Height * scale;

            // Create a rectangle representing the bounds of the predator
            Rectangle bounds = new Rectangle(Position.X, Position.Y, width, height);

            // Check if the given point is within the rectangle
            return Raylib.CheckCollisionPointRec(point, bounds);
        }


        /// <summary>
        /// Reset the eating animation.
        /// </summary>
        private void ResetEatingAnimation(float deltaTime)
        {
            _eatingTimer += deltaTime;
            if (_eatingTimer >= 1.0f)
            {
                _isEating = false;
                _eatingTimer = 0f;
            }
        }

        /// <summary>
        /// Handle being attacked.
        /// </summary>
        public void GetAttacked(float damage)
        {
            Health.Reduce(damage);
            Console.WriteLine($"Predator fish attacked! Damage: {damage}");
        }

        public void UnloadTextures()
        {
            // Unload textures used by the animators
            _leftAnimator.UnloadTextures();
            _rightAnimator.UnloadTextures();
            _leftAnimatorSnapping.UnloadTextures();
            _rightAnimatorSnapping.UnloadTextures();
        }


    }
}
