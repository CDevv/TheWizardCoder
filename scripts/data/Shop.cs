using Godot;
using Godot.Collections;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class Shop
    {
        public string Name { get; set; }
        public ShopType Type { get; set; }
        public Array<string> Items { get; set; }

        public Shop(string name, ShopType type, Array<string> items)
        {
            Name = name;
            Type = type;
            Items = items;
        }
    }
}