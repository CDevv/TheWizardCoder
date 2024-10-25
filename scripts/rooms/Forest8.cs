using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest8 : BaseRoom
{
    public override void OnReady()
    {
        base.OnReady();
		global.RemoveFromInventory("ForestInstructions5.cs");
		global.AddToInventory("ForestInstructions6.cs");
    }
}
