using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest3 : BaseRoom
{
    public override void OnReady()
    {
        base.OnReady();
		global.RemoveFromInventory("Forest2.cs");
		global.AddToInventory("Forest3.cs", true);
    }
}
