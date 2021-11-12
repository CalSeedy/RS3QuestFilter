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
        private Player playerData { get; set; }

        public Player PlayerData
        {
            get 
            {
                if (playerData == null)
                    playerData = new Player();
                return playerData;
            }
            set
            {
                if (playerData != value)
                {
                    playerData = value;
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
