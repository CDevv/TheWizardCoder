using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest15 : BaseRoom
{
	private Color redColor = new(255, 0, 0);
	private Color greenColor = new(0, 255, 0);
	private bool[] bools = { false, false, false, false, false };

	private void OnButtonPushed(int index)
	{
		bools[index] = !bools[index];
	}
}
