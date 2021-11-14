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
        Member = 16,
        AreaLocked = 32 // suggested by: "88warm2396"
    }

    public class Player : Editable
    {
        private string name { get; set; }

        [XmlAttribute]
        public string Name
        {
            get 
            {
                if (name == null)
                    name = "";
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
            ObservableDictionary<string, Skill> tmp = new();
            tmp.Add("Agility", new("Agility", 1, false));
            tmp.Add("Archaeology", new("Archaeology", 1, false));
            tmp.Add("Attack", new("Attack", 1, false));
            tmp.Add("Constitution", new("Constitution", 10, false));
            tmp.Add("Construction", new("Construction", 1, false));
            tmp.Add("Cooking", new("Cooking", 1, false));
            tmp.Add("Crafting", new("Crafting", 1, false));
            tmp.Add("Defence", new("Defence", 1, false));
            tmp.Add("Divination", new("Divination", 1, false));
            tmp.Add("Dungeoneering", new("Dungeoneering", 1, false));
            tmp.Add("Farming", new("Farming", 1, false));
            tmp.Add("Firemaking", new("Firemaking", 1, false));
            tmp.Add("Fishing", new("Fishing", 1, false));
            tmp.Add("Fletching", new("Fletching", 1, false));
            tmp.Add("Herblore", new("Herblore", 1, false));
            tmp.Add("Hunter", new("Hunter", 1, false));
            tmp.Add("Invention", new("Invention", 1, false));
            tmp.Add("Magic", new("Magic", 1, false));
            tmp.Add("Mining", new("Mining", 1, false));
            tmp.Add("Prayer", new("Prayer", 1, false));
            tmp.Add("Ranged", new("Ranged", 1, false));
            tmp.Add("Runecrafting", new("Runecrafting", 1, false));
            tmp.Add("Slayer", new("Slayer", 1, false));
            tmp.Add("Smithing", new("Smithing", 1, false));
            tmp.Add("Strength", new("Strength", 1, false));
            tmp.Add("Summoning", new("Summoning", 1, false));
            tmp.Add("Thieving", new("Thieving", 1, false));
            tmp.Add("Woodcutting", new("Woodcutting", 1, false));

            return tmp;
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

        public void SelfCheckup()
        {
            foreach (string k in Skills.Keys)
            {
                if (Skills[k].Name == "Unknown Skill")
                {
                    Skills[k].Name = k;
                }
            }
        }

        public Player()
        {
            Skills = CreateSkills();
            Name = "";
        }

        public Player(ObservableDictionary<string, Skill> skills, PlayerFlags flags)
        {
            Skills = skills;
            PrepareSerialisable();
            Flags = flags;
            Name = "";
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
            Name = "";
        }

    }

    public class Skill : Editable
    {
        [XmlAttribute("Name")]
        private string name { get; set; }

        public string Name { set { name = value ?? "Unknown Skill"; }  get { return name; } }

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
    }
}
