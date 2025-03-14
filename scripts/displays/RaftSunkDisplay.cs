using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.UI;

public partial class RaftSunkDisplay : Display
{
    [Signal]
    public delegate void RetryPressedEventHandler();

    private Button retryButton;

    public override void _Ready()
    {
        base._Ready();
        retryButton = GetNode<Button>("Retry");
    }

    public override async void ShowDisplay()
    {
        Show();
        global.CurrentRoom.TransitionRect.PlayAnimationBackwards();
        await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
        retryButton.GrabFocus();
    }

    public override async void HideDisplay()
    {
        global.CurrentRoom.TransitionRect.PlayAnimation();
        await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
        Hide();
    }

    private void OnRetry()
    {
        HideDisplay();
        EmitSignal(SignalName.RetryPressed);
    }

    private async void OnMainMenu()
    {
        global.CurrentRoom.TransitionRect.PlayAnimation();
        await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
        global.GoToMainMenu();
    }
}
