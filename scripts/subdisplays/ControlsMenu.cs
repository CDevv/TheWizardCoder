using Godot;
using System;

public partial class ControlsMenu : CanvasLayer
{
	[Signal]
	public delegate void OnBackButtonPressedEventHandler();

	private Global global;
	private ControlSelectButton upButton;
	private ControlSelectButton downButton;
	private ControlSelectButton leftButton;
	private ControlSelectButton rightButton;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		upButton = GetNode<ControlSelectButton>("%UpButton");
		downButton = GetNode<ControlSelectButton>("%DownButton");
		leftButton = GetNode<ControlSelectButton>("%LeftButton");
		rightButton = GetNode<ControlSelectButton>("%RightButton");
	}

	public void UpdateDisplay()
	{
		upButton.Text = ((InputEventKey)global.Settings.Controls["up"]).AsTextKeycode();
		downButton.Text = ((InputEventKey)global.Settings.Controls["down"]).AsTextKeycode();
		leftButton.Text = ((InputEventKey)global.Settings.Controls["left"]).AsTextKeycode();
		rightButton.Text = ((InputEventKey)global.Settings.Controls["right"]).AsTextKeycode();
	}

	public void FocusFirst()
	{
		upButton.GrabFocus();
	}

	public void OnBackButton()
	{
		EmitSignal(SignalName.OnBackButtonPressed);
	}
}
