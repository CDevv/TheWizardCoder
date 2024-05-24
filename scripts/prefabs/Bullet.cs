using Godot;
using System;

public partial class Bullet : Area2D
{
	private int speed = 3;
	public float Damage { get; set; } = 10;

	public override void _Process(double delta)
	{
		Position += new Vector2(speed, 0);
	}

	public void OnScreenExit()
	{
		QueueFree();
	}
}
