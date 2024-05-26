using Godot;
using System;

public partial class Bullet : Area2D
{
	[Signal]
	public delegate void DamagedPlayerEventHandler(int value);

	[Export]
	public int Speed { get; set; } = 3;
	[Export]
	public BulletType Type { get; set; } = BulletType.PlayerBullet;
	[Export]
	public float Damage { get; set; } = 10;
	[Export]
	public bool Damagable { get; set; } = false;
	[Export]
	public int Health { get; set; } = 10;
	public Direction DirectionOfMovement { get; set; } = Direction.Right;
	private Global global;

    public override void _Ready()
    {
        global = GetNode<Global>("/root/Global");
    }

    public override void _Process(double delta)
	{
		Vector2 velocity = global.GetDirectionVector(DirectionOfMovement) * Speed;
		Position += velocity;
	}

	public void OnAreaEntered(Area2D area)
	{
		if (area.GetType() == typeof(Bullet))
		{
			Bullet otherBullet = (Bullet)area;
			if (otherBullet.Damagable && otherBullet.Type != Type)
			{
				otherBullet.Health -= (int)Damage;
				if (otherBullet.Health <= 0)
				{
					otherBullet.QueueFree();
				}
			}
		}
		else if (area.GetType() == typeof(PlayerBall))
		{
			if (Type == BulletType.EnemyBullet)
			{
				EmitSignal(SignalName.DamagedPlayer, Damage);
				QueueFree();
			}
		}
	}

	public void OnScreenExit()
	{
		QueueFree();
	}
}
