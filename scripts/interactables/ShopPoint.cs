using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class ShopPoint : Interactable
{
	public override void _Ready()
	{
		base._Ready();
	}

    public override void Action()
    {
        global.OpenShop("TestShop");
    }
}
