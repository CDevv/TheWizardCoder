using System;
using Godot;
using Godot.Collections;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class MagicSpell : IApplyDictionary
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Effect { get; private set; }
        public int Cost { get; private set; }
        public int ShopPrice { get; set; }
        public CharacterType TargetType { get; private set; }

        public MagicSpell(Dictionary<string, Variant> dict)
        {
            ApplyDictionary(dict);
        }

        public void ApplyDictionary(Dictionary<string, Variant> dict)
        {
            Name = (string)dict["Name"];
            Description = (string)dict["Description"];
            Effect = (int)dict["Effect"];
            Cost = (int)dict["Cost"];
            ShopPrice = (int)dict["ShopPrice"];
            TargetType = Enum.Parse<CharacterType>((string)dict["TargetType"]);
        }
    }
}