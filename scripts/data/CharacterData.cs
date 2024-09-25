using Godot;
using Godot.Collections;
using System;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public partial class CharacterData
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Points { get; set; }
        public int MaxPoints { get; set; }
        public int AttackPoints { get; set; }
        public int DefensePoints { get; set; }
        public int AgilityPoints { get; set; }
        public Array<string> MagicSpells { get; set; }
        public CharacterType Type { get; set; }

        public void ApplyDictionary(Dictionary<string, Variant> dict)
        {
            Name = (string)dict["Name"];
            Health = (int)dict["Health"];
            MaxHealth = (int)dict["MaxHealth"];
            Points = (int)dict["Points"];
            MaxPoints = (int)dict["MaxPoints"];
            AttackPoints = (int)dict["AttackPoints"];
            DefensePoints = (int)dict["DefensePoints"];
            AgilityPoints = (int)dict["AgilityPoints"];
            MagicSpells = (Array<string>)dict["MagicSpells"];
            Type = Enum.Parse<CharacterType>((string)dict["Type"]);
        }

        public void AddHealth(int value)
        {
            int newValue = Mathf.Clamp(Health + value, 0, MaxHealth);
            Health = newValue;
        }
    }
}