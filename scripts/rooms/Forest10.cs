using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest10 : BaseRoom
{
	public override void OnReady()
	{
		base.OnReady();
		global.RemoveFromInventory("Forest6.cs");
		global.AddToInventory("Forest7.cs");
	}
}
