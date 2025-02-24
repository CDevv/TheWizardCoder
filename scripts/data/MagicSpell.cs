using System;
using Godot;
using Godot.Collections;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class MagicSpell
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Effect { get; private set; }
        public int Cost { get; private set; }
        public int ShopPrice { get; set; }
        public CharacterType TargetType { get; private set; }
        public MagicSpellType SpellType { get; set; }
        public bool HasBattleEffect { get; set; } = false;
        public BattleEffect BattleEffect { get; set; }

        public MagicSpell(string name, string description, int effect, int cost, int shopPrice, CharacterType targetType, MagicSpellType type)
        {
            Name = name;
            Description = description;
            Effect = effect;
            Cost = cost;
            ShopPrice = shopPrice;
            TargetType = targetType;
            SpellType = type;

            if (Effect < 0)
            {
                throw new ArgumentException("Effect cannot be negative.");
            }

            if (Cost < 0)
            {
                throw new ArgumentException("Cost cannot be negative.");
            }

            if (ShopPrice < 0)
            {
                throw new ArgumentException("ShopPrice cannot be negative.");
            }
        }

        public void SetBattleEffectData(string[] data)
        {
            HasBattleEffect = true;

            BattleEffectType action = Enum.Parse<BattleEffectType>(data[0]);
            CharacterType targetType = TargetType;
            int turns = int.Parse(data[1]);
            int effect = int.Parse(data[2]);
            bool isNegative = bool.Parse(data[3]);

            BattleEffect = new(action, targetType, turns, effect, isNegative);
        }
    }
}