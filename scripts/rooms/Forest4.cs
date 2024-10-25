using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest4 : BaseRoom
{
    public override void OnReady()
    {
        base.OnReady();
		global.RemoveFromInventory("ForestInstructions3.cs");
		global.AddToInventory("ForestInstructions4.cs");
    }
}
