using Microsoft.Xna.Framework;
using Cyber_Escape.StateManagement;
using Microsoft.Xna.Framework.Content;

namespace Cyber_Escape.Screens
{
    // The main menu screen is the first thing displayed when the game starts up.
    public class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen() : base("Cyber Escape")
        {
            var playGameMenuEntry = new MenuEntry("Play Game");
            var optionsMenuEntry = new MenuEntry("Options");
            var exitMenuEntry = new MenuEntry("Exit");

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(0, ScreenManager.Game));
        }

        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen((CyberEscape)ScreenManager.Game), e.PlayerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit?";
            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}
