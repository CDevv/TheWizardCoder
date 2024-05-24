using Godot;
using System;

public partial class Damagable : Area2D
{
	[Signal]
	public delegate void DamagedEventHandler(int value);

	[Signal]
	public delegate void DiedEventHandler();

	[Export]
	public float Health { get; set; } = 100;

	public void OnAreaEntered(Area2D area)
	{
		Health -= ((Bullet)area).Damage;

		if (Health > 0)
		{
			EmitSignal(SignalName.Damaged, ((Bullet)area).Damage);
		}
		else
		{
			EmitSignal(SignalName.Died);
		}
		((Bullet)area).QueueFree();
	}
}
