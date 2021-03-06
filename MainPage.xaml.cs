using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
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
            DataContext = App.ViewModel;
        }

        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("Home_Page", typeof(src.Pages.HomePage)),
            ("Quests_Page_Dev", typeof(src.Pages.QuestListDevPage)),
            ("Quests_Page", typeof(src.Pages.QuestListPage)),
            ("Player_Page", typeof(src.Pages.PlayerDataPage))
        };

        private string _lastPage = "Home_Page";
        private string _currentPage = "Home_Page";

        #region Events

        #region Navigation
        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                NavView_Navigate("Settings_Page", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
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

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (!ContentFrame.CanGoBack)
                return;

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen && (NavView.DisplayMode == NavigationViewDisplayMode.Compact || NavView.DisplayMode == NavigationViewDisplayMode.Minimal))
                return;

            if (_lastPage.Equals("Quests_Page_Dev") && !App.ViewModel.IsDevMode)
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals("Quests_Page"));
                Type _page = item.Page;
                var preNavPageType = ContentFrame.CurrentSourcePageType;

                // Only navigate if the selected page isn't currently loaded.
                if (_page is not null && !Type.Equals(preNavPageType, _page))
                {
                    ContentFrame.Navigate(_page, null, null);
                }
            }
            else if (_lastPage.Equals("Quests_Page") && App.ViewModel.IsDevMode)
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals("Quests_Page_Dev"));
                Type _page = item.Page;
                var preNavPageType = ContentFrame.CurrentSourcePageType;

                // Only navigate if the selected page isn't currently loaded.
                if (_page is not null && !Type.Equals(preNavPageType, _page))
                {
                    ContentFrame.Navigate(_page, null, null);
                }
            }
            else
                ContentFrame.GoBack();
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag.Equals("Settings_Page"))
            {
                _page = typeof(src.Pages.SettingsPage);
            }
            else
            {
                if (navItemTag.Equals("Quests_Page"))
                {
                    App.ViewModel.IsQuestPage = true;
                    if (App.ViewModel.IsDevMode)
                    {
                        navItemTag += ("_Dev");
                    }
                }
                else
                    App.ViewModel.IsQuestPage = false;
                
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;

                _lastPage = navItemTag;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (_page is not null && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(src.Pages.SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
                NavView.Header = "Settings";
            }
            else if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().First(n => {
                    if (item.Tag.Equals("Quests_Page_Dev"))
                        return n.Tag.Equals("Quests_Page");
                    else
                        return n.Tag.Equals(item.Tag);
                });

                //NavView.Header = ((NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
                _currentPage = item.Tag.Equals("Quests_Page_Dev") ? "Quests_Page" : item.Tag;
            }
        }

        #endregion

        #endregion

        #region Commands

        private async void SaveCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
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

        private async void ImportCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                App.ViewModel.VMPlayer.PlayerData = await src.FileHandler.Import();
                App.ViewModel.VMPlayer.PlayerData.SelfCheckup();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Unable to deserialize 'PlayerData.xml'. Reason: {ex.Message}\nUsing default initialisers instead...");
                App.ViewModel.VMPlayer.PlayerData = new();
            }
            catch (FileLoadException ex)
            {
                Console.WriteLine($"Unable to deserialize 'PlayerData.xml'. Reason: {ex.Message}\nUsing default initialisers instead...");
                App.ViewModel.VMPlayer.PlayerData = new();
            }
            
        }

        private void ExitCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args) => App.Current.Exit();

        #endregion

        public static async Task ShowAlert(string message)
        {
            ContentDialog alert = new();

            alert.Content = message;
            alert.CloseButtonText = "Okay";
            await alert.ShowAsync();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await src.FileHandler.Init();

            src.PlayerLookup.init();

            try
            {
                App.ViewModel.VMPlayer.PlayerData = await src.FileHandler.GetPlayerData();
                App.ViewModel.VMPlayer.PlayerData.SelfCheckup();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Unable to deserialize 'PlayerData.xml'. Reason: {ex.Message}\nUsing default initialisers instead...");
                App.ViewModel.VMPlayer.PlayerData = new();
            }
            catch (FileLoadException ex)
            {
                Console.WriteLine($"Unable to deserialize 'PlayerData.xml'. Reason: {ex.Message}\nUsing default initialisers instead...");
                App.ViewModel.VMPlayer.PlayerData = new();
            }

            if (App.ViewModel.VMQuests.QuestLog.Quests.Count == 0)
            {
                try
                {
                    App.ViewModel.VMQuests.QuestLog = await src.FileHandler.GetQuestLog();
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"Unable to deserialize 'QuestLog.xml'. Reason: {ex.Message}\nUsing default initialisers instead...");
                    App.ViewModel.VMQuests.QuestLog = new();
                }
                catch (FileLoadException ex)
                {
                    Console.WriteLine($"Unable to deserialize 'QuestLog.xml'. Reason: {ex.Message}\nUsing default initialisers instead...");
                    App.ViewModel.VMQuests.QuestLog = new();
                }
            }
            if (App.ViewModel.VMQuests.QuestLog.Quests.Count == 0)
            {
                App.ViewModel.VMQuests.QuestLog.CreateTestLog();
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
                return;

            MenuFlyoutItem item = (sender as MenuFlyoutItem);
            if (item is null)
                return;

            if (item.Text.Equals("Reset"))
            {
                App.ViewModel.VMPlayer.PlayerData = new();
                App.ViewModel.VMPlayer.PlayerData.SelfCheckup();
            }
        }
    }
}
