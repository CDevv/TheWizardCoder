using Godot;
using System;
using System.Collections.Generic;
using TheWizardCoder.Utils;

public partial class BoxStack : Node2D
{
	private const int BoxGapY = 24;

	[Export]
	public PackedScene BoxScene { get; set; }

	private Stack<Sprite2D> boxes = new Stack<Sprite2D>();

	public void AddBox()
	{
		Sprite2D box = BoxScene.Instantiate<Sprite2D>();
		box.Modulate = ColorHelper.GetRandom();

		if (boxes.Count > 0)
		{
			Vector2 newBoxPosition = boxes.Peek().Position + new Vector2(0, -BoxGapY);
			box.Position = newBoxPosition;
		}
		else
		{
			box.Position = Position - new Vector2(0, -16);
		}

		AddChild(box);
		boxes.Push(box);
	}
}
