using DialogueManagerRuntime;
using Godot;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Interactables;

public partial class ForestNobeResidence : BaseRoom
{
	private Warper cabinWarper;
	private DialoguePoint dialoguePoint;

    public override void OnReady()
    {
        base.OnReady();

		cabinWarper = GetNode<Warper>("NobeCabinWarper");
		dialoguePoint = GetNode<DialoguePoint>("NobeCabinDialogue");

		if (global.PlayerData.UnlockedNobeCabin)
		{
			UnlockCabin();
		}
    }

    private void UnlockCabin()
	{
		global.PlayerData.UnlockedNobeCabin = true;
		cabinWarper.Active = true;
		dialoguePoint.Active = false;
	}
}
