using Godot;
using Godot.Collections;
using System;
using System.Linq;
using TheWizardCoder.Autoload;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public delegate void LeveledUpEventHandler();

    public class Character
    {
        public event LeveledUpEventHandler LeveledUp;

        private const int BaseLevelPoints = 10;

        public int InitialHealth {  get; private set; }
        public int InitialMana { get; private set; }
        public int InitialAttackPoints { get; private set; }
        public int InitialDefensePoint { get; private set; }
        public int InitialAgilityPoints { get; private set; }

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
        public Array<string> Armours { get; set; }
        public Dictionary<int, string> EquippedArmours { get; set; }
        public CharacterType Type { get; set; }
        public int Level { get; set; } = 1;
        public int LevelPoints { get; set; }
        public ArmourEffects ArmourEffects { get; set; }
        public Dictionary<int, Array<CharacterAction>> Behaviour { get; set; }

        private Dictionary<string, Variant> dict;

        public Character()
        {
            Armours = new();
            EquippedArmours = new();
            ArmourEffects = new();
        }

        public Character(Dictionary<string, Variant> dict, Global global) : this()
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

                InitializeArmours(dict);
                InitializeDefaults();

                ApplyArmourEffects();

                if (Type == CharacterType.Enemy)
                {
                    if (dict.ContainsKey("Behaviour"))
                    {
                        FetchBehaviour(dict["Behaviour"]);
                    }
                }
            }
        }

        public void InitializeArmours(Dictionary<string, Variant> dict)
        {
            if (dict.ContainsKey("EquippedArmour"))
            {
                Array<string> equippedArmoursArr = (Array<string>)dict["EquippedArmour"];

                for (int i = 0; i < equippedArmoursArr.Count; i++)
                {
                    EquipArmour(i, equippedArmoursArr[i], false);
                }
            }
        }

        public void InitializeDefaults()
        {
            if (Global.Characters.Count > 0)
            {
                InitialHealth = Global.CharactersInitial[Name].InitialHealth;
                InitialMana = Global.CharactersInitial[Name].InitialMana;
                InitialAttackPoints = Global.CharactersInitial[Name].InitialAttackPoints;
                InitialDefensePoint = Global.CharactersInitial[Name].InitialDefensePoint;
                InitialAgilityPoints = Global.CharactersInitial[Name].InitialAgilityPoints;
            }
            else
            {
                InitialHealth = MaxHealth;
                InitialMana = MaxPoints;
                InitialAttackPoints = AttackPoints;
                InitialDefensePoint = DefensePoints;
                InitialAgilityPoints = AgilityPoints;
            }

            MagicSpells = new Array<string>(MagicSpells.Distinct());
        }

        private void FetchBehaviour(Variant variant)
        {
            Behaviour = new();
            Dictionary<string, Variant> dict = (Dictionary<string, Variant>)variant;

            foreach (System.Collections.Generic.KeyValuePair<string, Variant> item in dict)
            {
                int healthLevel = int.Parse(item.Key);

                Array<string> possibleActionsStrings = (Array<string>)item.Value;
                Array<CharacterAction> possibleActions = new();

                foreach (string actionString in possibleActionsStrings)
                {
                    CharacterAction action = Enum.Parse<CharacterAction>(actionString);
                    possibleActions.Add(action);
                }

                Behaviour[healthLevel] = possibleActions;
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

        public void ChangeHealth(int value)
        {
            if (value > 0)
            {
                AddHealth(value);
            }
            else
            {
                RemoveHealth(-value);
            }
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

        public void ChangeMana(int value)
        {
            if (value > 0)
            {
                AddMana(value);
            }
            else
            {
                RemoveMana(-value);
            }
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

        public CharacterAction ChooseBehaviour()
        {
            if (Behaviour == null)
            {
                return CharacterAction.Attack;
            }
            else
            {
                bool found = false;
                System.Collections.Generic.KeyValuePair<int, Array<CharacterAction>> pair = new();

                foreach (System.Collections.Generic.KeyValuePair<int, Array<CharacterAction>> item in Behaviour)
                {
                    if (Health <= item.Key)
                    {
                        found = true;
                        pair = item;
                    }
                }

                if (found)
                {
                    CharacterAction chosenAction = pair.Value.PickRandom();
                    return chosenAction;
                }
                else
                {
                    return CharacterAction.Attack;
                }
            }
        }

        public string GetRandomMagicSpell()
        {
            return MagicSpells.PickRandom();
        }

        public void AddArmourToInventory(string name)
        {
            if (!Armours.Contains(name))
            {
                Armours.Add(name);
            }
        }

        public void EquipArmour(int index, string name)
        {
            EquipArmour(index, name, true);
        }

        private void EquipArmour(int index, string name, bool checkPlayerData)
        {
            bool pass = false;

            if (!checkPlayerData)
            {
                pass = true;
            }
            else if (Global.PlayerData.Armours.Contains(name))
            {
                pass = true;
            }

            if (pass)
            {
                EquippedArmours[index] = name;

                Armour armour = Global.Armours[name];
                if (armour != null)
                {
                    switch (armour.EffectType)
                    {
                        case BattleEffectType.Defense:
                            ArmourEffects.Defense += armour.Effect;
                            break;
                        case BattleEffectType.Attack:
                            ArmourEffects.Attack += armour.Effect;
                            break;
                        case BattleEffectType.Mana:
                            ArmourEffects.Mana += armour.Effect;
                            break;
                        case BattleEffectType.Speed:
                            ArmourEffects.Agility += armour.Effect;
                            break;
                        case BattleEffectType.Health:
                            ArmourEffects.Health += armour.Effect;
                            break;
                    }
                }
            }
        }

        public void UnequipArmour(int index)
        {
            if (!EquippedArmours.ContainsKey(index))
            {
                return;
            }

            if (!string.IsNullOrEmpty(EquippedArmours[index]))
            {
                string name = EquippedArmours[index];
                Armour armour = Global.Armours[name];

                EquippedArmours[index] = string.Empty;

                if (armour != null)
                {
                    switch (armour.EffectType)
                    {
                        case BattleEffectType.Defense:
                            ArmourEffects.Defense -= armour.Effect;
                            break;
                        case BattleEffectType.Attack:
                            ArmourEffects.Attack -= armour.Effect;
                            break;
                        case BattleEffectType.Mana:
                            ArmourEffects.Mana -= armour.Effect;
                            break;
                        case BattleEffectType.Speed:
                            ArmourEffects.Agility -= armour.Effect;
                            break;
                        case BattleEffectType.Health:
                            ArmourEffects.Health -= armour.Effect;
                            break;
                    }
                }
            }
        }

        public bool HasEquippedArmour(int index)
        {
            if (EquippedArmours.ContainsKey(index))
            {
                if (string.IsNullOrEmpty(EquippedArmours[index]))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public bool HasEquippedArmour(string name)
        {
            bool result = false;

            for (int i = 0; i < 3; i++)
            {
                if (EquippedArmours.ContainsKey(i))
                {
                    if (EquippedArmours[i] == name)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        public void ApplyArmourEffects()
        {
            MaxHealth = InitialHealth + ArmourEffects.Health;
            MaxPoints = InitialMana + ArmourEffects.Mana;
            AttackPoints = InitialAttackPoints + ArmourEffects.Attack;
            DefensePoints = InitialDefensePoint + ArmourEffects.Defense;
            AgilityPoints = InitialAgilityPoints + ArmourEffects.Agility;
        }

        public void ApplyBattleEffect(BattleEffect effect)
        {
            int multiplier = effect.IsNegative ? -1 : 1;

            switch (effect.Action)
            {
                case BattleEffectType.Attack:
                    AttackPoints += effect.Effect * multiplier;
                    break;
                case BattleEffectType.Defense:
                    DefensePoints += effect.Effect * multiplier;
                    break;
                case BattleEffectType.Mana:
                    MaxPoints += effect.Effect * multiplier;
                    Points = MaxPoints;
                    break;
                case BattleEffectType.Speed:
                    AgilityPoints += effect.Effect * multiplier;
                    break;
                case BattleEffectType.Health:
                    MaxHealth += effect.Effect * multiplier;
                    Health = MaxHealth;
                    break;
            }
        }

        public void RemoveBattleEffect(BattleEffect effect)
        {
            int multiplier = effect.IsNegative ? -1 : 1;

            switch (effect.Action)
            {
                case BattleEffectType.Attack:
                    AttackPoints -= effect.Effect * multiplier;
                    break;
                case BattleEffectType.Defense:
                    DefensePoints -= effect.Effect * multiplier;
                    break;
                case BattleEffectType.Mana:
                    MaxPoints -= effect.Effect * multiplier;
                    Points = MaxPoints;
                    break;
                case BattleEffectType.Speed:
                    AgilityPoints -= effect.Effect * multiplier;
                    break;
                case BattleEffectType.Health:
                    MaxHealth -= effect.Effect * multiplier;
                    Health = MaxHealth;
                    break;
            }
        }
    }
}