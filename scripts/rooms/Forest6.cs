using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest6 : BaseRoom
{
    public override void OnReady()
    {
        base.OnReady();
		global.RemoveFromInventory("ForestInstructions4.cs");
		global.AddToInventory("ForestInstructions5.cs");
    }
}
