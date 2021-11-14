using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace RS3QuestFilter.src
{
    public class QuestsViewModel : Editable
    {
        private bool isCumulative { get; set; }

        public ObservableCollection<Item> originalReqs { get; set; }
        public ObservableCollection<Item> originalRews { get; set; }

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

        private QuestLog originalQuestLog;
        private QuestLog questLog;
        public QuestLog QuestLog
        {
            get
            {
                if (questLog == null)
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
                return isCumulative;
            }
            set
            {
                if (isCumulative != value)
                {
                    isCumulative = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool isFiltered;
        public bool IsFiltered
        {
            get
            {
                return isFiltered;
            }
            set
            {
                if (isFiltered != value)
                {
                    isFiltered = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool isIron { get { return App.ViewModel.VMPlayer.PlayerData.Flags.HasFlag(PlayerFlags.Ironman); } }
        private bool isHardcore { get { return App.ViewModel.VMPlayer.PlayerData.Flags.HasFlag(PlayerFlags.Hardcore); } }
        private bool isOsat { get { return App.ViewModel.VMPlayer.PlayerData.Flags.HasFlag(PlayerFlags.Osaat); } }
        private bool isSkiller { get { return App.ViewModel.VMPlayer.PlayerData.Flags.HasFlag(PlayerFlags.Skiller); } }
        private bool isMember { get { return App.ViewModel.VMPlayer.PlayerData.Flags.HasFlag(PlayerFlags.Member); } }
        private bool canCombat { get { return App.ViewModel.VMPlayer.PlayerData.Skills["Constitution"].Enabled && App.ViewModel.VMPlayer.PlayerData.Skills["Magic"].Enabled; } }


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

        public void CalculateCumulatives()
        {
            Debug.WriteLine("Calculating now...");

            ObservableCollection<src.Item> reqs = new();
            ObservableCollection<src.Item> rews = new();
            src.Quest quest = new(Selected);
            ObservableCollection<src.Quest> searchedQuests = new();

            (reqs, rews) = ScanQuestForItems(reqs, rews, quest, searchedQuests);

            Selected.Requirements = reqs;
            Selected.Rewards = rews;
        }

        private void RestoreQuestDataAndResetOriginals()
        {
            if (selectedQuest == null)
                throw new NullReferenceException("Cannot restore quest data when there is no selection!");

            Trace.WriteLine($"Finding quest: {selectedQuest.Title}");
            var quest = QuestLog.Quests.FirstOrDefault(q => q.Title.Equals(selectedQuest.Title));

            if (quest != null)
            {
                Trace.WriteLine("Found quest! Resetting values..");
                quest.Requirements = originalReqs;
                quest.Rewards = originalRews;
                Selected = null;
                originalReqs = new();
                originalRews = new();
            }
            else
            {
                Trace.WriteLine("Could not find quest! That's strange...");
            }
        }

        public void DG_OnSelectionChange(object sender)
        {
            if (!isCumulative)
            {
                if ((sender as DataGrid).SelectedItem == null)
                    App.ViewModel.IsQuestSelected = false;
                else
                    App.ViewModel.IsQuestSelected = true;
            }
            App.ViewModel.IsSubDatagridEditable = App.ViewModel.IsQuestSelected && App.ViewModel.IsQuestPage && !isCumulative;

            DataGrid dg = (DataGrid)sender;

            bool oldSelection = Selected != null;
            bool newSelection = dg.SelectedItem != null;
            if (Selected == (dg.SelectedItem as src.Quest))
            {
                return;
            }

            Trace.WriteLine($"Old: {oldSelection}\nNew: {newSelection}\nCumulative: {IsCumulative}");

            if (oldSelection)
                RestoreQuestDataAndResetOriginals();


            if (newSelection)
            {
                Selected = (src.Quest)dg.SelectedItem;
                originalReqs = Selected.Requirements;
                originalRews = Selected.Rewards;

                if (IsCumulative)
                    CalculateCumulatives();
            }
            else
            {
                Selected = null;
                originalReqs = null;
                originalRews = null;
            }
        }

        public void FilterQuests(bool shouldFilter = true)
        {
            if (shouldFilter)
            {
                if (originalQuestLog is null)
                {
                    originalQuestLog = new();
                    originalQuestLog.Quests = new(QuestLog.Quests);
                }
                else
                {
                    QuestLog.Quests = new(originalQuestLog.Quests);
                }
                QuestLog.Quests = new((QuestLog.Quests.ToList()).Where(QueryQuest).ToList());
            }
            else
            {
                if (originalQuestLog != null)
                    QuestLog.Quests = originalQuestLog.Quests;
            }
        }

        private bool QueryQuest(Quest quest)
        {

            if (!isMember && (quest.Member ?? false))
                return false;

            if (isOsat)
            {
                bool reqsGood = quest.Requirements.All(req =>
                {
                    switch (req.Type)
                    {
                        case EType.Level:
                            return (req.Amount <= App.ViewModel.VMPlayer.PlayerData.Skills[req.Name].Level) && App.ViewModel.VMPlayer.PlayerData.Skills[req.Name].Enabled;

                        case EType.Combat:
                            return canCombat;

                        case EType.Quest:
                            return QueryQuest(questLog.Quests.FirstOrDefault(q => q.Title.Equals(req.Name)));

                    }
                    return true;
                });

                bool rewsGood = quest.Rewards.All(rew =>
                {
                    if (rew.Type == EType.Level)
                        return App.ViewModel.VMPlayer.PlayerData.Skills[rew.Name].Enabled;
                    
                    return true;
                });

                if (rewsGood && reqsGood)
                    return true;
                
                return false;
            }

            if (isSkiller)
            {
                bool reqsGood = quest.Requirements.All(req =>
                {
                    switch (req.Type)
                    {
                        case EType.Level:
                            {
                                bool x = (req.Amount <= App.ViewModel.VMPlayer.PlayerData.Skills[req.Name].Level) && App.ViewModel.VMPlayer.PlayerData.Skills[req.Name].Enabled;
                                bool y = req.Name == "Attack" || req.Name == "Constitution" || req.Name == "Strength" || req.Name == "Defence" || req.Name == "Prayer" || req.Name == "Summoning" || req.Name == "Ranged" || req.Name == "Magic";
                                return x && !y;
                            }
                        case EType.Combat:
                            return false;

                        case EType.Quest:
                            return QueryQuest(questLog.Quests.FirstOrDefault(q => q.Title.Equals(req.Name)));
                    }
                    return true;
                });

                bool rewsGood = quest.Rewards.All(rew =>
                {
                    if (rew.Type == EType.Level)
                    {
                        bool y = rew.Name == "Attack" || rew.Name == "Constitution" || rew.Name == "Strength" || rew.Name == "Defence" || rew.Name == "Prayer" || rew.Name == "Summoning" || rew.Name == "Ranged" || rew.Name == "Magic";
                        return !y;
                    }
                    
                    return true;
                });

                if (rewsGood && reqsGood)
                    return true;

                return false;
            }
            return true;
        }
    }
}
