using DialogueManagerRuntime;
using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class ForestNobeResidence : BaseRoom
{
	private void OnDialogueEnded(string initialTitle, string lastTitle)
	{
		if (initialTitle == "nobe_intro" && lastTitle == "8")
		{
			global.CurrentRoom.ShopDisplay.ShowDisplay("NobeShop");
		}
	}
}
