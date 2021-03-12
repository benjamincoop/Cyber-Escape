using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cyber_Escape
{
    public class Portal : Sprite
    {
        private Animation animation;
        public bool IsActive = false;
        public bool IsSliding = true;
        private float oldPosition;

        /// <summary>
        /// Constructs a new portal at given position
        /// </summary>
        /// <param name="position"></param>
        public Portal(Vector2 position, Texture2D[] textures)
        {
            Position = position;
            animation = new Animation(textures, 3, true);
            CurrentTexture = animation.Animate();
        }

        public void Update()
        {
            if (IsActive)
            {
                CurrentTexture = animation.Animate();
            } else
            {
                CurrentTexture = animation.Frames[3];
                Rotation += 0.1f;
            }

            if(IsSliding)
            {
                Position = new Vector2(Position.X, Position.Y + 18);
                if(Position.Y >= oldPosition + 540)
                {
                    IsSliding = false;
                }
            }
        }

        public void Slide()
        {
            if(IsSliding == false)
            {
                IsSliding = true;
                oldPosition = Position.Y;
            }
        }
    }
}
