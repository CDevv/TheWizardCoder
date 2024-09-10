using Godot;
using System;

public partial class Warper : StaticBody2D
{
	[Export]
	public string TargetRoomName { get; set; }
	[Export]
	public string TargetLocation { get; set; }
	[Export]
	public Direction PlayerDirection { get; set; } = Direction.Down;

	[Export]
	public bool Enabled { get; set; } = true;
}
