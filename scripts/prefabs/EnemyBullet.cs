using Godot;
using System;

public partial class EnemyBullet : Area2D
{
	[Signal]
	public delegate void DamagedPlayerEventHandler(int value);

	private int speed = 3;

	public float Damage { get; set; } = 10;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += new Vector2(0, speed);
	}

	public void OnScreenExit()
	{
		QueueFree();
	}

	public void OnAreaEntered(Area2D area)
	{
		if (area.GetType() == typeof(PlayerBall))
		{
			EmitSignal(SignalName.DamagedPlayer, new Variant[] {Damage});
			QueueFree();
		}
	}

	public void OnAreaExit(Area2D area)
	{
		if (area.GetType() == typeof(CombatArea))
		{
			QueueFree();
		}
	}
}
