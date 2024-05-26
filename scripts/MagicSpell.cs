using System;
using Godot;
using Godot.Collections;

public class MagicSpell
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool OneTime { get; private set; }
    public int Effect { get; private set; }
    public string MethodName { get; private set; }

    public void ApplyDictionary(Dictionary<string, Variant> dict)
    {
        Name = (string)dict["Name"];
        Description = (string)dict["Description"];
        OneTime = (bool)dict["OneTime"];
        Effect = (int)dict["Effect"];
        MethodName = (string)dict["MethodName"];
    }
}