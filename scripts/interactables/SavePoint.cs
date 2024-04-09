using Godot;
using System;

public partial class SavePoint : Interactable
{
    public override void Action()
    {
        var savedGames = global.CurrentRoom.SavedGamesDisplay;
        savedGames.UpdateDisplay();
        savedGames.Show();
        savedGames.FocusOnSaveButton();
    }
}