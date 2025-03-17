using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Displays
{
    public partial class CreditsDisplay : Display
    {
        [Signal]
        public delegate void BackButtonTriggeredEventHandler();


        public override void _Ready()
        {
        }

        private void OnBackButton()
        {
            Hide();
            EmitSignal(SignalName.BackButtonTriggered);
        }

        public override void ShowDisplay()
        {
            Show();
        }
    }

}