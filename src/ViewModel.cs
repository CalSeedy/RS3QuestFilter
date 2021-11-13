using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RS3QuestFilter.src
{
    public class ViewModel : Editable
    {
        private QuestsViewModel quests;
        public QuestsViewModel VMQuests
        { 
            get 
            {
                if (quests == null)
                    quests = new();
                return quests;
            } 
        }

        private PlayerDataViewModel player;
        public PlayerDataViewModel VMPlayer
        {
            get
            {
                if (player == null)
                    player = new();
                return player;
            }
        }

        public bool IsQuestPage
        {
            get { return State.isQuestPage; }
            set { State.isQuestPage = value; NotifyPropertyChanged(); }
        }

        public bool IsQuestSelected
        {
            get { return State.isQuestSelected; }
            set { State.isQuestSelected = value; NotifyPropertyChanged(); }
        }

        public bool IsSubDatagridEditable
        {
            get
            {
                return State.isQuestPage && State.isQuestSelected;
            }
            set { State.isQuestPage = value; NotifyPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class State
    {
        public static bool isQuestPage = false;
        public static bool isQuestSelected = false;
    }
}
