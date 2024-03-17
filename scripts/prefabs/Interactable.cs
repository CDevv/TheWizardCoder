using Godot;
using System;
using DialogueManagerRuntime;

public partial class Interactable : Area2D
{
	[Export]
	public Resource DialogueResource { get; set; }
	[Export]
	public string DialogueTitle { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ShowDialogue()
	{
		DialogueManager.ShowDialogueBalloon(DialogueResource, DialogueTitle);
	}
}
