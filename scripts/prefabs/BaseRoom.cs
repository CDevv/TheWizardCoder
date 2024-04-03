using Godot;
using System;

public partial class BaseRoom : Node2D
{
	[Signal]
	public delegate void SceneChangedEventHandler();

	private Global global;
	public GameDisplay GameDisplay { get; private set; }
	public AudioStreamPlayer AudioPlayer { get; set; }
	public SavedGamesDisplay SavedGamesDisplay { get; set; }

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		GameDisplay = GetNode<GameDisplay>("GameDisplay");
		AudioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");
		SavedGamesDisplay = GetNode<SavedGamesDisplay>("SavedGamesDisplay");

		global.CurrentRoom = this;
	}

	public override void _Process(double delta)
	{
	}
}
