using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DrWPF.Windows.Data;

namespace RS3QuestFilter.src
{
    [Flags]
    public enum PlayerFlags
    {
        Standard = 0,
        Ironman = 1,
        Hardcore = 2,
        Osaat = 4,
        Skiller = 8,
        Member = 16
    }

    public class Player : INotifyPropertyChanged
    {
        private string name { get; set; }

        [XmlAttribute]
        public string Name
        {
            get 
            {
                if (name == null)
                    name = "Unknown";
                return name;
            }
            set
            {
                if (value != null)
                {
                    if (value != name)
                    {
                        name = value;
                        NotifyPropertyChanged();
                    }
                }
            }
        }

        [XmlAttribute("PlayerFlags")]
        private PlayerFlags flags { get; set; }
        public PlayerFlags Flags 
        {
            get
            {
                if (flags == null)
                    flags = new();
                return flags;
            }
            set
            {
                if (value != flags)
                {
                    flags = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [XmlElement("Skills", IsNullable = false)]
        public SerializableDictionary<string, Skill> serialisableSkills { get; set; }

        [XmlIgnore]
        private ObservableDictionary<string, Skill> skills { get; set; }
        
        [XmlIgnore]
        public ObservableDictionary<string, Skill> Skills 
        {
            get
            {
                if (skills == null)
                    skills = new();
                return skills;
            }
            set
            {
                if (value != skills)
                {
                    skills = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableDictionary<string, Skill> CreateSkills()
        {
            ObservableDictionary<string, Skill> skills = new();
            skills.Add("Agility", new("Agility", 1, false));
            skills.Add("Archaeology", new("Archaeology", 1, false));
            skills.Add("Attack", new("Attack", 1, false));
            skills.Add("Constitution", new("Constitution", 1, false));
            skills.Add("Construction", new("Construction", 1, false));
            skills.Add("Cooking", new("Cooking", 1, false));
            skills.Add("Crafting", new("Crafting", 1, false));
            skills.Add("Defence", new("Defence", 1, false));
            skills.Add("Divination", new("Divination", 1, false));
            skills.Add("Dungeoneering", new("Dungeoneering", 1, false));
            skills.Add("Farming", new("Farming", 1, false));
            skills.Add("Firemaking", new("Firemaking", 1, false));
            skills.Add("Fishing", new("Fishing", 1, false));
            skills.Add("Fletching", new("Fletching", 1, false));
            skills.Add("Herblore", new("Herblore", 1, false));
            skills.Add("Hunter", new("Hunter", 1, false));
            skills.Add("Invention", new("Invention", 1, false));
            skills.Add("Magic", new("Magic", 1, false));
            skills.Add("Mining", new("Mining", 1, false));
            skills.Add("Prayer", new("Prayer", 1, false));
            skills.Add("Ranged", new("Ranged", 1, false));
            skills.Add("Runecrafting", new("Runecrafting", 1, false));
            skills.Add("Slayer", new("Slayer", 1, false));
            skills.Add("Smithing", new("Smithing", 1, false));
            skills.Add("Strength", new("Strength", 1, false));
            skills.Add("Summoning", new("Summoning", 1, false));
            skills.Add("Thieving", new("Thieving", 1, false));
            skills.Add("Woodcutting", new("Woodcutting", 1, false));
            
            skills["Constitution"].Level = 10;

            return skills;
        }

        public void PrepareSkills()
        {
            Skills = new();
            foreach (string key in serialisableSkills.Keys)
            {
                Skills.Add(key, serialisableSkills[key]);
            }
        }

        public void PrepareSerialisable()
        {
            serialisableSkills = new();
            foreach (string key in skills.Keys)
            {
                serialisableSkills.Add(key, skills[key]);
            }
        }

        public Player()
        {
            Skills = CreateSkills();
            Name = "Unknown";
        }

        public Player(ObservableDictionary<string, Skill> skills, PlayerFlags flags)
        {
            Skills = skills;
            PrepareSerialisable();
            Flags = flags;
            Name = "Unknown";
        }

        public Player(Player player)
        {
            Skills = new(player.Skills);
            PrepareSerialisable();
            Flags = player.Flags;
            Name = player.Name;
        }

        public Player(SerializableDictionary<string, Skill> skills, PlayerFlags flags)
        {
            Skills = new();
            PrepareSkills();
            Flags = flags;
            Name = "Unknown";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class Skill : INotifyPropertyChanged
    {
        [XmlAttribute("Name")]
        private string name { get; set; }

        public string Name { get { return name; } }

        [XmlAttribute("Level")]
        private int level { get; set; }
        public int Level 
        {
            get
            {
               return level;
            }
            set
            {
                if (value != null && value >= 0)
                {
                    level = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [XmlAttribute("Enabled")]
        private bool enabled { get; set; }
        public bool Enabled 
        {
            get
            {
                return enabled;
            }
            set
            {
                if (value != enabled)
                {
                    enabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Skill(string name, int level, bool enabled)
        {
            this.name = name;
            Level = level;
            Enabled = enabled;
        }

        public Skill(Skill skill)
        {
            this.name = skill.Name;
            Level = skill.Level;
            Enabled = skill.Enabled;
        }

        public Skill()
        {
            this.name = "Unknown Skill";
            Level = 0;
            Enabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
