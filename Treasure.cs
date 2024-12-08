using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public class Treasure : DropItems
    {
        private List<Texture2D> _animationFrames;
        private int _currentFrame;
        private float _frameTime;
        private float _animationTimer;
        private float _lifetime;
        private const float MaxLifetime = 2.5f;
        private bool _isAtBottom;
        public Treasure(Vector2 position)
            : base(position, GetCoinTexturePath(), 300f) // Use base class constructor
        {
            _animationFrames = LoadAnimationFrames();
            _currentFrame = 0;
            _frameTime = 0.1f;
            _animationTimer = 0;
            _lifetime = MaxLifetime;
            _isAtBottom = false;
        }

        public override void OnPickup()
        {
            IsActive = false;
        }

        public override int GetValue()
        {
            return 200 + (Player.GameLevel -1) * 100;
        }

        public bool IsClicked(Vector2 mousePosition)
        {
            Rectangle coinRect = new Rectangle(Position.X, Position.Y, _animationFrames[0].Width, _animationFrames[0].Height);
            return Raylib.CheckCollisionPointRec(mousePosition, coinRect);
        }

        public override void Update(float deltaTime, int windowHeight)
        {
            if (!_isAtBottom && Position.Y < windowHeight - _animationFrames[0].Height * 0.1f)
            {
                Position = new Vector2(Position.X, Position.Y + fallSpeed * deltaTime);
            }
            else if (!_isAtBottom && Position.Y >= windowHeight - _animationFrames[0].Height * 0.1f)
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
            float scaleFactor = 0.1f;
            Raylib.DrawTextureEx(currentSprite, Position, 0.0f, scaleFactor, Color.White);
        }

        public void UnloadTextures()
        {
            foreach (var frame in _animationFrames)
            {
                Raylib.UnloadTexture(frame);
            }
        }

        private List<Texture2D> LoadAnimationFrames()
        {
            string folderName = "sprites/gold_egg";
            List<Texture2D> frames = new List<Texture2D>();

            for (int i = 1; i <= 12; i++) // Loop from 1 to 11
            {
                string filePath = $"{folderName}/{i:D2}.png";

                // Load each texture and add to the frames list
                if (File.Exists(filePath)) // Optional: Check if the file exists
                {
                    frames.Add(Raylib.LoadTexture(filePath));
                }
                else
                {
                    Console.WriteLine($"File not found: {filePath}"); // Debug message for missing files
                }
            }

            return frames;
        }


        private static string GetCoinTexturePath()
        {
            return "sprites/gold_egg/1.png";
        }
        // New method to check if the coin has expired
        public bool IsExpired()
        {
            return _lifetime <= 0;
        }
    }
}
