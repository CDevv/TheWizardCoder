using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;
using TheWizardCoder.UI;

namespace TheWizardCoder.Displays
{
    public partial class GameOverDisplay : Display
    {
        private Button retryButton;

        public override void _Ready()
        {
            base._Ready();
            retryButton = GetNode<Button>("Retry");
        }

        public override void ShowDisplay()
        {
            global.CurrentRoom.TransitionRect.PlayAnimationBackwards();
            Show();
            retryButton.GrabFocus();
        }

        private async void OnLoadGame()
        {
            global.CurrentRoom.TransitionRect.PlayAnimation();
            await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
            global.SaveFiles.LoadSaveFile(global.PlayerData.SaveName);
            global.ChangeRoom(global.PlayerData.SceneFileName, global.PlayerData.SceneDefaultMarker, Direction.Down);
            Hide();
        }

        private async void OnMainMenu()
        {
            global.CurrentRoom.TransitionRect.PlayAnimation();
            await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
            global.GoToMainMenu();
        }
    }
}