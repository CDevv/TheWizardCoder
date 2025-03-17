using Godot.Collections;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class Shop
    {
        public string Name { get; set; }
        public Array<string> Items { get; set; }

        public Shop(string name, Array<string> items)
        {
            Name = name;
            Items = items;
        }
    }
}