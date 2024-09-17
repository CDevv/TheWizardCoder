using Godot;
using System;

public partial class BattlePoint : Interactable
{ 
    public override void _Ready()
	{
		base._Ready();
	}

	public override void Action()
    {
        global.CanWalk = false;
		global.GameDisplayEnabled = false;
		GD.Print("Battle point");
		global.CurrentRoom.BattleDisplay.ShowDisplay(new() { "Glitch" });
    }
}
