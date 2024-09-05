using Godot;
using Godot.Collections;

public partial class CharacterData : GodotObject
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Points { get; set; }
    public int MaxPoints { get; set; }
    public int AttackPoints { get; set; }
    public int DefensePoints { get; set; }
    public Array<string> MagicSpells { get; set; }

    public void ApplyDictionary(Dictionary<string, Variant> dict)
    {
        Name = (string)dict["Name"];
        Health = (int)dict["Health"];
        MaxHealth = (int)dict["MaxHealth"];
        Points = (int)dict["Points"];
        MaxPoints = (int)dict["MaxPoints"];
        AttackPoints = (int)dict["AttackPoints"];
        DefensePoints = (int)dict["DefensePoints"];
        MagicSpells = (Array<string>)dict["MagicSpells"];
    }
}