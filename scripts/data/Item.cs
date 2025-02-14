using System;
using Godot;
using Godot.Collections;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class Item : IApplyDictionary
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Effect { get; set; }
        public ItemType Type { get; set; }
        public int Price { get; set; }
        public bool Sellable { get; set; }
        public string[] AdditionalData { get; set; }

        public Item(Godot.Collections.Dictionary<string, Variant> dict)
        {
            ApplyDictionary(dict);
        }

        public void ApplyDictionary(Godot.Collections.Dictionary<string, Variant> dict)
        {
            Description = (string)dict["Description"];
            Effect = (int)dict["Effect"];
            Type = Enum.Parse<ItemType>((string)dict["Type"]);          

            if (dict.ContainsKey("Price"))
            {
                Sellable = true;
                Price = (int)dict["Price"];
            }
            else
            {
                Sellable = false;
            }

            if (Type == ItemType.Key || Type == ItemType.Magic)
            {
                AdditionalData = (string[])dict["AdditionalData"];
            }         
        }
    }
}