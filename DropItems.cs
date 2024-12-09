using System.Numerics;
using Raylib_cs;

namespace FishTankSimulator
{
    public abstract class DropItems
    {
        protected Vector2 position;
        protected bool isActive;

        protected Texture2D texture;
        protected float fallSpeed;

        // Constructor for DropItems, shared by all items
        protected DropItems(Vector2 position, float fallSpeed)
        {
            this.position = position;
            isActive = true;
            this.fallSpeed = fallSpeed;
        }

        public void OnPickup()
        {
            isActive = false;
        } 
        public abstract int GetValue();  // Abstract method to return value (specific to each item)


        // Update fall behavior
        public virtual void Update(float deltaTime, int windowHeight)
        {
            if (!isActive) return;

            position = new Vector2(position.X, position.Y + fallSpeed * deltaTime);

            if (position.Y >= windowHeight - texture.Height)
            {
                isActive = false;
            }
        }

        public void UnloadTexture()
        {
            Raylib.UnloadTexture(texture);
        }
        public Vector2 Position => position;
        public bool IsActive => isActive;
    }
}
