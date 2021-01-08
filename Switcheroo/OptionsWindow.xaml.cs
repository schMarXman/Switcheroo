/*
 * Switcheroo - The incremental-search task switcher for Windows.
 * http://www.switcheroo.io/
 * Copyright 2009, 2010 James Sulak
 * Copyright 2014 Regin Larsen
 * 
 * Switcheroo is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Switcheroo is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Switcheroo.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using ManagedWinapi;
using Switcheroo.Core;
using Switcheroo.Properties;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace Switcheroo
{
    public partial class OptionsWindow : Window
    {
        public enum DisplayBehaviour
        {
            FollowCursor,
            PrimaryScreen,
            CustomScreen
        }

        public enum ThemeMode
        {
            // Light or dark depending on windows settings
            SystemDefault,
            DefaultDark,
            DefaultLight,
            CustomTheme
        }

        private readonly HotKey _hotkey;
        private HotkeyViewModel _hotkeyViewModel;

        public OptionsWindow()
        {
            InitializeComponent();

            // Show what's already selected     
            _hotkey = (HotKey)Application.Current.Properties["hotkey"];

            try
            {
                _hotkey.LoadSettings();
            }
            catch (HotkeyAlreadyInUseException)
            {
            }

            _hotkeyViewModel = new HotkeyViewModel
            {
                KeyCode = KeyInterop.KeyFromVirtualKey((int)_hotkey.KeyCode),
                Alt = _hotkey.Alt,
                Ctrl = _hotkey.Ctrl,
                Windows = _hotkey.WindowsKey,
                Shift = _hotkey.Shift
            };

            HotKeyCheckBox.IsChecked = Settings.Default.EnableHotKey;
            HotkeyPreview.Text = _hotkeyViewModel.ToString();
            HotkeyPreview.IsEnabled = Settings.Default.EnableHotKey;
            AltTabCheckBox.IsChecked = Settings.Default.AltTabHook;
            AutoSwitch.IsChecked = Settings.Default.AutoSwitch;
            AutoSwitch.IsEnabled = Settings.Default.AltTabHook;
            RunAsAdministrator.IsChecked = Settings.Default.RunAsAdmin;

            SetupThemeDropdown();

            SetupDisplayBehaviourDropdown();
        }

        private void SetupThemeDropdown()
        {
            // Tuple: Text to display, enum, custom theme name
            var themes = new List<Tuple<string, ThemeMode, string>>()
            {
              new Tuple<string, ThemeMode, string>( "System default", ThemeMode.SystemDefault, ""),
              new Tuple<string, ThemeMode, string>( "Light",ThemeMode.DefaultLight, ""),
              new Tuple<string, ThemeMode, string>( "Dark", ThemeMode.DefaultDark,"")
            };

            // load theme files
            var themesDirectory = Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Themes"));
            var themeFiles = Directory.GetFiles(themesDirectory.FullName, "*.stheme");//.Select(x => Path.GetFileNameWithoutExtension(x));
            foreach (var file in themeFiles)
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                if (!themes.Any(x => x.Item1.Equals(filename, StringComparison.OrdinalIgnoreCase)))
                {
                    themes.Add(new Tuple<string, ThemeMode, string>(filename, ThemeMode.CustomTheme, Path.GetFileName(file)));
                }
            }

            var themeSetting = (ThemeMode)Enum.Parse(typeof(ThemeMode), Settings.Default.ThemeMode, true);

            ThemeDropdown.ItemsSource = themes;
            int index;
            if (themeSetting == ThemeMode.CustomTheme)
            {
                index = themes.FindIndex(x => x.Item3.Equals(Settings.Default.CustomTheme, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                index = themes.FindIndex(x => x.Item2 == themeSetting);
            }
            ThemeDropdown.SelectedIndex = index < 0 ? 0 : index;
        }

        private void SetupDisplayBehaviourDropdown()
        {
            // Tuple: Text to display, enum, screen index
            var settings = new List<Tuple<string, DisplayBehaviour, int>>()
            {
                new Tuple<string, DisplayBehaviour, int>("Follow cursor", DisplayBehaviour.FollowCursor, -1),
                new Tuple<string, DisplayBehaviour, int>("Always on primary screen", DisplayBehaviour.PrimaryScreen, -1)
            };

            if (Screen.AllScreens.Length > 1)
            {
                for (int i = 0; i < Screen.AllScreens.Length; i++)
                {
                    settings.Add(new Tuple<string, DisplayBehaviour, int>("Always on screen " + (i + 1), DisplayBehaviour.CustomScreen, i));
                }
            }

            var behaviourSetting = (DisplayBehaviour)Enum.Parse(typeof(DisplayBehaviour), Settings.Default.DisplayBehaviour, true);

            BehaviourDropdown.ItemsSource = settings;
            int index;
            if (behaviourSetting == DisplayBehaviour.CustomScreen)
            {
                index = settings.FindIndex(x => x.Item3 == Settings.Default.ScreenIndex);
            }
            else
            {
                index = settings.FindIndex(x => x.Item2 == behaviourSetting);
            }
            BehaviourDropdown.SelectedIndex = index < 0 ? 0 : index;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var closeOptionsWindow = true;

            try
            {
                _hotkey.Enabled = false;

                if (Settings.Default.EnableHotKey)
                {
                    // Change the active hotkey
                    _hotkey.Alt = _hotkeyViewModel.Alt;
                    _hotkey.Shift = _hotkeyViewModel.Shift;
                    _hotkey.Ctrl = _hotkeyViewModel.Ctrl;
                    _hotkey.WindowsKey = _hotkeyViewModel.Windows;
                    _hotkey.KeyCode = (Keys)KeyInterop.VirtualKeyFromKey(_hotkeyViewModel.KeyCode);
                    _hotkey.Enabled = true;
                }

                _hotkey.SaveSettings();
            }
            catch (HotkeyAlreadyInUseException)
            {
                var boxText = "Sorry! The selected shortcut for activating Switcheroo is in use by another program. " +
                              "Please choose another.";
                MessageBox.Show(boxText, "Shortcut already in use", MessageBoxButton.OK, MessageBoxImage.Warning);
                closeOptionsWindow = false;
            }

            Settings.Default.EnableHotKey = HotKeyCheckBox.IsChecked.GetValueOrDefault();
            Settings.Default.AltTabHook = AltTabCheckBox.IsChecked.GetValueOrDefault();
            Settings.Default.AutoSwitch = AutoSwitch.IsChecked.GetValueOrDefault();
            Settings.Default.RunAsAdmin = RunAsAdministrator.IsChecked.GetValueOrDefault();

            var themeSetting = (Tuple<string, ThemeMode, string>)ThemeDropdown.SelectedItem;
            Settings.Default.ThemeMode = themeSetting.Item2.ToString();
            Settings.Default.CustomTheme = themeSetting.Item3.ToString();

            var behaviourSetting = (Tuple<string, DisplayBehaviour, int>)BehaviourDropdown.SelectedItem;
            Settings.Default.DisplayBehaviour = behaviourSetting.Item2.ToString();
            Settings.Default.ScreenIndex = behaviourSetting.Item3;

            Settings.Default.Save();

            if (closeOptionsWindow)
            {
                Close();
            }
        }

        private void HotkeyPreview_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // The text box grabs all input
            e.Handled = true;

            // Fetch the actual shortcut key
            var key = (e.Key == Key.System ? e.SystemKey : e.Key);

            // Ignore modifier keys
            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            var previewHotkeyModel = new HotkeyViewModel();
            previewHotkeyModel.Ctrl = (Keyboard.Modifiers & ModifierKeys.Control) != 0;
            previewHotkeyModel.Shift = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            previewHotkeyModel.Alt = (Keyboard.Modifiers & ModifierKeys.Alt) != 0;

            var winLKey = new KeyboardKey(Keys.LWin);
            var winRKey = new KeyboardKey(Keys.RWin);
            previewHotkeyModel.Windows = (winLKey.State & 0x8000) == 0x8000 || (winRKey.State & 0x8000) == 0x8000;
            previewHotkeyModel.KeyCode = key;

            var previewText = previewHotkeyModel.ToString();

            // Jump to the next element if the user presses only the Tab key
            if (previewText == "Tab")
            {
                ((UIElement)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                return;
            }

            HotkeyPreview.Text = previewText;
            _hotkeyViewModel = previewHotkeyModel;
        }

        private class HotkeyViewModel
        {
            public Key KeyCode { get; set; }
            public bool Shift { get; set; }
            public bool Alt { get; set; }
            public bool Ctrl { get; set; }
            public bool Windows { get; set; }

            public override string ToString()
            {
                var shortcutText = new StringBuilder();

                if (Ctrl)
                {
                    shortcutText.Append("Ctrl + ");
                }

                if (Shift)
                {
                    shortcutText.Append("Shift + ");
                }

                if (Alt)
                {
                    shortcutText.Append("Alt + ");
                }

                if (Windows)
                {
                    shortcutText.Append("Win + ");
                }

                var keyString =
                    KeyboardHelper.CodeToString((uint)KeyInterop.VirtualKeyFromKey(KeyCode)).ToUpper().Trim();
                if (keyString.Length == 0)
                {
                    keyString = new KeysConverter().ConvertToString(KeyCode);
                }

                // If the user presses "Escape" then show "Escape" :)
                if (keyString == "\u001B")
                {
                    keyString = "Escape";
                }

                shortcutText.Append(keyString);
                return shortcutText.ToString();
            }
        }

        private void HotkeyPreview_OnGotFocus(object sender, RoutedEventArgs e)
        {
            // Disable the current hotkey while the hotkey field is active
            _hotkey.Enabled = false;
        }

        private void HotkeyPreview_OnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                _hotkey.Enabled = true;
            }
            catch (HotkeyAlreadyInUseException)
            {
                // It is alright if the hotkey can't be reactivated
            }
        }

        private void AltTabCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            AutoSwitch.IsEnabled = true;
        }

        private void AltTabCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            AutoSwitch.IsEnabled = false;
            AutoSwitch.IsChecked = false;
        }

        private void HotKeyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            HotkeyPreview.IsEnabled = true;
        }

        private void HotKeyCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            HotkeyPreview.IsEnabled = false;
        }
    }
}