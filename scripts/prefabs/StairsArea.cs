using Godot;
using System;

public partial class StairsArea : Area2D
{
	[Export]
	public bool Invert { get; set; } = false;
	[Export]
	public float Factor { get; set; } = 100f;
	[Export]
	public bool Up { get; set; } = true;

	private Global global;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
	}


	public void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Player")
		{
			global.PlayerIsOnStairs = true;
			global.StairsInverted = Invert;
			global.StairsGoUp = Up;
		}
	}

	public void OnBodyExited(Node2D body)
	{
		if (body.Name == "Player")
		{
			global.PlayerIsOnStairs = false;
		}
		
	}
}
