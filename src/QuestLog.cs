﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace RS3QuestFilter.src
{
    [XmlType("QuestLog"), XmlRoot("QuestLog")]
    public class QuestLog
    {
        [XmlElement("Quests")]
        public ObservableCollection<Quest> Quests = new();

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

    }
}
