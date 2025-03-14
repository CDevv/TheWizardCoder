using System;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Effect { get; set; }
        public ItemType Type { get; set; }
        public int Price { get; set; }
        public bool Sellable { get; set; }
        public string[] AdditionalData { get; set; }

        public Item(string name, string description, int effect, ItemType type)
        {
            Name = name;
            Description = description;
            Effect = effect;
            Type = type;

            if (Effect < 0)
            {
                throw new ArgumentException("Effect cannot be negative.");
            }
        }

        public void MakeSellable(int price)
        {
            Sellable = true;
            Price = price;

            if (Price < 0)
            {
                throw new ArgumentException("Price cannot be negative.");
            }
        }

        public void AddAdditionalData(string[] data)
        {
            AdditionalData = data;
        }
    }
}