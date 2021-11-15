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
        private static List<string> cbt = new List<string> { "Attack", "Strength", "Defence", "Constitution", "Ranged", "Prayer", "Magic", "Summoning" };

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


        private (ObservableCollection<Item>, ObservableCollection<Item>) ScanQuestForItems(ObservableCollection<Item> requirements, ObservableCollection<Item> rewards, Quest quest, ObservableCollection<Quest> searched)
        {
            Debug.WriteLine($"Scanning Quest: {quest.Title}\n# of Requirements: {quest.Requirements.Count}\n# of Rewards: {quest.Rewards.Count}");

            ObservableCollection<Item> currentReqs = new(quest.Requirements);
            //foreach (Item i in quest.Requirements) currentReqs.Add(i);

            ObservableCollection<Item> currentRews = new(quest.Rewards);
            //foreach (Item i in quest.Rewards) currentRews.Add(i);

            ObservableCollection<Quest> questQueue = new();

            foreach (Item i in currentReqs)
            {
                var found = requirements.FirstOrDefault(it => it.Equals(i));
                if (found == null)                                                                          // if we didnt find item (from current quest) in list of all items
                {
                    if (i.Type == EType.Quest)                                                              // and if the item is a Quest
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
                        case EType.Quest:
                            break;
                        case EType.Level:
                        case EType.Item:
                        case EType.QP:
                        case EType.XP:
                        case EType.None:
                        case EType.Literal:
                            {
                                found.Amount += i.Amount;   // just add the amounts
                            }
                            break;
                        case EType.Lamp:
                            break;
                    }
                }
            }

            foreach (Item i in currentRews)
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
                        case EType.Quest:
                            break;
                        case EType.XP:
                            {
                                if (i.Name.StartsWith("Choice", StringComparison.CurrentCultureIgnoreCase))
                                    rewards.Add(new(i));
                                else
                                    found.Amount += i.Amount;
                            }
                            break;
                        case EType.Level:
                        case EType.Item:
                        case EType.QP:
                        case EType.None:
                        case EType.Literal:
                            {
                                found.Amount += i.Amount;   // just add the amounts
                            }
                            break;
                        case EType.Lamp:
                            rewards.Add(i);
                            break;
                    }
                }
            }

            foreach (Quest q in questQueue)
            {
                (requirements, rewards) = ScanQuestForItems(requirements, rewards, q, searched);
            }


            return (requirements, rewards);
        }

        public void CalculateCumulatives()
        {
            Debug.WriteLine("Calculating now...");

            ObservableCollection<Item> reqs = new();
            ObservableCollection<Item> rews = new();
            Quest quest = new(Selected);
            ObservableCollection<Quest> searchedQuests = new();

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
            if (Selected == (dg.SelectedItem as Quest))
            {
                return;
            }

            Trace.WriteLine($"Old: {oldSelection}\nNew: {newSelection}\nCumulative: {IsCumulative}");

            if (oldSelection)
                RestoreQuestDataAndResetOriginals();


            if (newSelection)
            {
                Selected = (Quest)dg.SelectedItem;
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
                    if (rew.Type == EType.XP)

                        if (rew.Name.StartsWith("Choice"))
                        {
                            string s = rew.Name.Substring(7);
                            string[] split = s.Split(','); foreach (string str in split) str.Trim();
                            return App.ViewModel.VMPlayer.PlayerData.Skills.Any(x => split.Contains(x.Key) && x.Value.Enabled);
                        }
                        else 
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
                                return x && !cbt.Contains(req.Name);
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
                    if (rew.Type == EType.XP)
                    {
                        if (rew.Name.StartsWith("Choice"))
                        {
                            string s = rew.Name.Substring(7);
                            List<string> split = s.Split(',').ToList(); foreach (string str in split) str.Trim();
                            
                            return !split.Intersect(cbt).Any();
                        }
                        return !cbt.Contains(rew.Name);
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
