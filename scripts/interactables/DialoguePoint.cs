using Godot;
using System;
using DialogueManagerRuntime;

public partial class DialoguePoint : Interactable
{
	[Export]
	public Resource DialogueResource { get; set; }
	[Export]
	public string DialogueTitle { get; set; }

    public override void Action()
    {
        global.CurrentRoom.Dialogue.ShowDisplay(DialogueResource, DialogueTitle);
    }
}
