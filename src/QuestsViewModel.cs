using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace RS3QuestFilter.src
{
    public class QuestsViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<Item> originalReqs { get; set; }
        private ObservableCollection<Item> originalRews { get; set; }

        private Quest selectedQuest;

        public Quest Selected
        {
            get
            {
                if (selectedQuest == null)
                    selectedQuest = new Quest();
                return selectedQuest;
            }
            set
            {
                if (selectedQuest != value)
                {
                    selectedQuest = value;
                }
            }
        }

        private QuestLog questLog;
        public QuestLog QuestLog
        {
            get 
            {
                if(questLog == null)
                    questLog = new();
                return questLog;
            }
            set
            {
                if (questLog != value)
                {
                    questLog = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool isCumulative;
        public bool IsCumulative
        {
            get 
            { 
                if (isCumulative == null)
                    isCumulative = false;
                return isCumulative;
            }
            set
            {
                if (value != isCumulative)
                {
                    isCumulative = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
