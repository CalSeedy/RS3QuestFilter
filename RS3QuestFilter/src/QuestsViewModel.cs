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

        private QuestLog questLog;
        public QuestLog QLog
        {
            get 
            {
                if(questLog == null)
                    questLog = new();
                return questLog;
            }
            set
            {
                if (!questLog.Equals(value))
                {
                    questLog = value;
                    NotifyPropertyChanged("QLog");
                }
            }
        }

        public ObservableCollection<Quest> QuestList
        {
            get
            {
                if (questLog == null)
                    questLog = new();
                return questLog.Quests;
            }
            set
            {
                if (!questLog.Quests.Equals(value))
                {
                    questLog.Quests = value;
                    Console.WriteLine(value);
                    NotifyPropertyChanged("QuestList");
                    NotifyPropertyChanged("QLog");
                }
            }
        }

        private bool isCumulative;
        public bool IsCumulative
        {
            get { return isCumulative; }
            set
            {
                if (value != isCumulative)
                {
                    isCumulative = value;
                    NotifyPropertyChanged("IsCumulative");
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
