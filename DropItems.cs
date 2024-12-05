using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public abstract class DropItems
    {
        public Vector2 Position { get; protected set; }
        public bool IsActive { get; protected set; }

        protected Texture2D texture;
        protected float fallSpeed;

        // Constructor for DropItems, shared by all items
        protected DropItems(Vector2 position, string texturePath, float fallSpeed)
        {
            Position = position;
            IsActive = true;
            this.fallSpeed = fallSpeed;
            texture = Raylib.LoadTexture(texturePath);
        }

        public abstract void OnPickup(); // Abstract method for pickup behavior
        public abstract int GetValue();  // Abstract method to return value (specific to each item)

        public Texture2D Texture => texture;

        // Update fall behavior
        public virtual void Update(float deltaTime, int windowHeight)
        {
            if (!IsActive) return;

            Position = new Vector2(Position.X, Position.Y + fallSpeed * deltaTime);

            if (Position.Y >= windowHeight - texture.Height)
            {
                IsActive = false;
            }
        }

        public void Draw()
        {
            if (IsActive)
            {
                Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
                float scaleFactor = 0.5f;
                Rectangle destRect = new Rectangle(
                    Position.X,
                    Position.Y,
                    texture.Width * scaleFactor,
                    texture.Height * scaleFactor
                );

                Raylib.DrawTexturePro(
                    texture,
                    sourceRect,
                    destRect,
                    Vector2.Zero,
                    0.0f,
                    Color.White
                );
            }
        }

        public void UnloadTexture()
        {
            Raylib.UnloadTexture(texture);
        }
    }
}
