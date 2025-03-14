using Godot;
using TheWizardCoder.Abstractions;

public partial class CodeMessageDisplay : Display
{
    private CodeEdit codeText;
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        base._Ready();
        codeText = GetNode<CodeEdit>("%CodeEdit");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("ui_cancel"))
        {
            if (Visible)
            {
                HideDisplay();
            }
        }
    }

    public override void ShowDisplay()
    {
        ShowDisplay(codeText.Text);
    }

    public void ShowDisplay(string code)
    {
        global.CanWalk = false;
        global.GameDisplayEnabled = false;

        codeText.Text = code;
        Show();
        animationPlayer.Play("show");
    }

    public override async void HideDisplay()
    {
        animationPlayer.PlayBackwards("show");
        await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        Hide();
        global.CanWalk = true;
        global.GameDisplayEnabled = true;
    }
}
