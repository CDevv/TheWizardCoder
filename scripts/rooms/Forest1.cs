using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest1 : BaseRoom
{
    public override void OnReady()
    {
        base.OnReady();
		global.AddToInventory("Forest1.cs", true);
    }
}
