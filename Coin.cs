using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Coin : DropItems
    {
        public CoinType Type { get; private set; }
        private static readonly Dictionary<CoinType, List<Texture2D>> SharedTextures = new();
        private List<Texture2D> _animationFrames;
        private int _currentFrame;
        private float _frameTime;
        private float _animationTimer;
        private float _lifetime;
        private const float MaxLifetime = 3.0f;
        private bool _isAtBottom;

        public Coin(Vector2 position, CoinType type)
            : base(position, 100f) // Use base class constructor
        {
            Type = type;
            _animationFrames = LoadAnimationFrames(type);
            _currentFrame = 0;
            _frameTime = 0.1f;
            _animationTimer = 0;
            _lifetime = MaxLifetime;
            _isAtBottom = false;
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
            Rectangle coinRect = new Rectangle(position.X, position.Y, _animationFrames[0].Width, _animationFrames[0].Height);
            return Raylib.CheckCollisionPointRec(mousePosition, coinRect);
        }

        public override void Update(float deltaTime, int windowHeight)
        {
            if (!_isAtBottom && position.Y < windowHeight - _animationFrames[0].Height + 50)
            {
                position = new Vector2(position.X, position.Y + 100 * deltaTime);
            }
            else if (!_isAtBottom && position.Y >= windowHeight - _animationFrames[0].Height + 50)
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
            Raylib.DrawTextureEx(currentSprite, position, 0.0f, scaleFactor, Color.White);
        }

        public static void UnloadSharedTextures()
        {
            foreach (var frames in SharedTextures.Values)
            {
                foreach (var frame in frames)
                {
                    Raylib.UnloadTexture(frame);
                }
            }
            SharedTextures.Clear();
        }

        private List<Texture2D> LoadAnimationFrames(CoinType type)
        {
            if (!SharedTextures.ContainsKey(type))
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
                SharedTextures[type] = frames;
            }

            return SharedTextures[type];
        }

        public bool IsExpired()
        {
            return _lifetime <= 0;
        }
    }
}
