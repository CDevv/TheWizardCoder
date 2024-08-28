using Godot;
using System;

public partial class CutscenePoint : Area2D
{
	[Export]
	public string RoomMethodName { get; set; }
	public bool Active { get; set; } = true;

	private Global global;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
	}

	private void OnBodyEntered(Node2D node)
	{
		if (Active)
		{
			if (node.GetType() == typeof(Player))
			{
				Active = false;
				global.CurrentRoom.CallDeferred(RoomMethodName);
			}
		}
	}
}
