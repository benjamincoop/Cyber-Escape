using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cyber_Escape
{
    /// <summary>
    /// Defines a base class that all sprites inherit
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// The game this sprite is a part of
        /// </summary>
        public Game BaseGame { get; set; }

        /// <summary>
        /// The texture to draw for this sprite
        /// </summary>
        public Texture2D CurrentTexture { get; set; }

        /// <summary>
        /// The dimmensions of the sprite texture, before scaling
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// The position of the sprite
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;

        /// <summary>
        /// The scaling factor by which to increase or reduce sprite size
        /// </summary>
        public float ScaleFactor { get; set; } = 1f;

        /// <summary>
        /// The degree of rotation of the sprite
        /// </summary>
        public float Rotation { get; set; } = 0f;

        /// <summary>
        /// The color used to shade the sprite, usually white
        /// </summary>
        public Color ShadingColor { get; set; } = Color.White;

        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        /// <summary>
        /// Draws the sprite
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to render with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentTexture is null)
            {
                throw new InvalidOperationException("Texture not loaded.");
            }
            else
            {
                spriteBatch.Draw(CurrentTexture, Position, null, ShadingColor, Rotation, Vector2.Zero, ScaleFactor, Effects, 0);
            }

        }
    }
}