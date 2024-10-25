using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest2 : BaseRoom
{
	public override void OnReady()
	{
		base.OnReady();
		global.RemoveFromInventory("ForestInstructions1.cs");
		global.AddToInventory("ForestInstructions2.cs");
	}
}
