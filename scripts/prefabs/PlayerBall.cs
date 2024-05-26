using Godot;
using System;

public partial class PlayerBall : Area2D
{
	[Export]
	public PackedScene BulletPackedScene { get; set; }

	public void SpawnPlayerBullet()
	{
		Bullet bullet = BulletPackedScene.Instantiate<Bullet>();
		bullet.Position = Position;
		bullet.DirectionOfMovement = Direction.Up;

		GetParent().CallDeferred(Node.MethodName.AddChild, bullet);
	}
}
