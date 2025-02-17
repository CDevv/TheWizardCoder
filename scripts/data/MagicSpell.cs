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

        public MagicSpell(string name, string description, int effect, int cost, int shopPrice, CharacterType targetType)
        {
            Name = name;
            Description = description;
            Effect = effect;
            Cost = cost;
            ShopPrice = shopPrice;
            TargetType = targetType;

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
    }
}