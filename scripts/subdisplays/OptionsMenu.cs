using Godot;
using System;

public partial class OptionsMenu : CanvasLayer
{
	[Signal]
	public delegate void OnControlsButtonPressedEventHandler();
	[Signal]
	public delegate void OnBackButtonPressedEventHandler();

	private Global global;
	private OptionButton firstButton;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		firstButton = GetNode<OptionButton>("%ResolutionOptions");
	}

	public void FocusFirst()
	{
		firstButton.GrabFocus();
	}

	public void OnWindowSizeChanged(int optionId)
	{
		WindowSize size = (WindowSize)optionId;
		global.ChangeWindowSize(size);
		global.Settings.SaveSettings();
	}

	public void OnFullscreenToggled(bool toggled)
	{
		global.Settings.ToggleFullscreen(toggled);
	}

	public void OnBackButton()
	{
		EmitSignal(SignalName.OnBackButtonPressed);
	}

	public void OnControlsPage()
	{
		EmitSignal(SignalName.OnControlsButtonPressed);
	}
}
