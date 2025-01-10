using Godot;
using Godot.Collections;
using System;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class Shop
    {
        public string Name { get; set; }
        public ShopType Type { get; set; }
        public Array<string> Items { get; set; }

        public void ApplyDictionary(Dictionary<string, Variant> dict)
        {
            Name = (string)dict["Name"];
            Type = Enum.Parse<ShopType>((string)dict["Type"]);
            Items = (Array<string>)dict["Items"];
        }
    }
}