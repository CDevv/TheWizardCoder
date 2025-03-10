using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Subdisplays
{
    public partial class ControlsDisplay : Display
    {
        private const float sizeXNormal = 152f;
        private const float sizeXExtra = 228f;
        private const float sizeY = 24f;

        private NinePatchRect patchRect;
        private Label xControl;
        private Label zControl;
        private Label qControl;
        private MarginContainer qContainer;

        public override void _Ready()
        {
            patchRect = GetNode<NinePatchRect>("Rect");
            xControl = GetNode<Label>("%XControl");
            zControl = GetNode<Label>("%ZControl");
            qControl = GetNode<Label>("%QControl");
            qContainer = GetNode<MarginContainer>("%QContainer");
        }

        public override void ShowDisplay()
        {
            xControl.Text = "[X] Close";
            zControl.Text = "[Z] Confirm";

            Show();
        }

        public void ChangeXLabel(string text)
        {
            xControl.Text = $"[X] {text}";
        }

        public void ChangeZLabel(string text)
        {
            zControl.Text = $"[Z] {text}";
        }

        public void ChangeQLabel(string text)
        {
            qControl.Text = $"[Q] {text}";
        }

        public void ShowQLabel()
        {
            qContainer.Show();

            patchRect.Size = new(sizeXExtra, sizeY);
        }

        public void HideQLabel()
        {
            qContainer.Hide();

            patchRect.Size = new(sizeXNormal, sizeY);
        }
    }
}
