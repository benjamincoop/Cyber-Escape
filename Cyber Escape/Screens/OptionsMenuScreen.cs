

using System;

namespace Cyber_Escape.Screens
{
    // The options screen is brought up over the top of the main menu
    // screen, and gives the user a chance to configure the game
    // in various hopefully useful ways.
    public class OptionsMenuScreen : MenuScreen
    {
        private CyberEscape game;
        private readonly MenuEntry SFXVolume;
        private readonly MenuEntry MusicVolume;

        private static float musicVol = 0.25f;
        private static float SFXVol = 0.25f;

        public OptionsMenuScreen(CyberEscape game) : base("Options")
        {
            this.game = game;

            SFXVolume = new MenuEntry(string.Empty);
            MusicVolume = new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            SFXVolume.Selected += SFXMenuEntrySelected;
            MusicVolume.Selected += MusicMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(MusicVolume);
            MenuEntries.Add(SFXVolume);
            MenuEntries.Add(back);
        }

        // Fills in the latest values for the options screen menu text.
        private void SetMenuEntryText()
        {
            SFXVolume.Text = $"SFX Volume: {Math.Truncate(SFXVol * 100)}%";
            MusicVolume.Text = $"Music Volume: {Math.Truncate(musicVol * 100)}%";

            game.SFXVol = SFXVol;
            game.MusicVol = musicVol;
        }

        private void SFXMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if(SFXVol < 1f)
            {
                SFXVol += 0.05f;
            } else
            {
                SFXVol = 0f;
            }
            
            SetMenuEntryText();
        }

        private void MusicMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (musicVol < 1f)
            {
                musicVol += 0.05f;
            }
            else
            {
                musicVol = 0f;
            }
            SetMenuEntryText();
        }
    }
}
