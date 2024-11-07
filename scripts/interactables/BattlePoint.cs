using Godot;
using System;
using TheWizardCoder.Autoload;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Interactables
{
	public partial class BattlePoint : Interactable
	{
		private CollisionShape2D collisionShape;

		public string EnemyName { get; set; }
		public Texture2D BackgroundImage { get; set; }

		public override void _Ready()
		{
			base._Ready();
			collisionShape = GetNode<CollisionShape2D>("%CollisionShape");
		}

		public override void Action()
		{
			global.CanWalk = false;
			global.GameDisplayEnabled = false;
			GD.Print("Battle point");
			global.CurrentRoom.BattleDisplay.ShowDisplay(new() { EnemyName }, BackgroundImage);
			Active = false;
			collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		}
	}
}