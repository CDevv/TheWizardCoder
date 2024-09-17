using Godot;
using System;

public partial class Chest : Interactable
{
	[Export]
	public string PlaythroughPropertyName { get; set; }
	[Export]
	public Resource DialogueResource { get; set;}
	[Export]
	public string ItemName { get; set; }

	private AnimatedSprite2D sprite;

	public override void _Ready()
	{
		base._Ready();

		sprite = GetNode<AnimatedSprite2D>("Sprite");

		if (!string.IsNullOrEmpty(PlaythroughPropertyName) && global.PlayerData.Get(PlaythroughPropertyName).AsBool())
		{
			DisabledState();
		}
	}

	public override void Action()
	{
		global.CurrentRoom.Dialogue.ShowDisplay(DialogueResource, "chest", new() { ItemName });
		global.PlayerData.AddToInventory(ItemName);
		DisabledState();

		if (!string.IsNullOrEmpty(PlaythroughPropertyName))
		{
			global.PlayerData.Set(PlaythroughPropertyName, true);
		}
	}

	private void DisabledState()
	{
		sprite.Frame = 1;
		Active = false;

		global.CanWalk = true;
		global.GameDisplayEnabled = true;
	}

	public override void OnNotActive()
	{
		DisabledState();
	}
}
