using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        Skiller = 8
    }

    public class Player
    {
        [XmlAttribute("PlayerFlags")]
        public PlayerFlags Flags { get; set; }

        [XmlElement("Skills", IsNullable = false)]
        public SerializableDictionary<string, Skill> serialisableSkills { get; set; }

        [XmlIgnore]
        public ObservableDictionary<string, Skill> Skills { get; set; }

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

        private void PrepareSkills()
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
            foreach (string key in Skills.Keys)
            {
                serialisableSkills.Add(key, Skills[key]);
            }
        }

        public Player()
        {
            Skills = CreateSkills();
            PrepareSerialisable();
        }

        public Player(ObservableDictionary<string, Skill> skills, PlayerFlags flags)
        {
            Skills = skills;
            PrepareSerialisable();
            Flags = flags;
        }

        public Player(Player player)
        {
            Skills = new(player.Skills);
            PrepareSerialisable();
            Flags = player.Flags;
        }

        public Player(SerializableDictionary<string, Skill> skills, PlayerFlags flags)
        {
            Skills = new();
            PrepareSkills();
            Flags = flags;
        }

    }

    public class Skill
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Level")]
        public int Level { get; set; }

        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }

        public Skill(string name, int level, bool enabled)
        {
            Name = name;
            Level = level;
            Enabled = enabled;
        }

        public Skill(Skill skill)
        {
            Name = skill.Name;
            Level = skill.Level;
            Enabled = skill.Enabled;
        }

        public Skill()
        {
            Name = "Unknown Skill";
            Level = 0;
            Enabled = false;
        }
    }
}
