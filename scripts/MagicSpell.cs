using System;
using Godot;
using Godot.Collections;

public class MagicSpell
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int Effect { get; private set; }
    public int Cost { get; private set; }
    public TargetType TargetType { get; private set; }

    public void ApplyDictionary(Dictionary<string, Variant> dict)
    {
        Name = (string)dict["Name"];
        Description = (string)dict["Description"];
        Effect = (int)dict["Effect"];
        Cost = (int)dict["Cost"];
        TargetType = Enum.Parse<TargetType>((string)dict["TargetType"]);
    }
}