using Godot;
using Godot.Collections;

namespace TheWizardCoder.Data
{
    public class Shop
    {
        public string Name { get; set; }
        public Array<string> Items { get; set; }

        public void ApplyDictionary(Dictionary<string, Variant> dict)
        {
            Name = (string)dict["Name"];
            Items = (Array<string>)dict["Items"];
        }
    }
}