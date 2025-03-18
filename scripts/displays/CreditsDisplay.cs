using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Displays
{
    public partial class CreditsDisplay : Display
    {
        [Signal]
        public delegate void BackButtonTriggeredEventHandler();

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