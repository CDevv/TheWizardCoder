using Godot;
using System;
using System.Collections.Generic;

public partial class CodeProblemPoint : Interactable
{
	[Signal]
	public delegate void ProblemSolvedEventHandler();

	[Export]
	public string UniqueIdentifier { get; set; }

	[Export(PropertyHint.MultilineText)]
	public string Code { get; set; }

	[Export]
	public Godot.Collections.Array<string> Items { get; set; }


	[Export]
	public Godot.Collections.Dictionary<string, Vector2> SolvableAreas { get; set; }

	[Export]
	public bool UseInventory { get; set; } = false;

	public bool Solved { get; set; } = false;

    public override void Action()
    {
        if (!Solved)
		{
			global.CurrentRoom.CodeProblemPanel.Point = this;
			global.CurrentRoom.CodeProblemPanel.ProblemId = UniqueIdentifier;
			global.CurrentRoom.CodeProblemPanel.ShowDisplay(Code, Items, SolvableAreas, UseInventory);
		}
		else
		{
			global.CanWalk = true;
			global.GameDisplayEnabled = true;
		}
    }
}
