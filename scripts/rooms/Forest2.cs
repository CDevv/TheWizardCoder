using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest2 : BaseRoom
{
	public override void OnReady()
	{
		base.OnReady();
		global.RemoveFromInventory("Forest1.cs");
		global.AddToInventory("Forest2.cs", true);
	}
}
