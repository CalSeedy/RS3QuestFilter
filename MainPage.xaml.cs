using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
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
        private NavigationViewItem m_PreviousView;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void NavigationView_OnItemInvoked(
            Windows.UI.Xaml.Controls.NavigationView sender,
            NavigationViewItemInvokedEventArgs args)
        {
            var item = args.InvokedItemContainer as NavigationViewItem;
            if (item == null || item == m_PreviousView)
                return;
            var clickedView = item.Tag?.ToString() ?? "SettingsView";
            if (!NavigateToView(clickedView)) return;
            m_PreviousView = item;
        }

        private bool NavigateToView(string clickedView)
        {
            var view = Assembly.GetExecutingAssembly()
                .GetType($"NavigationView.Views.{clickedView}");

            if (string.IsNullOrWhiteSpace(clickedView) || view == null)
            {
                return false;
            }

            ContentFrame.Navigate(view, null, new EntranceNavigationTransitionInfo());
            return true;
        }

        private void ContentFrame_OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new NavigationException(
                $"Navigation failed {e.Exception.Message} for {e.SourcePageType.FullName}");
        }

        private void NavView_OnBackRequested(
            Windows.UI.Xaml.Controls.NavigationView sender,
            NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack)
                ContentFrame.GoBack();
        }

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

        private void Contentframe_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {

        }

        public static void ShowAlert(string message)
        {
            ContentDialog alert = new();

            alert.Content = message;
            alert.CloseButtonText = "Okay";
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            ;
        }
    }

    internal class NavigationException : Exception
    {
        public NavigationException(string msg) : base(msg)
        {

        }
    }
}
