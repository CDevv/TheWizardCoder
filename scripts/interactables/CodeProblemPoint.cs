using Godot;
using System;
using System.Collections.Generic;

public partial class CodeProblemPoint : Interactable
{
	[Export(PropertyHint.MultilineText)]
	public string Code { get; set; }

	[Export]
	public Godot.Collections.Array<string> Items { get; set; }


	[Export]
	public Godot.Collections.Dictionary<string, Vector2> SolvableAreas { get; set; }

    public override void Action()
    {
        global.CurrentRoom.CodeProblemPanel.ShowDisplay(Code, Items, SolvableAreas);
    }
}
