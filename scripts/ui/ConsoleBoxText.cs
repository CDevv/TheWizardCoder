using Godot;
using System;

public partial class ConsoleBoxText : NinePatchRect
{
	[Export]
	public string Text { get; set; }

	private Label label;
	private MarginContainer marginContainer;

	public override void _Ready()
	{
		marginContainer = GetNode<MarginContainer>("MarginContainer");
		label = GetNode<Label>("%Label");
		label.Text = Text;
	}

	public void SetText(string text)
	{
		label.Text = text;
	}

	private void OnLabelResized()
	{
		if (marginContainer != null)
		{
			Size = marginContainer.Size;
		}
	}
}
