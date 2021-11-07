using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RS3QuestFilter.src
{
    public class PlayerDataViewModel : INotifyPropertyChanged
    {
        public static object dataContext;

        public PlayerDataViewModel() {
            dataContext = this;
        }


        private Player playerData = new();

        public Player PlayerData
        {
            get { return playerData; }
            set
            {
                if (!playerData.Equals(value))
                {
                    playerData = value;
                    NotifyPropertyChanged("PlayerData");
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
