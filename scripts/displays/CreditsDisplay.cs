using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class CreditsDisplay : Display
{
	[Signal]
	public delegate void BackButtonTriggeredEventHandler();

	private Button backButton;

    public override void _Ready()
    {
        backButton = GetNode<Button>("%Back");
    }

    private void OnBackButton()
	{
		Hide();
		EmitSignal(SignalName.BackButtonTriggered);
	}

    public override void ShowDisplay()
    {
		Show();
        backButton.GrabFocus();
    }
}
