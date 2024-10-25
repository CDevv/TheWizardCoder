using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest10 : BaseRoom
{
    public override void OnReady()
    {
        base.OnReady();
		global.RemoveFromInventory("ForestInstructions6.cs");
		global.AddToInventory("ForestInstructions7.cs");
    }
}
