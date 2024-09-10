using Godot;
using System;

public partial class GameOverDisplay : Display
{
	private Button retryButton;

	public override void _Ready()
	{
		base._Ready();
		retryButton = GetNode<Button>("RetryButton");
	}

	public override void ShowDisplay()
	{
		global.CurrentRoom.TransitionRect.PlayAnimationBackwards();
		Show();
		retryButton.GrabFocus();
	}

    public override void UpdateDisplay()
    {
        throw new NotImplementedException();
    }

    private async void OnLoadGame()
	{
		global.CurrentRoom.TransitionRect.PlayAnimation();
		await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
		global.LoadSaveFile(global.PlayerData.SaveName);
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
