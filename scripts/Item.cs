using Godot;
using Godot.Collections;

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Effect { get; set; }

    public void ApplyDictionary(Dictionary<string, Variant> dict)
    {
        Name = (string)dict["Name"];
        Description = (string)dict["Description"];
        Effect = (int)dict["Effect"];
    }
}