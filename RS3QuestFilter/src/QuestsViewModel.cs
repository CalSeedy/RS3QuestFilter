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

        private ObservableCollection<Item>? originalReqs = null, originalRews = null;

        private Quest? selectedQuest = null;


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
