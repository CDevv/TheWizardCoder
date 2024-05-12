using Godot;
using System;

public partial class BattleDisplay : CanvasLayer
{
	private Global global;
	private AnimationPlayer animationPlayer;
	private Marker2D playerMarker;
	private Marker2D enemyMarker;
	private Button fightButton;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		playerMarker = GetNode<Marker2D>("PlayerMarker");
		fightButton = GetNode<Button>("%FightButton");
	}

	public override void _Process(double delta)
	{
	}

	public void ShowDisplay()
	{
		fightButton.GrabFocus();
		Vector2 resolutionVector = new Vector2(global.Settings.WindowWidth, global.Settings.WindowHeight);
		Vector2 playerFinalPosition = global.CurrentRoom.Camera.GlobalPosition - (resolutionVector / 2) + playerMarker.Position;

		Show();
		animationPlayer.Play("battle_intro");
		global.CurrentRoom.Player.PlayIdleAnimation(Direction.Right);
		Tween posTween = global.CurrentRoom.Player.CreateTween();
		posTween.TweenProperty(global.CurrentRoom.Player, (string)Player.PropertyName.Position, playerFinalPosition, 1);
	}
}
