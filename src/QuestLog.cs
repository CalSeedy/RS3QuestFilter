using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RS3QuestFilter.src
{
    [XmlType("QuestLog"), XmlRoot("QuestLog")]
    public class QuestLog : INotifyPropertyChanged
    {
        [XmlIgnore]
        private ObservableCollection<Quest> quests;

        [XmlElement("Quests")]
        public ObservableCollection<Quest> Quests
        { 
            get 
            {
                if (quests == null)
                    quests = new();
                return quests;
            }
            set
            {
                if (quests != value)
                {
                    quests = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void AddQuest(Quest quest)
        {
            if (!QuestExists(quest))
                Quests.Add(quest);
            else
                Console.WriteLine($"Skipping quest: \"{quest.Title}\" already exists.");
        }

        public void Print()
        {
            string printed = "Quest Log:";

            foreach (Quest q in Quests)
            {
                printed += $"\n\t{q.Title}:";
                printed += "\n\t- Requirements";
                if (q.Requirements.Count > 0)
                {
                    foreach (Item i in q.Requirements)
                    {
                        printed += $"\n\t\t+ {i.Print()} (type = {Item.TypeToString(i.Type)})";
                    }
                }
                else {
                    printed += "\n\t\tNone";
                }



                printed += "\n\t- Rewards";
                if (q.Requirements.Count > 0)
                {
                    foreach (Item i in q.Rewards)
                    {
                        printed += $"\n\t\t+ {i.Print()} (type = {Item.TypeToString(i.Type)})";
                    }

                }
                else
                {
                    printed += "\n\t\tNone";
                }
                
                printed += $"\n\t- Member: {q.Member}";
                printed += $"\n\t- Difficulty: {Quest.DifficultyToString(q.Difficulty)}\n";
            }

            Console.WriteLine(printed);
        }

        private bool QuestExists(Quest quest)
        {
            foreach (Quest q in Quests)
            {
                if (q.Title.Equals(quest.Title))
                    return true;
            }
            return false;
        }

        public void CreateTestLog()
        {
            //Trace.WriteLine("Creating Quest Log...");
            {
                // Creating Biohazard
                Item rq = new("Plague City", 1, EType.Quest);
                Item rw1 = new("Quest Points", 3, EType.QP);
                Item rw2 = new("Thieving", 1250, EType.XP);

                ObservableCollection<Item> reqs = new();
                ObservableCollection<Item> rews = new();

                reqs.Add(rq);

                rews.Add(rw1);
                rews.Add(rw2);

                Quest q1 = new Quest("Biohazard", EDifficulty.Novice, true, reqs, rews);
                AddQuest(q1);
                // End of Biohazard
            }
            {
                // Creating Plague City
                Item rq1 = new("Dwellberries", 1, EType.Item);
                Item rq2 = new("Rope", 1, EType.Item);
                Item rq3 = new("Chocolate Dust", 1, EType.Item);
                Item rq4 = new("Snape Grass", 1, EType.Item);
                Item rq5 = new("Bucket of Milk", 1, EType.Item);

                Item rw1 = new("Quest Points", 1, EType.QP);
                Item rw2 = new("Mining", 2425, EType.XP);
                Item rw3 = new("Gas Mask", 1, EType.Item);

                ObservableCollection<Item> reqs = new();
                ObservableCollection<Item> rews = new();

                reqs.Add(rq1);
                reqs.Add(rq2);
                reqs.Add(rq3);
                reqs.Add(rq4);
                reqs.Add(rq5);

                rews.Add(rw1);
                rews.Add(rw2);
                rews.Add(rw3);

                Quest q2 = new("Plague City", EDifficulty.Novice, true, reqs, rews);
                AddQuest(q2);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
