using Godot;
using System;

public partial class EnemyBullet : Area2D
{
	[Signal]
	public delegate void DamagedPlayerEventHandler(int value);

	private int speed = 3;
	public float Damage { get; set; } = 10;
	public Direction DirectionOfMovement { get; set; } = Direction.Down;
	private Global global;

    public override void _Ready()
    {
        global = GetNode<Global>("/root/Global");
    }

	public override void _Process(double delta)
	{
		Vector2 velocity = global.GetDirectionVector(DirectionOfMovement) * speed;
		Position += velocity;
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
