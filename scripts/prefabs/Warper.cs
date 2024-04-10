using Godot;
using System;

public partial class Warper : StaticBody2D
{
	[Export]
	public PackedScene TargetRoom { get; set; }
}
