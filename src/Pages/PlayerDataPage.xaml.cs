﻿using DrWPF.Windows.Data;
using Syncfusion.UI.Xaml.Controls.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RS3QuestFilter.src.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayerDataPage : Page
    {
        public PlayerDataPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContext = null;
            DataContext = App.ViewModel.VMPlayer;
        }

        private void skill_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (sender == null)
                return;

            ToggleButton button = sender as ToggleButton;

            if (button.Tag == null)
                throw new ArgumentException("Skill button was clicked, without having a Tag defined!");


            App.ViewModel.VMPlayer.PlayerData.Skills[button.Tag as string].Enabled = button.IsChecked ?? false;
        }

        private void SfNumericUpDown_ValueChanged(object sender, Syncfusion.UI.Xaml.Controls.Input.ValueChangedEventArgs e)
        {
            if (sender is not null)
            {
                SfNumericUpDown obj = (SfNumericUpDown)sender;
                App.ViewModel.VMPlayer.PlayerData.Skills[obj.Tag as string].Level = (int)(double)e.NewValue;
            }
        }

        private void OnLoad()
        {
            if (DataContext == null)
                DataContext = App.ViewModel.VMPlayer;

            ironCheck.IsChecked = App.ViewModel.VMPlayer.PlayerData.Flags.HasFlag(PlayerFlags.Ironman);
            hcCheck.IsChecked = App.ViewModel.VMPlayer.PlayerData.Flags.HasFlag(PlayerFlags.Hardcore);
            osatCheck.IsChecked = App.ViewModel.VMPlayer.PlayerData.Flags.HasFlag(PlayerFlags.Osaat);
            skillerCheck.IsChecked = App.ViewModel.VMPlayer.PlayerData.Flags.HasFlag(PlayerFlags.Skiller);
            memberCheck.IsChecked = App.ViewModel.VMPlayer.PlayerData.Flags.HasFlag(PlayerFlags.Member);

        }

        private void PagePlayer_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            OnLoad();
        }

        private void Checkbox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (sender == null)
                return;

            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Tag == null)
                throw new ArgumentException("Checkbox is expected to have Tag property, Tag is null.");

            if (checkBox.IsChecked is null)
                return;

            bool isChecked = checkBox.IsChecked ?? false;

            PlayerFlags flags = App.ViewModel.VMPlayer.PlayerData.Flags;
            switch (checkBox.Tag as string)
            {
                case "ironman":
                    if (isChecked)
                    {
                        flags |= PlayerFlags.Ironman;
                    }
                    else
                    {
                        if (flags.HasFlag(PlayerFlags.Ironman))
                            flags ^= PlayerFlags.Ironman;

                        if (flags.HasFlag(PlayerFlags.Hardcore))
                            flags ^= PlayerFlags.Hardcore;                          
                    }
                    break;

                case "hardcore":
                    if (isChecked)
                    {
                        flags |= PlayerFlags.Hardcore;
                    }
                    else
                    {
                        if (flags.HasFlag(PlayerFlags.Hardcore))
                            flags ^= PlayerFlags.Hardcore;
                    }
                    break;

                case "osat":
                    if (isChecked)
                    {
                        flags |= PlayerFlags.Osaat;
                    }
                    else
                    {
                        if (flags.HasFlag(PlayerFlags.Osaat))
                            flags ^= PlayerFlags.Osaat;
                    }
                    break;

                case "skiller":
                    if (isChecked)
                    {
                        flags |= PlayerFlags.Skiller;
                    }
                    else
                    {
                        if (flags.HasFlag(PlayerFlags.Skiller))
                            flags ^= PlayerFlags.Skiller;
                    }
                    break;

                case "member":
                    if (isChecked)
                    {
                        flags |= PlayerFlags.Member;
                    }
                    else
                    {
                        if (flags.HasFlag(PlayerFlags.Member))
                            flags ^= PlayerFlags.Member;
                    }
                    break;

                default:
                    throw new NotSupportedException($"{checkBox.Tag} is not a supported Flag");
            }

            App.ViewModel.VMPlayer.PlayerData.Flags = flags;
        }
    }

    public class SkillToLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            if (parameter == null)
                return null;

            ObservableDictionary<string, Skill> skills = value as ObservableDictionary<string, Skill>; 
            string key = parameter as string;
            return skills[key].Level;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
