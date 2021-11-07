using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RS3QuestFilter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        #region Events
        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            bool settings = args.IsSettingsSelected;
            if (settings)
            {
                ContentFrame.Navigate(typeof(src.Pages.SettingsPage));
            }
            else
            {
                var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.SelectedItem);
                switch (item.Tag)
                {
                    case "Home_Page":
                        ContentFrame.Navigate(typeof(src.Pages.HomePage));
                        break;
                    case "Quests_Page":
                        ContentFrame.Navigate(typeof(src.Pages.QuestListPage));
                        break;
                    case "Player_Page":
                        ContentFrame.Navigate(typeof(src.Pages.PlayerDataPage));
                        break;
                    case "Settings_Page":
                        ContentFrame.Navigate(typeof(src.Pages.SettingsPage));
                        break;
                }
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString().Equals("Home_Page"))
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
            ContentFrame.Navigate(typeof(src.Pages.HomePage));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            /*
            bool settings = args.IsSettingsInvoked;
            if (settings)
            {
                ContentFrame.Navigate(typeof(src.Pages.SettingsPage));
            }
            else
            {
                var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                switch (item.Tag)
                {
                    case "Home_Page":
                        ContentFrame.Navigate(typeof(src.Pages.HomePage));
                        break;
                    case "Quests_Page":
                        ContentFrame.Navigate(typeof(src.Pages.QuestListPage));
                        break;
                    case "Player_Page":
                        ContentFrame.Navigate(typeof(src.Pages.PlayerDataPage));
                        break;
                    case "Settings_Page":
                        ContentFrame.Navigate(typeof(src.Pages.SettingsPage));
                        break;
                }
            }
            */
        }
        #endregion
        #region Commands

        private void SaveCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }

        private async void SaveAsCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await src.FileHandler.SaveAll();
        }

        private void OpenCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }

        private void NewCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }

        private async void ExportCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            await src.FileHandler.Export();
        }

        private void ImportCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {

        }

        private void ExitCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => App.Current.Exit();

        #endregion
        public static void ShowAlert(string message)
        {
            ContentDialog alert = new();

            alert.Content = message;
            alert.CloseButtonText = "Okay";
        }
    }
}
