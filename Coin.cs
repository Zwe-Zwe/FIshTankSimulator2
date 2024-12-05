using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Coin : DropItems
    {
        public CoinType Type { get; private set; }
        private List<Texture2D> _animationFrames;
        private int _currentFrame;
        private float _frameTime;
        private float _animationTimer;
        private float _lifetime;
        private const float MaxLifetime = 3.0f;
        private bool _isAtBottom;

        public Coin(Vector2 position, CoinType type)
            : base(position, GetCoinTexturePath(type), 100f) // Use base class constructor
        {
            Type = type;
            _animationFrames = LoadAnimationFrames(type);
            _currentFrame = 0;
            _frameTime = 0.1f;
            _animationTimer = 0;
            _lifetime = MaxLifetime;
            _isAtBottom = false;
        }

        public override void OnPickup()
        {
            IsActive = false;
            Console.WriteLine($"Coin collected! Type: {Type}, Value: {GetValue()}");
        }

        public override int GetValue()
        {
            return Type switch
            {
                CoinType.Copper => 35,
                CoinType.Silver => 50,
                CoinType.Gold => 70,
                _ => 0
            };
        }

        public bool IsClicked(Vector2 mousePosition)
        {
            Rectangle coinRect = new Rectangle(Position.X, Position.Y, _animationFrames[0].Width, _animationFrames[0].Height);
            return Raylib.CheckCollisionPointRec(mousePosition, coinRect);
        }

        public override void Update(float deltaTime, int windowHeight)
        {
            if (!_isAtBottom && Position.Y < windowHeight - _animationFrames[0].Height + 50)
            {
                Position = new Vector2(Position.X, Position.Y + 100 * deltaTime);
            }
            else if (!_isAtBottom && Position.Y >= windowHeight - _animationFrames[0].Height + 50)
            {
                _isAtBottom = true;
            }

            if (_isAtBottom)
            {
                _lifetime -= deltaTime;
            }

            _animationTimer += deltaTime;
            if (_animationTimer >= _frameTime)
            {
                _animationTimer = 0;
                _currentFrame = (_currentFrame + 1) % _animationFrames.Count;
            }
        }

        public void Draw()
        {
            Texture2D currentSprite = _animationFrames[_currentFrame];
            float scaleFactor = Type switch
            {
                CoinType.Copper => 0.3f,
                CoinType.Silver => 0.4f,
                CoinType.Gold => 0.5f,
                _ => throw new ArgumentException("Invalid CoinType")
            };
            Raylib.DrawTextureEx(currentSprite, Position, 0.0f, scaleFactor, Color.White);
        }

        public void UnloadTextures()
        {
            foreach (var frame in _animationFrames)
            {
                Raylib.UnloadTexture(frame);
            }
        }

        private List<Texture2D> LoadAnimationFrames(CoinType type)
        {
            string folderName = type switch
            {
                CoinType.Copper => "sprites/bronze_coin",
                CoinType.Silver => "sprites/silver_coin",
                CoinType.Gold => "sprites/gold_coin",
                _ => throw new ArgumentException("Invalid CoinType")
            };

            List<Texture2D> frames = new List<Texture2D>();
            for (int i = 1; i <= 6; i++)
            {
                frames.Add(Raylib.LoadTexture($"{folderName}/{i:D2}.png"));
            }
            return frames;
        }

        private static string GetCoinTexturePath(CoinType type)
        {
            return type switch
            {
                CoinType.Copper => "sprites/bronze_coin/1.png",
                CoinType.Silver => "sprites/silver_coin/1.png",
                CoinType.Gold => "sprites/gold_coin/1.png",
                _ => throw new ArgumentException("Invalid CoinType")
            };
        }

        // New method to check if the coin has expired
        public bool IsExpired()
        {
            return _lifetime <= 0;
        }
    }
}
