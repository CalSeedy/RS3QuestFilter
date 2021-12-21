using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace RS3QuestFilter.src
{
    public enum EType
    {
        Default,
        Area,
        Combat,
        Item,
        Lamp,
        Level,
        Literal,
        QP,
        Quest,
        XP
    }

    public class Item : Editable, IEquatable<Item>
    {
        [XmlIgnore]
        private string name;

        [XmlAttribute]
        public string Name
        {
            get { return name; }

            set {
                if (value != null) {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [XmlIgnore]
        private int amount;
        [XmlElement]
        public int Amount
        {
            get { return amount; }

            set
            {
                amount = value;
                NotifyPropertyChanged();

            }
        }
        [XmlIgnore]
        private EType type;
        [XmlAttribute]
        public EType Type
        {
            get { return type; }

            set
            {
                type = value;
                NotifyPropertyChanged();
            }
        }
        [XmlIgnore]
        private bool enabled;
        [XmlAttribute]
        public bool Enabled
        {
            get { return enabled; }

            set
            {
                enabled = value;
                NotifyPropertyChanged();
            }
        }

        public Item(Item it)
        {
            Name = it.name;
            Amount = it.amount;
            Type = it.type;
            Enabled = it.Enabled;
        }

        public Item(string name, int amount, EType type, bool enabled = false)
        {
            Name = name;
            Amount = amount;
            Type = type;
            Enabled = enabled;
        }

        public Item()
        {
            Name = "";
            Amount = 0;
            Type = EType.Default;
            Enabled = false;
        }

        public static string TypeToString(EType type)
        {
            switch (type)
            {
                case EType.Area:
                    return "Area";
                case EType.Combat:
                    return "Combat";
                case EType.Item:
                    return "Item";
                case EType.Lamp:
                    return "Lamp";
                case EType.Level:
                    return "Level";
                case EType.Literal:
                    return "Literal";
                case EType.QP:
                    return "QP";
                case EType.Quest:
                    return "Quest";
                case EType.XP:
                    return "XP";
                default:
                    throw new ArgumentException("Invalid Item Type.");
            }
        }

        public string Print()
        {
            switch (this.Type)
            {
                case EType.Area:
                    return $"Can access {this.Name}";
                case EType.Combat:
                    return $"Ability to defeat {this.Amount}x {this.Name}";
                case EType.Item:
                    return $"{this.Amount}x {this.Name}";
                case EType.Lamp:
                    return $"A Lamp that gives {this.Amount} {this.Name} XP";
                case EType.Level:
                    return $"{this.Amount} {this.Name}";
                case EType.Literal:
                    return this.Name;
                case EType.QP:
                    return $"{this.Amount} QP";
                case EType.Quest:
                    return $"{this.Name} completed";
                case EType.XP:
                    return $"{this.Amount} {this.Name} XP";
            }

            return "Invalid type!";
        }
        public override bool Equals(object? obj) => this.Equals(obj as Item);

        public bool Equals(Item? other)
        {
            if (other is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            switch (this.Type)
            {
                case EType.Area:
                case EType.Combat:
                case EType.Item:
                case EType.Level:
                case EType.QP:
                case EType.Quest:
                case EType.XP:
                    return (this.Name.Equals(other.Name)) && (this.Type == other.Type);
                case EType.Literal:
                case EType.Default:
                    return (this.Name.Equals(other.Name)) && (this.Type == other.Type) && (this.Amount == other.Amount);
                default:
                    return false;
            }

        }

        public static bool operator ==(Item? lhs, Item? rhs)
        {
            if (lhs is null)
            {
                return rhs is null;
            }
            else
            {
                return rhs is not null && lhs.Equals(rhs);
            }
        }

        public static bool operator !=(Item? lhs, Item? rhs)
        {
            return !(lhs == rhs);
        }

        public bool Exactly(Item? other)
        {
            if (other is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return (this.Name.Equals(other.Name)) && (this.Type == other.Type) && (this.Amount == other.Amount);
        }

        public override int GetHashCode()
        {
            return (name, amount, (int)type).GetHashCode();
        }
    }
}
