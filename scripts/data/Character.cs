using Godot;
using Godot.Collections;
using System;
using TheWizardCoder.Autoload;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public delegate void LeveledUpEventHandler();

    public class Character
    {
        public event LeveledUpEventHandler LeveledUp;

        private const int BaseLevelPoints = 10;

        public Global Global { get; set; }

        public virtual string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Points { get; set; }
        public int MaxPoints { get; set; }
        public int AttackPoints { get; set; }
        public int DefensePoints { get; set; }
        public int AgilityPoints { get; set; }
        public Array<string> MagicSpells { get; set; }
        public CharacterType Type { get; set; }
        public int Level { get; set; } = 1;
        public int LevelPoints { get; set; }

        private Dictionary<string, Variant> dict;

        public Character()
        {

        }

        public Character(Dictionary<string, Variant> dict, Global global)
        {
            Global = global;
            this.dict = dict;

            if (dict != null)
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
                Level = 1;
                LevelPoints = 0;
            }
        }

        public Character Clone()
        {
            Character newData = new(this.dict, Global);
            return newData;
        }

        public void AddHealth(int value)
        {
            int newValue = Mathf.Clamp(Health + value, 0, MaxHealth);
            Health = newValue;
        }

        public void RemoveHealth(int value)
        {
            int newValue = Mathf.Clamp(Health - value, 0, MaxHealth);
            Health = newValue;
        }

        public void AddMana(int value)
        {
            int newValue = Mathf.Clamp(Points + value, 0, MaxPoints);
            Points = newValue;
        }

        public void RemoveMana(int value)
        {
            int newValue = Mathf.Clamp(Points - value, 0, MaxPoints);
            Points = newValue;
        }

        public void AddLevelPoints(int value)
        {
            LevelPoints += value;

            if (LevelPoints >= GetMaxLevelPoints())
            {
                LevelPoints -= GetMaxLevelPoints();
                Level++;

                if (Name == "Nolan")
                {
                    LeveledUp();
                }
            }
        }

        public int GetMaxLevelPoints()
        {
            return Level * BaseLevelPoints;
        }

        public void SetMaxHealth(int value)
        {
            MaxHealth = value;
            Health = value;
        }

        public void SetMaxPoints(int value)
        {
            MaxPoints = value;
            Points = value;
        }
    }
}