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
        private const float MaxLifetime = 2f;
        private bool _isAtBottom;
        public Treasure(Vector2 position)
            : base(position, 400f) // Use base class constructor
        {
            _animationFrames = LoadAnimationFrames();
            _currentFrame = 0;
            _frameTime = 0.1f;
            _animationTimer = 0;
            _lifetime = MaxLifetime;
            _isAtBottom = false;
        }
        public List<Texture2D> AnimationFrames{
            get { return _animationFrames; }
            set { _animationFrames = value; }
        }
        public override int GetValue()
        {
            return 200 + (Player.GameLevel -1) * 100;
        }

        public bool IsClicked(Vector2 mousePosition)
        {
            Rectangle coinRect = new Rectangle(position.X, position.Y, _animationFrames[0].Width, _animationFrames[0].Height);
            return Raylib.CheckCollisionPointRec(mousePosition, coinRect);
        }

        public override void Update(float deltaTime)
        {
            if (!_isAtBottom && position.Y < Program.windowHeight - _animationFrames[0].Height * 0.1f)
            {
                position = new Vector2(position.X, position.Y + fallSpeed * deltaTime);
            }
            else if (!_isAtBottom && position.Y >= Program.windowHeight - _animationFrames[0].Height * 0.1f)
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
            Raylib.DrawTextureEx(currentSprite, position, 0.0f, scaleFactor, Color.White);
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

            for (int i = 1; i <= 12; i++) // Loop from 1 to 12
            {
                string filePath = $"{folderName}/{i}.png"; // No leading zero

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

        // New method to check if the coin has expired
        public bool IsExpired()
        {
            return _lifetime <= 0;
        }
    }
}
