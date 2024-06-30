using Godot;
using System;

public partial class StairsArea : Area2D
{
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
