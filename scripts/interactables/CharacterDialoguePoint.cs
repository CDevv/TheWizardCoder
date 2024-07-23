using Godot;
using System;
using DialogueManagerRuntime;

public partial class CharacterDialoguePoint : Interactable
{
	[Export]
	public Resource DialogueResource { get; set; }
	[Export]
	public string DialogueTitle { get; set; }

	private AnimatedSprite2D sprite;

	public override void _Ready()
	{
		base._Ready();
		sprite = GetNode<AnimatedSprite2D>("Sprite");
	}

	public override void Action()
	{
		Player player = global.CurrentRoom.Player;
		sprite.Frame = global.ReverseDirections[(int)player.Direction];
		//DialogueManager.ShowDialogueBalloon(DialogueResource, DialogueTitle);
		global.CurrentRoom.Dialogue.ShowDisplay(DialogueResource, DialogueTitle);
		global.CurrentRoom.Dialogue.DialogueEnded += OnDialogueEnded;
	}

	private void PlayAnimation(string name)
	{
		sprite.Play(name);
	}

	private void PlayIdleAnimation(Direction direction)
	{
		sprite.Animation = "default";
		sprite.Frame = (int)direction;
	}

	private void OnDialogueEnded()
	{
		sprite.Frame = (int)Direction.Down;
		global.CurrentRoom.Dialogue.DialogueEnded -= OnDialogueEnded;
	}
}
