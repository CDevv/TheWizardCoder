using Godot;
using System;
using TheWizardCoder.Autoload;
using TheWizardCoder.Interactables;


namespace TheWizardCoder.Components
{
	public partial class GroundEnemy : Area2D
	{
		private const float Speed = 1.5f;

		[Export]
		public string EnemyName { get; set; } = "Glitch";

		private Global global;
		private BattlePoint battlePoint;
		private Sprite2D sprite;
		private bool following;

		public override void _Ready()
		{
			global = GetNode<Global>("/root/Global");
			battlePoint = GetNode<BattlePoint>("BattlePoint");
			sprite = GetNode<Sprite2D>("Sprite");

			battlePoint.EnemyName = EnemyName;

			Texture2D enemyTexture = ResourceLoader.Load<Texture2D>($"res://assets/characters/enemies/{EnemyName}.png");
			sprite.Texture = enemyTexture;
		}

		public override void _Process(double delta)
		{
			if (following)
			{
				Vector2 velocity = (global.CurrentRoom.Player.Position - Position).Normalized() * Speed;
				Position += velocity;
			}
		}

		private void OnBodyEntered(Node2D body)
		{
			if (global.CurrentRoom.Player.Enemy == null && Visible)
			{
				if (body.GetType() == typeof(Player))
				{
					global.CurrentRoom.BattleDisplay.BattleEnded += BattleEnded;
					global.CurrentRoom.Player.Enemy = this;
					following = true;
				}
			}	
		}

		private void OnBodyExited(Node2D body)
		{
			if (body.GetType() == typeof(Player))
			{
				global.CurrentRoom.Player.Enemy = null;
				following = false;
			}
		}

		private void BattleEnded()
		{
			battlePoint.Active = false;
			Visible = false;
			following = false;
		}
	}
}
