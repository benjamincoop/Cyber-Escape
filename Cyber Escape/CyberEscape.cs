using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cyber_Escape.Screens;
using Cyber_Escape.StateManagement;


namespace Cyber_Escape
{
    public class CyberEscape : Game
    {
        private GraphicsDeviceManager graphics;
        private readonly ScreenManager screenManager;

        public float SFXVol = 0.25f;
        public float MusicVol = 0.25f;

        public CyberEscape()
        {
            // Initalize game size and other parameters
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Constants.GAME_WIDTH;
            graphics.PreferredBackBufferHeight = Constants.GAME_HEIGHT;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Add ScreenFactory service and initalize ScreenManager
            Services.AddService(typeof(IScreenFactory), new ScreenFactory());
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            AddInitialScreens();
        }

        private void AddInitialScreens()
        {
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }


        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent() { }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}
