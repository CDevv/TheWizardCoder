using Godot;
using System;

public abstract partial class Interactable : Area2D
{
	protected Global global;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
	}

	public abstract void Action();
}
