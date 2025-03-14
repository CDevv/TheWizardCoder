using Godot;

namespace TheWizardCoder.UI
{
    [Tool]
    public partial class ConsoleBoxText : NinePatchRect
    {
        [Signal]
        public delegate void OnResizedEventHandler();

        private string text = string.Empty;

        [Export]
        public string Text
        {
            get { return text; }
            set
            {
                text = value;

                label = GetNodeOrNull<Label>("%Label");

                if (label != null)
                {
                    label.Text = value;
                    OnLabelResized();
                }
            }
        }

        private Label label;
        private MarginContainer marginContainer;

        public override void _Ready()
        {
            marginContainer = GetNode<MarginContainer>("MarginContainer");
            label = GetNode<Label>("%Label");
            label.Text = Text;
        }

        public void SetText(string text)
        {
            label.Text = text;
            OnLabelResized();
        }

        private void OnLabelResized()
        {
            marginContainer = GetNodeOrNull<MarginContainer>("MarginContainer");

            if (marginContainer != null)
            {
                Size = marginContainer.Size;
            }
        }
    }
}
