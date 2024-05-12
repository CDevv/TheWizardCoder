using Godot;
using System;
using System.Threading.Tasks;

public partial class BaseRoom : Node2D
{
	[Export]
	public string LocationName { get; set; }

	protected Global global;
	public GameDisplay GameDisplay { get; private set; }
	public AudioStreamPlayer AudioPlayer { get; set; }
	public SavedGamesDisplay SavedGamesDisplay { get; set; }
	public BattleDisplay BattleDisplay { get; set; }
	public TransitionRect TransitionRect { get; set; }
	public AnimationPlayer AnimationPlayer { get; set; }
	public Player Player { get; set; }
	public Camera2D Camera { get; set; }

	public override void _Ready()
	{
		OnReady();
	}

	public override void _Process(double delta)
	{
	}

	public virtual void OnReady()
	{
		global = GetNode<Global>("/root/Global");
		GameDisplay = GetNode<GameDisplay>("GameDisplay");
		AudioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");
		SavedGamesDisplay = GetNode<SavedGamesDisplay>("SavedGamesDisplay");
		BattleDisplay = GetNode<BattleDisplay>("BattleDisplay");
		TransitionRect = GetNode<TransitionRect>("TransitionRect");
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Player = GetNode<Player>("Player");
		Camera = GetNode<Camera2D>("Camera");

		TransitionRect.Show();

		global.CurrentRoom = this;
		global.Location = LocationName;

		Player.PlayIdleAnimation(global.PlayerDirection);
		if (!string.IsNullOrEmpty(global.LocationMarkerName))
		{
			Player.Position = GetNode<Marker2D>(global.LocationMarkerName).Position;
		}		

		TransitionRect.CallDeferred(TransitionRect.MethodName.PlayAnimationBackwards);
		global.CanWalk = true;
	}

	public async Task PlayCutscene(string name)
	{
		global.CanWalk = false;
		AnimationPlayer.Play(name, -1, 0.5f);
        await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		//global.CanWalk = true;
	}
}
