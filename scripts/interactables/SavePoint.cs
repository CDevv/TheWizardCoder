using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Interactables
{
    public partial class SavePoint : Interactable
    {
        public override void Action()
        {
            var savedGames = global.CurrentRoom.SavedGamesDisplay;
            savedGames.UpdateDisplay();
            savedGames.ShowDisplay();
        }
    }
}