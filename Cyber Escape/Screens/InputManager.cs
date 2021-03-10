using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cyber_Escape.StateManagement;

namespace Cyber_Escape.Screens
{
    public class InputManager
    {
        private Vector2 movementDirection;
        private KeyboardState previousKeyboardState;
        private KeyboardState currentKeyboardState;
        public InputManager()
        {

        }

        public Vector2 Update(GameTime gameTime, InputState input)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = input.CurrentKeyboardStates[0];
            return Vector2.Zero;
        }
    }
}
