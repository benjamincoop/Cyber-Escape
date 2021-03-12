﻿using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cyber_Escape.StateManagement;

namespace Cyber_Escape.Screens
{
    // This screen implements the actual game logic. It is just a
    // placeholder to get the idea across: you'll probably want to
    // put some more interesting gameplay in here!
    public class GameplayScreen : GameScreen
    {
        private ContentManager content;

        private float pauseAlpha;
        private readonly InputAction pauseAction;
        private readonly InputAction advanceAction;
        private Random random = new Random();

        private Texture2D[] portalTextures;
        private List<Portal> portals = new List<Portal>();
        private const int numPortals = 2;

        private Texture2D orbTexture;
        private List<Orb> orbs = new List<Orb>();

        private Texture2D playerTexture;
        private Player player;

        private bool isAdvancing = false;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);

            advanceAction = new InputAction(
                new[] { Buttons.A, Buttons.B },
                new[] { Keys.Enter, Keys.Space }, true);
        }

        // Load graphics content for the game
        public override void Activate()
        {
            if (content == null) content = new ContentManager(ScreenManager.Game.Services, "Content");

            portalTextures = new Texture2D[]
            {
                content.Load<Texture2D>("portal0"),
                content.Load<Texture2D>("portal1"),
                content.Load<Texture2D>("portal2"),
                content.Load<Texture2D>("portal3"),
                content.Load<Texture2D>("portal4"),
                content.Load<Texture2D>("portal5"),
                content.Load<Texture2D>("portal6"),
                content.Load<Texture2D>("portal7"),
                content.Load<Texture2D>("portal8"),
                content.Load<Texture2D>("portal9"),
                content.Load<Texture2D>("portal10"),
                content.Load<Texture2D>("portal11"),
                content.Load<Texture2D>("portal12"),
                content.Load<Texture2D>("portal13"),
                content.Load<Texture2D>("portal14"),
                content.Load<Texture2D>("portal15")
            };

            orbTexture = content.Load<Texture2D>("orb_red");

            // Add starting portal
            portals.Add(SpawnPortal());

            playerTexture = content.Load<Texture2D>("orb_blue");

            player = new Player(playerTexture, new Vector2(Constants.GAME_WIDTH / 2, Constants.GAME_HEIGHT));

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
            {
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            }
            else
            {
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
            }

            foreach (Portal portal in portals) portal.Update();
            foreach (Orb orb in orbs) orb.Update(gameTime);
            player.Update(gameTime);

            // Indicates player has finished moving and we are ready to advance the level
            if(isAdvancing && player.IsMoving == false)
            {
                isAdvancing = false;

                // deactivate portal
                portals[portals.Count - 1].IsActive = false;
                // spawn a new portal at the top of the screen
                portals.Add(SpawnPortal());
                // add orb(s) to new portal
                orbs.Add(SpawnOrb(portals[portals.Count - 1]));

                // slide down and remove offscreen sprites
                foreach (Portal portal in portals.ToArray())
                {
                    portal.Slide();
                    if (portal.Position.Y > Constants.GAME_HEIGHT)
                    {
                        portals.Remove(portal);
                    }
                }
                foreach (Orb orb in orbs.ToArray())
                {
                    if (orb.Position.Y > Constants.GAME_HEIGHT)
                    {
                        orbs.Remove(orb);
                    }
                }
            }
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            if (advanceAction.Occurred(input, ControllingPlayer, out player))
            {
                if(isAdvancing == false && portals[portals.Count - 1].IsSliding == false)
                {
                    isAdvancing = true;
                    this.player.Advance(portals[portals.Count - 1], 2000f);
                }
            }
        }

        private Portal SpawnPortal()
        {
            //Vector2 pos = new Vector2(random.Next(0, Constants.GAME_WIDTH), (Constants.GAME_HEIGHT / (numPortals + 1)) * (numPortals - portalCount));
            Vector2 pos = new Vector2(random.Next(0, Constants.GAME_WIDTH), 0);
            Portal portal = new Portal(pos, portalTextures);
            portal.IsActive = true;
            return portal;
        }

        private Orb SpawnOrb(Portal portal)
        {
            return new Orb(orbTexture, portal, 200f, 5f);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            foreach(Portal portal in portals)
            {
                spriteBatch.Draw(portal.CurrentTexture, portal.Position, null, portal.ShadingColor, portal.Rotation, new Vector2(64, 64), portal.ScaleFactor, SpriteEffects.None, 0);
            }

            foreach(Orb orb in orbs)
            {
                spriteBatch.Draw(orb.CurrentTexture, orb.Position, null, orb.ShadingColor, orb.Rotation, new Vector2(24, 24), orb.ScaleFactor, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(player.CurrentTexture, player.Position, null, player.ShadingColor, player.Rotation, new Vector2(24, 24), player.ScaleFactor, SpriteEffects.None, 0);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
