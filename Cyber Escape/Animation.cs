using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cyber_Escape
{
    /// <summary>
    /// Defines a single animation loop for one sprite
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// The textures used in the animation
        /// </summary>
        public readonly Texture2D[] Frames;

        /// <summary>
        /// The number of game frames that are rendered between each animation frame transition
        /// </summary>
        private int frameDelay;

        /// <summary>
        /// Tracks the number of game frames rendered between animation frame transitions
        /// </summary>
        private int frameCounter = 0;

        /// <summary>
        /// An index into the frames array giving the current texture
        /// </summary>
        private int animIndex = 0;

        /// <summary>
        /// If true, animation will loop, otherwise animation will play just once.
        /// </summary>
        private bool isLoop;

        public Animation(Texture2D[] textures, int delay, bool loop)
        {
            Frames = textures;
            frameDelay = delay;
            isLoop = loop;
        }

        /// <summary>
        /// Updates the animation and returns the texture to be drawn
        /// </summary>
        /// <returns></returns>
        public Texture2D Animate()
        {
            if (frameCounter >= frameDelay)
            {
                animIndex++;
                if (animIndex >= Frames.Length)
                {
                    if (isLoop)
                    {
                        animIndex = 0;
                    }
                    else
                    {
                        animIndex = Frames.Length - 1;
                    }
                }
                frameCounter = 0;
            }
            frameCounter++;
            return Frames[animIndex];
        }

        /// <summary>
        /// Reverts animation to the beginning of the loop
        /// </summary>
        public void ResetAnimation()
        {
            frameCounter = 0;
            animIndex = 0;
        }
    }
}
