using Godot;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Interactables
{
	public partial class Chest : Interactable
	{
		[Export]
		public string PlaythroughPropertyName { get; set; }
		[Export]
		public Resource DialogueResource { get; set; }
		[Export]
		public ChestType ChestType { get; set; } = ChestType.Item;
		[Export]
		public string ItemName { get; set; }
		[Export]
		public int GoldAmount { get; set; }

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
			switch (ChestType)
			{
				case ChestType.Item:
					global.CurrentRoom.Dialogue.ShowDisplay(DialogueResource, "chest", format: new() { ItemName });
					global.PlayerData.AddToInventory(ItemName);
					break;
				case ChestType.Gold:
					global.CurrentRoom.Dialogue.ShowDisplay(DialogueResource, "chest_gold", format: new() { GoldAmount.ToString() });
					global.PlayerData.Gold += GoldAmount;
					break;
			}
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
}