using Godot;
using System;

public partial class ControlsMenu : CanvasLayer
{
	[Signal]
	public delegate void OnBackButtonPressedEventHandler();

	private Global global;
	private ControlSelectButton firstButton;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		firstButton = GetNode<ControlSelectButton>("%UpButton");
	}

	public void FocusFirst()
	{
		firstButton.GrabFocus();
	}

	public void OnBackButton()
	{
		EmitSignal(SignalName.OnBackButtonPressed);
	}
}
