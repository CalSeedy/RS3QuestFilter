using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public sealed partial class QuestListPage : Page
    {

        public readonly Array difficultySource = Enum.GetValues(typeof(EDifficulty));
        public readonly Array typeSource = Enum.GetValues(typeof(EType));

        public QuestListPage()
        {
            this.InitializeComponent();
            //App.ViewModel.VMQuests.Number = -1;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContext = null;
            DataContext = App.ViewModel.VMQuests;

            //App.ViewModel.VMQuests.Number = CalculateTotalQP();

            if (filterSwitch.IsOn)
                App.ViewModel.VMQuests.FilterQuests(true);

        }

        //private int CalculateTotalQP()
        //{
        //    int total = 0;
        //    foreach (Quest q in App.ViewModel.VMQuests.QuestLog.Quests)
        //    {
        //        int? a = q.Rewards.FirstOrDefault(x => x.Type == EType.QP)?.Amount;
        //        if (a.HasValue)
        //            total += a.Value;
        //    }

        //    return (total >= 0) ? total : -1;
        //}

        private void dgQuests_SelectionChanged(object sender, SelectionChangedEventArgs e) => App.ViewModel.VMQuests.DG_OnSelectionChange(sender);

        private void OnLoad()
        {
            if (DataContext == null)
                DataContext = App.ViewModel.VMQuests;
        }

        private void PageQuests_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            OnLoad();
        }

        private void DG_AddRow<T>(ObservableCollection<T> target) where T : class, new()
        {
            target.Add(new T());
        }

        private void DG_DelRow<T>(ObservableCollection<T> target, int idx) where T : class, new()
        {
            if (target.Count == 1)
            {
                target.Clear();
                return;
            }
            target.RemoveAt(idx);
        }

        private async void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;
            string tag = button.Tag as string;

            Microsoft.UI.Xaml.Controls.CommandBarFlyout flyout = null;
            if (tag != null)
            {
                switch (tag)
                {
                    case "questAdd":
                        flyout = questDGCommandFlyout;
                        goto case "questAddDef";

                    case "questAddEmpty":
                        flyout = emptyQuestsFlyout;
                        goto case "questAddDef";

                    case "questAddDef":
                        DG_AddRow(App.ViewModel.VMQuests.QuestLog.Quests);
                        dgQuests.SelectedIndex = App.ViewModel.VMQuests.QuestLog.Quests.Count - 1;
                        App.ViewModel.VMQuests.QuestLog.Quests[dgQuests.SelectedIndex].Requirements.Add(new());
                        App.ViewModel.VMQuests.QuestLog.Quests[dgQuests.SelectedIndex].Rewards.Add(new());
                        dgQuests.ScrollIntoView(App.ViewModel.VMQuests.QuestLog.Quests[dgQuests.SelectedIndex], null);
                        break;

                    case "questDel":
                        DG_DelRow(App.ViewModel.VMQuests.QuestLog.Quests, dgQuests.SelectedIndex);
                        flyout = questDGCommandFlyout;
                        break;

                    case "reqAdd":
                        flyout = reqsDGCommandFlyout;
                        goto case "reqAddDef";

                    case "reqAddEmpty":
                        flyout = emptyReqsFlyout;
                        goto case "reqAddDef";

                    case "reqAddDef":
                        if (dgQuests.SelectedItem is not null)
                        {
                            DG_AddRow((dgQuests.SelectedItem as Quest).Requirements);
                            if (dgReqs.SelectedItem as Item is not null)
                            {
                                dgReqs.SelectedIndex = (dgQuests.SelectedItem as Quest).Requirements.Count - 1;
                                dgReqs.ScrollIntoView((dgQuests.SelectedItem as Quest).Requirements[dgReqs.SelectedIndex], null);
                            }
                        }
                        else 
                        {
                            await MainPage.ShowAlert("You need to select a quest before you try to add a requirement!");
                        }
                        break;

                    case "reqDel":
                        DG_DelRow((dgQuests.SelectedItem as Quest).Requirements, dgReqs.SelectedIndex);
                        flyout = reqsDGCommandFlyout;
                        break;

                    case "rewAdd":
                        flyout = rewsDGCommandFlyout;
                        goto case "rewAddDef";
                    case "rewAddEmpty":
                        flyout = emptyRewsFlyout;
                        goto case "rewAddDef";

                    case "rewAddDef":
                        if (dgQuests.SelectedItem is not null)
                        {
                            DG_AddRow((dgQuests.SelectedItem as Quest).Rewards);
                            if (dgRews.SelectedItem is not null)
                            {
                                dgRews.SelectedIndex = (dgQuests.SelectedItem as Quest).Rewards.Count - 1;
                                dgRews.ScrollIntoView((dgQuests.SelectedItem as Quest).Rewards[dgRews.SelectedIndex], null);
                            }
                        }
                        else
                        {
                            await MainPage.ShowAlert("You need to select a quest before you try to add a reward!");
                        }
                        break;

                    case "rewDel":
                        DG_DelRow((dgQuests.SelectedItem as Quest).Rewards, dgRews.SelectedIndex);
                        flyout = rewsDGCommandFlyout;
                        break;
                }

                if (flyout != null)
                    flyout.Hide();
            }


        }

        private void cumulativeSwitch_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (dgQuests.SelectedItem != null)
            {
                if (App.ViewModel.VMQuests.IsCumulative)
                {
                    App.ViewModel.VMQuests.originalReqs = new((dgQuests.SelectedItem as src.Quest).Requirements);
                    App.ViewModel.VMQuests.originalRews = new((dgQuests.SelectedItem as src.Quest).Rewards);

                    App.ViewModel.VMQuests.CalculateCumulatives();
                }
                else
                {
                    (dgQuests.SelectedItem as src.Quest).Requirements = new(App.ViewModel.VMQuests.originalReqs);
                    (dgQuests.SelectedItem as src.Quest).Rewards = new(App.ViewModel.VMQuests.originalRews);
                }
            }
        }

        private void filterSwitch_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is null)
                return;

            ToggleSwitch ts = (sender as ToggleSwitch);

            App.ViewModel.VMQuests.FilterQuests(ts.IsOn);
        }

        private void dgQuests_Sorting(object sender, DataGridColumnEventArgs e)
        {
            if (e.Column.Tag.ToString() == "Title")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    App.ViewModel.VMQuests.QuestLog.Quests = new(App.ViewModel.VMQuests.QuestLog.Quests.OrderBy(x =>
                    x.Title.StartsWith("A ", StringComparison.OrdinalIgnoreCase) || x.Title.StartsWith("The ", StringComparison.OrdinalIgnoreCase) ?
                    x.Title.Substring(x.Title.IndexOf(" ") + 1) : x.Title));
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    App.ViewModel.VMQuests.QuestLog.Quests = new(App.ViewModel.VMQuests.QuestLog.Quests.OrderByDescending(x =>
                    x.Title.StartsWith("A ", StringComparison.OrdinalIgnoreCase) || x.Title.StartsWith("The ", StringComparison.OrdinalIgnoreCase) ?
                    x.Title.Substring(x.Title.IndexOf(" ") + 1) : x.Title));
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }

            foreach (var dgColumn in dgQuests.Columns)
            {
                if (dgColumn.Tag.ToString() != e.Column.Tag.ToString())
                {
                    dgColumn.SortDirection = null;
                }
            }
        }
    }

    public class EnumToArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value is not Enum)
                return null;

            return Enum.GetValues(targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value is not Enum)
                return null;

            var @enum = value as Enum;
            var description = @enum.ToString();

            var attrib = this.GetAttribute<DisplayAttribute>(@enum);
            if (attrib != null)
                description = attrib.Name;

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private T GetAttribute<T>(Enum enumValue) where T : Attribute
        {
            return enumValue.GetType().GetTypeInfo().GetDeclaredField(enumValue.ToString()).GetCustomAttribute<T>();
        }
    }

    public class StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
                return "Error";

            if (parameter is null)
                return (string)value;

            return String.Format((string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
