using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Interactables
{
    public partial class SavePoint : Interactable
    {
        public override void Action()
        {
            Displays.SavedGamesDisplay savedGames = global.CurrentRoom.SavedGamesDisplay;
            savedGames.UpdateDisplay();
            savedGames.ShowDisplay();
        }
    }
}