using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace RS3QuestFilter.src
{
    public class QuestsViewModel : INotifyPropertyChanged
    {
        private bool isCumulative { get; set; }
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

#if false
        private (ObservableCollection<src.Item>, ObservableCollection<src.Item>) ScanQuestForItems(ObservableCollection<src.Item> requirements, ObservableCollection<src.Item> rewards, src.Quest quest, ObservableCollection<src.Quest> searched)
        {
            Debug.WriteLine($"Scanning Quest: {quest.Title}\n# of Requirements: {quest.Requirements.Count}\n# of Rewards: {quest.Rewards.Count}");

            ObservableCollection<src.Item> currentReqs = new(quest.Requirements);
            //foreach (src.Item i in quest.Requirements) currentReqs.Add(i);

            ObservableCollection<src.Item> currentRews = new(quest.Rewards);
            //foreach (src.Item i in quest.Rewards) currentRews.Add(i);

            ObservableCollection<src.Quest> questQueue = new();

            foreach (src.Item i in currentReqs)
            {
                var found = requirements.FirstOrDefault(it => it.Equals(i));
                if (found == null)                                                                          // if we didnt find item (from current quest) in list of all items
                {
                    if (i.Type == src.EType.Quest)                                                              // and if the item is a Quest
                    {
                        Debug.WriteLine("Found quest requirement!");

                        var questObj = questLog.Quests.FirstOrDefault(q => q.Title.Equals(i.Name));
                        if (questObj != null)                                                                        // and if the quest has been created/ exists in the db
                            questQueue.Add(new(questObj));
                        else
                            throw new NotImplementedException($"Quest '{i.Name}' has not been created yet!");
                    }

                    requirements.Add(i);
                }
                else    // if we do have a similar item
                {
                    switch (i.Type)
                    {
                        case src.EType.Quest:
                            break;
                        case src.EType.Level:
                        case src.EType.Item:
                        case src.EType.QP:
                        case src.EType.XP:
                        case src.EType.None:
                        case src.EType.Literal:
                            {
                                found.Amount += i.Amount;   // just add the amounts
                            }
                            break;
                        case src.EType.Lamp:
                            break;
                    }
                }
            }

            foreach (src.Item i in currentRews)
            {
                var found = rewards.FirstOrDefault(it => it.Equals(i));
                if (found == null)                                                                          // if we didnt find item (from current quest) in list of all items
                {
                    rewards.Add(new(i));
                }
                else    // if we do have a similar item
                {
                    switch (i.Type)
                    {
                        case src.EType.Quest:
                            break;
                        case src.EType.Level:
                        case src.EType.Item:
                        case src.EType.QP:
                        case src.EType.XP:
                        case src.EType.None:
                        case src.EType.Literal:
                            {
                                found.Amount += i.Amount;   // just add the amounts
                            }
                            break;
                        case src.EType.Lamp:
                            rewards.Add(i);
                            break;
                    }
                }
            }

            foreach (src.Quest q in questQueue)
            {
                (requirements, rewards) = ScanQuestForItems(requirements, rewards, q, searched);
            }


            return (requirements, rewards);
        }

        private void CalculateCumulatives()
        {
            Debug.WriteLine("Calculating now...");

            ObservableCollection<src.Item> reqs = new();
            ObservableCollection<src.Item> rews = new();
            src.Quest quest = new(selectedQuest);
            ObservableCollection<src.Quest> searchedQuests = new();

            (reqs, rews) = ScanQuestForItems(reqs, rews, quest, searchedQuests);

            selectedQuest.Requirements = reqs;
            selectedQuest.Rewards = rews;
        }
#endif



        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
