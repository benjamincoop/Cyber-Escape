using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cyber_Escape
{
    class Player : Sprite
    {
        // variables related to player movement
        public bool IsMoving = false;
        private Vector2 startPosition;
        private float moveDistance;
        private Vector2 moveDirection;
        private float moveSpeed;

        Portal portal;

        public Player(Texture2D texture, Vector2 position)
        {
            CurrentTexture = texture;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            // Updates the player position if they are moving
            if(IsMoving)
            {
                Position += moveDirection * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(Vector2.Distance(startPosition, Position) >= moveDistance)
                {
                    IsMoving = false;
                }
            } else
            {
                if(portal != null)
                {
                    Position = new Vector2(portal.Position.X + 10, portal.Position.Y + 10);
                }
            }
        }

        // Start moving player towards portal
        public void Advance(Portal portal, float speed)
        {
            startPosition = Position;
            this.portal = portal;
            moveDistance = Vector2.Distance(Position, portal.Position);
            moveDirection = Vector2.Normalize(portal.Position - Position);
            moveSpeed = speed;
            IsMoving = true;
        }
    }
}
