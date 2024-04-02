using Godot;
using System;

public partial class Global : Node
{
	public bool CanWalk = true;
	public int Health = 100;

	private Godot.Collections.Array<string> inventory = new();

	public Godot.Collections.Array<string> Inventory
	{
		get
		{
			return inventory;
		}
	}

	public override void _Ready()
	{
	}

	public void AddToInventory(string item)
	{
		inventory.Add(item);
	}

	public void RemoveFromInventory(string item)
	{
		inventory.Remove(item);
	}
}
