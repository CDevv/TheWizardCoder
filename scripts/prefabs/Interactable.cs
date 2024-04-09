using Godot;
using System;

public abstract partial class Interactable : Area2D
{
	protected Global global;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
	}

	public abstract void Action();
}
