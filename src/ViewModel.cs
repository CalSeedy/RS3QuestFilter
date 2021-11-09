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
    public class ViewModel
    {
        private static bool isQuestPage = false;
        public static bool IsQuestPage
        {
            get
            {
                return isQuestPage;
            }
            set
            {
                if (value != isQuestPage)
                    isQuestPage = value;
            }
        }

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

    }
}
