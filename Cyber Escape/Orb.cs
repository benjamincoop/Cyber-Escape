﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cyber_Escape
{
    class Orb : Sprite
    {
        // The portal about which the orb orbits
        private Portal portal;

        // The orgin position of the orb
        private Vector2 center;

        // The radius of this orb's orbit
        private float radius;

        // The angular speed at which this orb orbits
        private float speed;

        // The orbs current direction
        private float angle;



        public Orb(Texture2D texture, Portal portal, float radius, float speed)
        {
            CurrentTexture = texture;
            this.portal = portal;
            Position = portal.Position;
            this.radius = radius;
            this.speed = speed;
            angle = 0f;
        }

        public void Update(GameTime gameTime)
        {
            if(portal.IsActive)
            {
                // Set the orb's inital position to that of the portal
                Position = portal.Position;
                center = Position;

                // Find the location of the center of the portal
                //center = new Vector2(
                //    Position.X + (portal.CurrentTexture.Width / 2),
                //    Position.Y + (portal.CurrentTexture.Height / 2)
                //);

                // Calculate the new angle of rotation
                angle += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Calcuate and set the new orbital position of the orb
                Vector2 offset = new Vector2(
                    MathF.Sin(angle),
                    MathF.Cos(angle)
                ) * radius;

                Position = center + offset;
            } else
            {
                // If attached portal is inactive, move orb off screen so it can be garbage collected.
                Position = new Vector2(Constants.GAME_HEIGHT + 1, Constants.GAME_WIDTH + 1);
            }
        }
    }
}
