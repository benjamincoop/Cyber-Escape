using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cyber_Escape.StateManagement;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Cyber_Escape.Screens
{
    public class GameplayScreen : GameScreen
    {
        private ContentManager content;

        private float pauseAlpha;
        private readonly InputAction pauseAction;
        private readonly InputAction advanceAction;
        private Random random = new Random();
        private int highScore = 0;

        private Texture2D backgroundTexture;
        private Texture2D[] portalTextures;
        private List<Portal> portals = new List<Portal>();
        private const int numPortals = 2;

        private Texture2D orbTexture;
        private List<Orb> orbs = new List<Orb>();

        private Texture2D playerTexture;
        private Player player;

        private Song music;
        private SoundEffect advanceSFX;
        private SoundEffect failSFX;

        private bool isAdvancing = false;

        private int difficulty = 0;
        private bool tutorialComplete = false;

        private SpriteFont gameFont;
        private string messageStr;
        private Vector2 messagePos;

        private int score = 0;
        private bool gameOver = false;

        public GameplayScreen(int hiscore)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            highScore = hiscore;

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

            backgroundTexture = content.Load<Texture2D>("MainMenuBG");

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

            music = content.Load<Song>("GameMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = ((CyberEscape)ScreenManager.Game).MusicVol;
            SoundEffect.MasterVolume = ((CyberEscape)ScreenManager.Game).SFXVol;
            MediaPlayer.Play(music);

            advanceSFX = content.Load<SoundEffect>("playerSFX");
            failSFX = content.Load<SoundEffect>("failSFX");

            player = new Player(playerTexture, new Vector2(Constants.GAME_WIDTH / 2, Constants.GAME_HEIGHT));

            gameFont = content.Load<SpriteFont>("gamefont");

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

            if(IsActive)
            {
                if (gameOver)
                {
                    if(score > highScore)
                    {
                        highScore = score;
                    }
                    ExitScreen();
                    LoadingScreen.Load(ScreenManager, true, 0, new GameplayScreen(highScore));
                }

                foreach (Portal portal in portals) portal.Update();
                foreach (Orb orb in orbs)
                {
                    orb.Update(gameTime);
                    if (orb.bounds.CollidesWith(player.bounds) && gameOver == false)
                    {
                        failSFX.Play();
                        gameOver = true;
                        ScreenManager.AddScreen(new MessageBoxScreen("Game over!\nPress 'enter' to restart.", false), ControllingPlayer);
                        return;
                    }
                }
                player.Update(gameTime);

                // Indicates player has finished moving and we are ready to advance the level
                if (isAdvancing && player.IsMoving == false)
                {
                    isAdvancing = false;
                    score += 100;

                    // deactivate portal
                    portals[portals.Count - 1].IsActive = false;
                    // spawn a new portal at the top of the screen
                    portals.Add(SpawnPortal());
                    // add orb(s) to new portal
                    SpawnOrbs(portals[portals.Count - 1]);
                    if (tutorialComplete)
                    {
                        difficulty = random.Next(3, 11);
                    }
                    else
                    {
                        difficulty++;
                    }

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
                else
                {
                    if (portals[portals.Count - 1].IsSliding == false && player.IsMoving == false && difficulty >= 2 && score > 0)
                    {
                        score -= 1;
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
                Deactivate();
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            if (advanceAction.Occurred(input, ControllingPlayer, out player))
            {
                if(isAdvancing == false && portals[portals.Count - 1].IsSliding == false)
                {
                    isAdvancing = true;
                    this.player.Advance(portals[portals.Count - 1], 2000f);
                    advanceSFX.Play();
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

        private void SpawnOrbs(Portal portal)
        {
            //return new Orb(orbTexture, portal, 200f, 5f, startAngle);
            switch(difficulty)
            {
                case 0:
                    // 3 orbs, evenly spaced, slow
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 0f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 2f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 4f));
                    break;
                case 1:
                    // 4 orbs, line, slow
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 0.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 0.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 0.75f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 1.0f));
                    break;
                case 2:
                    // 2 lines of 2 orbs, same direction, slow
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 0f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 0.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 3.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 3.5f));
                    break;
                case 3:
                    // 5 orbs, line, medium speed
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 0f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 0.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 0.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 0.75f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 1.0f));
                    break;
                case 4:
                    // 2 lines of 3 orbs, opposing directions, medium speed
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 0f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 0.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 0.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -5f, 3f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -5f, 3.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -5f, 3.5f));
                    break;
                case 5:
                    // 6 orbs, line, fast
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0.75f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 1.0f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 1.25f));
                    break;
                case 6:
                    // 7 orbs, evenly spaced, medium speed
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 0.67f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 1.33f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 2f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 2.67f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 3.33f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 4f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 4.67f));
                    break;
                case 7:
                    // 2 lines of 4 orbs, opposing directions, fast
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0.75f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -7.5f, 3f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -7.5f, 3.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -7.5f, 3.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -7.5f, 3.75f));
                    break;
                case 8:
                    // 8 orbs, different speeds and radii
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 0f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 2.5f, 0.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 0.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 5f, 0.75f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, -5f, 1f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, -5f, 1.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 1.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 1.75f));
                    break;
                case 9:
                    // two lines of 8 orbs, opposing directions, different radii, slow and medium speed
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 0.75f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 1f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 1.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 1.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, 7.5f, 1.75f));

                    orbs.Add(new Orb(orbTexture, portal, 100f, -5f, 0f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, -5f, 0.25f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, -5f, 0.5f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, -5f, 0.75f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, -5f, 1f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, -5f, 1.25f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, -5f, 1.5f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, -5f, 1.75f));
                    break;
                case 10:
                    // two lines of 8 orbs, 8 spread evenly, opposing directions, different radii, all speeds
                    orbs.Add(new Orb(orbTexture, portal, 300f, 7.5f, 0f));
                    orbs.Add(new Orb(orbTexture, portal, 300f, 7.5f, 0.25f));
                    orbs.Add(new Orb(orbTexture, portal, 300f, 7.5f, 0.5f));
                    orbs.Add(new Orb(orbTexture, portal, 300f, 7.5f, 0.75f));
                    orbs.Add(new Orb(orbTexture, portal, 300f, 7.5f, 1f));
                    orbs.Add(new Orb(orbTexture, portal, 300f, 7.5f, 1.25f));
                    orbs.Add(new Orb(orbTexture, portal, 300f, 7.5f, 1.5f));
                    orbs.Add(new Orb(orbTexture, portal, 300f, 7.5f, 1.75f));

                    orbs.Add(new Orb(orbTexture, portal, 200f, -5f, 0f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -5f, 0.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -5f, 0.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -5f, 0.75f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -5f, 1f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -5f, 1.25f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -5f, 1.5f));
                    orbs.Add(new Orb(orbTexture, portal, 200f, -5f, 1.75f));

                    orbs.Add(new Orb(orbTexture, portal, 100f, 2.5f, 0.5f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, 2.5f, 1f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, 2.5f, 1.5f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, 2.5f, 2f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, 2.5f, 2.5f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, 2.5f, 3f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, 2.5f, 3.5f));
                    orbs.Add(new Orb(orbTexture, portal, 100f, 2.5f, 4f));

                    tutorialComplete = true;
                    break;
                default:
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;
            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullscreen,
                new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            foreach (Portal portal in portals)
            {
                spriteBatch.Draw(portal.CurrentTexture, portal.Position, null, portal.ShadingColor, portal.Rotation, new Vector2(64, 64), portal.ScaleFactor, SpriteEffects.None, 0);
            }

            foreach(Orb orb in orbs)
            {
                spriteBatch.Draw(orb.CurrentTexture, orb.Position, null, orb.ShadingColor, orb.Rotation, new Vector2(24, 24), orb.ScaleFactor, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(player.CurrentTexture, player.Position, null, player.ShadingColor, player.Rotation, new Vector2(24, 24), player.ScaleFactor, SpriteEffects.None, 0);

            messageStr = "High Score: " + highScore.ToString();
            messagePos = new Vector2(Constants.GAME_WIDTH - (gameFont.MeasureString(messageStr).X), 0);
            spriteBatch.DrawString(gameFont, messageStr, messagePos, Color.White);

            switch (difficulty)
            {
                case 0:
                    messageStr = "Press 'space' to advance.";
                    messagePos = new Vector2(Constants.GAME_WIDTH / 2 - (gameFont.MeasureString(messageStr).X / 2), Constants.GAME_HEIGHT / 2 - (gameFont.MeasureString(messageStr).Y / 2));
                    spriteBatch.DrawString(gameFont, messageStr, messagePos, Color.White);
                    break;
                case 1:
                    messageStr = "Avoid the red orbs.";
                    messagePos = new Vector2(Constants.GAME_WIDTH / 2 - (gameFont.MeasureString(messageStr).X / 2), Constants.GAME_HEIGHT / 2 - (gameFont.MeasureString(messageStr).Y / 2));
                    spriteBatch.DrawString(gameFont, messageStr, messagePos, Color.White);
                    break;
                case 2:
                    messageStr = "Your score decreases while you remain stationary.";
                    messagePos = new Vector2(Constants.GAME_WIDTH / 2 - (gameFont.MeasureString(messageStr).X / 2), Constants.GAME_HEIGHT / 2 - (gameFont.MeasureString(messageStr).Y / 2));
                    spriteBatch.DrawString(gameFont, messageStr, messagePos, Color.White);
                    break;
                default:
                    break;
            }

            if(difficulty >= 2)
            {
                spriteBatch.DrawString(gameFont, "Score: "+score.ToString(), Vector2.Zero, Color.White);
            }

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
