using Godot;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.UI;

namespace TheWizardCoder.Subdisplays
{
	public partial class ControlsMenu : Display
	{
		[Signal]
		public delegate void OnBackButtonPressedEventHandler();

		private ControlSelectButton upButton;
		private ControlSelectButton downButton;
		private ControlSelectButton leftButton;
		private ControlSelectButton rightButton;

		public override void _Ready()
		{
			base._Ready();
			upButton = GetNode<ControlSelectButton>("%UpButton");
			downButton = GetNode<ControlSelectButton>("%DownButton");
			leftButton = GetNode<ControlSelectButton>("%LeftButton");
			rightButton = GetNode<ControlSelectButton>("%RightButton");
		}

		public override void _Input(InputEvent @event)
		{
			if (Input.IsActionJustPressed("ui_cancel"))
			{
				if (Visible)
				{
					EmitSignal(SignalName.OnBackButtonPressed);
				}
			}
		}

		public override void ShowDisplay()
		{
			Show();
			FocusFirst();
		}

		public override void UpdateDisplay()
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
}