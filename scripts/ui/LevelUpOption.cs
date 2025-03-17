using Godot;

namespace TheWizardCoder.UI
{
    public partial class LevelUpOption : NinePatchRect
    {
        [Signal]
        public delegate void PressedEventHandler();

        [Export(PropertyHint.MultilineText)]
        public string Text { get; set; }

        private ColorRect background;
        private AnimatedSprite2D arrow;
        private Label label;
        private bool focused = false;

        public override void _Ready()
        {
            FocusMode = FocusModeEnum.All;
            background = GetNode<ColorRect>("Background");
            arrow = GetNode<AnimatedSprite2D>("SelectArrow");
            label = GetNode<Label>("Label");

            label.Text = Text;
        }

        public void SetText(string text)
        {
            label.Text = text;
        }

        private void OnFocusEntered()
        {
            focused = true;

            background.Modulate = new Color(255, 255, 255, 0.3f);
            arrow.Show();
            arrow.Play("default");
        }

        private void OnFocusExited()
        {
            focused = false;

            background.Modulate = new Color(255, 255, 255, 0);
            arrow.Hide();
            arrow.Stop();
        }

        private void OnGuiInput(InputEvent inputEvent)
        {
            if (Input.IsActionJustPressed("ui_accept") && focused)
            {
                EmitSignal(SignalName.Pressed);
            }
        }
    }
}
