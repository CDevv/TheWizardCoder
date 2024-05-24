using Godot;
using System;

public partial class OptionsMenu : CanvasLayer
{
	[Signal]
	public delegate void OnControlsButtonPressedEventHandler();
	[Signal]
	public delegate void OnBackButtonPressedEventHandler();

	private Global global;
	private OptionButton resolutionsButton;
	private CheckBox fullscreenButton;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		resolutionsButton = GetNode<OptionButton>("%ResolutionOptions");
		fullscreenButton = GetNode<CheckBox>("%FullscreenCheckBox");
	}

	public void UpdateDisplay()
	{
		resolutionsButton.Set(OptionButton.PropertyName.Selected, (int)global.Settings.WindowSize);
		fullscreenButton.Set(CheckBox.PropertyName.ButtonPressed, (bool)global.Settings.Fullscreen);
	}

	public void FocusFirst()
	{
		resolutionsButton.GrabFocus();
	}

	public void OnWindowSizeChanged(int optionId)
	{
		WindowSize size = (WindowSize)optionId;
		global.Settings.ChangeWindowSize(size);
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
