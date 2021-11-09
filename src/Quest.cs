﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace RS3QuestFilter.src
{
    public enum EDifficulty
    {
        [Display("Miniquest")]
        Miniquest = 0,
        [Display("Novice")]
        Novice = 1,
        [Display("Intermediate")]
        Intermediate = 2,
        [Display("Experienced")]
        Experienced = 3,
        [Display("Master")]
        Master = 4,
        [Display("Grandmaster")]
        Grandmaster = 5
    }

    [XmlRoot("Quest"), XmlType("Quest")]
    public class Quest : INotifyPropertyChanged
    {
        private string title;
        [XmlAttribute]
        public string Title
        {
            get { return title; }

            set
            {
                if (value != null)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private EDifficulty difficulty;
        [XmlAttribute]
        public EDifficulty Difficulty
        {
            get { return difficulty; }

            set
            {
                difficulty = value;
                NotifyPropertyChanged("Difficulty");

            }
        }

        private bool? member;
        [XmlElement(IsNullable = true)]
        public bool? Member
        {
            get { return member; }

            set
            {
                member = value;
                NotifyPropertyChanged("Member");
            }
        }

        private ObservableCollection<Item> requirements;
        [XmlElement]
        public ObservableCollection<Item> Requirements
        {
            get { return requirements; }

            set
            {
                if (value != null)
                {
                    requirements = value;
                    NotifyPropertyChanged("Requirements");
                }
            }
        }

        private ObservableCollection<Item> rewards;
        [XmlElement]
        public ObservableCollection<Item> Rewards
        {
            get { return rewards; }

            set
            {
                if (value != null)
                {
                    rewards = value;
                    NotifyPropertyChanged("Rewards");
                }
            }
        }

        private bool? completed;
        [XmlElement(IsNullable = true)]
        public bool? Completed
        {
            get { return completed; }

            set
            {
                completed = value;
                NotifyPropertyChanged("Completed");
            }
        }

        public Quest(Quest q)
        {
            Title = q.Title;
            Difficulty = q.Difficulty;
            Member = q.Member;
            Requirements = q.Requirements;
            Rewards = q.Rewards;
        }

        public Quest(string title, EDifficulty difficulty, bool member, ObservableCollection<Item> reqs, ObservableCollection<Item> rewards)
        {
            Title = title;
            Difficulty = difficulty;
            Member = member;
            Requirements = reqs;
            Rewards = rewards;
        }

        public Quest()
        {
            Title = "";
            Difficulty = EDifficulty.Novice;
            Requirements = new ObservableCollection<Item>();
            Rewards = new ObservableCollection<Item>();
            Member = null;
        }

        public static string DifficultyToString(EDifficulty diff)
        {
            switch (diff)
            {
                case EDifficulty.Miniquest:
                    return "Miniquest";
                case EDifficulty.Novice:
                    return "Novice";
                case EDifficulty.Intermediate:
                    return "Intermediate";
                case EDifficulty.Experienced:
                    return "Experienced";
                case EDifficulty.Master:
                    return "Master";
                case EDifficulty.Grandmaster:
                    return "Grandmaster";
                default:
                    throw new ArgumentException("Invalid Difficulty.");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}