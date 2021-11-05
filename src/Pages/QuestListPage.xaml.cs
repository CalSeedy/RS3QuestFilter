using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
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
        private ObservableCollection<Item>? originalReqs, originalRews;

        private Quest? selectedQuest;

        public readonly Array difficultySource = Enum.GetValues(typeof(EDifficulty));

        public QuestListPage()
        {
            this.InitializeComponent();

            selectedQuest = null;
            originalReqs = null;
            originalRews = null;


            OnLoad();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContext = null;
            DataContext = App.ViewModel.VMQuests;
        }

        private void dgQuests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void OnLoad()
        {
            try
            {
                if (DataContext == null)
                    DataContext = App.ViewModel.VMQuests;

                (DataContext as QuestsViewModel).QLog = await FileHandler.GetQuestLog();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Unable to deserialize 'QuestLog.xml'. Reason: {e.Message}\nUsing default initialisers instead...");
                (DataContext as QuestsViewModel).QLog.Quests = new();
                dgQuests.ItemsSource = (DataContext as QuestsViewModel).QLog.Quests;
            }
            CreateTestLog();
        }

        public void CreateTestLog()
        {
            //Trace.WriteLine("Creating Quest Log...");
            {
                // Creating Biohazard
                Item rq = new("Plague City", 1, EType.Quest);
                Item rw1 = new("Quest Points", 3, EType.QP);
                Item rw2 = new("Thieving", 1250, EType.XP);

                ObservableCollection<Item> reqs = new();
                ObservableCollection<Item> rews = new();

                reqs.Add(rq);

                rews.Add(rw1);
                rews.Add(rw2);

                Quest q1 = new Quest("Biohazard", EDifficulty.Novice, true, reqs, rews);
                (DataContext as QuestsViewModel).QLog.AddQuest(q1);
                // End of Biohazard
            }
            {
                // Creating Plague City
                Item rq1 = new("Dwellberries", 1, EType.Item);
                Item rq2 = new("Rope", 1, EType.Item);
                Item rq3 = new("Chocolate Dust", 1, EType.Item);
                Item rq4 = new("Snape Grass", 1, EType.Item);
                Item rq5 = new("Bucket of Milk", 1, EType.Item);

                Item rw1 = new("Quest Points", 1, EType.QP);
                Item rw2 = new("Mining", 2425, EType.XP);
                Item rw3 = new("Gas Mask", 1, EType.Item);

                ObservableCollection<Item> reqs = new();
                ObservableCollection<Item> rews = new();

                reqs.Add(rq1);
                reqs.Add(rq2);
                reqs.Add(rq3);
                reqs.Add(rq4);
                reqs.Add(rq5);

                rews.Add(rw1);
                rews.Add(rw2);
                rews.Add(rw3);

                Quest q2 = new("Plague City", EDifficulty.Novice, true, reqs, rews);
                (DataContext as QuestsViewModel).QLog.AddQuest(q2);
            }
        }
    }

    public class EnumToArrayConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is Enum))
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
            if (value == null || !(value is Enum))
                return null;

            var @enum = value as Enum;
            var description = @enum.ToString();

            var attrib = this.GetAttribute<DisplayAttribute>(@enum);
            if (attrib != null)
                description = attrib.Name;

            Console.WriteLine(description);

            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            throw new NotImplementedException();
        }

        private T GetAttribute<T>(Enum enumValue) where T : Attribute
        {
            return enumValue.GetType().GetTypeInfo().GetDeclaredField(enumValue.ToString()).GetCustomAttribute<T>();
        }
    }

}
