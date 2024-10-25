using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest3 : BaseRoom
{
    public override void OnReady()
    {
        base.OnReady();
		global.RemoveFromInventory("ForestInstructions2.cs");
		global.AddToInventory("ForestInstructions3.cs");
    }
}
