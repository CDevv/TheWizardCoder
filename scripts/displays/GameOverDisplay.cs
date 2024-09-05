using Godot;
using System;

public partial class GameOverDisplay : CanvasLayer
{
	private Global global;
	private Button retryButton;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		retryButton = GetNode<Button>("RetryButton");
	}

	public void ShowDisplay()
	{
		global.CurrentRoom.TransitionRect.PlayAnimationBackwards();
		Show();
		retryButton.GrabFocus();
	}
}
