using Godot;
using System;

public partial class BaseRoom : Node2D
{
	[Export]
	public string LocationName { get; set; }

	private Global global;
	public GameDisplay GameDisplay { get; private set; }
	public AudioStreamPlayer AudioPlayer { get; set; }
	public SavedGamesDisplay SavedGamesDisplay { get; set; }
	public TransitionRect TransitionRect { get; set; }

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		GameDisplay = GetNode<GameDisplay>("GameDisplay");
		AudioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");
		SavedGamesDisplay = GetNode<SavedGamesDisplay>("SavedGamesDisplay");
		TransitionRect = GetNode<TransitionRect>("TransitionRect");
		TransitionRect.Show();

		global.CurrentRoom = this;
		global.Location = LocationName;

		TransitionRect.CallDeferred(TransitionRect.MethodName.PlayAnimationBackwards);
		global.CanWalk = true;
	}

	public override void _Process(double delta)
	{
	}
}
