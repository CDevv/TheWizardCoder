using Godot;
using System;
using System.Threading.Tasks;
using DialogueManagerRuntime;

public partial class BaseRoom : Node2D
{
	[Export]
	public string SceneFileName { get; set;}
	[Export]
	public string LocationName { get; set; }
	[Export]
	public string DefaultMarkerName { get; set; }

	protected Global global;
	public GameDisplay GameDisplay { get; private set; }
	public AudioStreamPlayer AudioPlayer { get; set; }
	public SavedGamesDisplay SavedGamesDisplay { get; set; }
	public BattleDisplay BattleDisplay { get; set; }
	public CodeProblemPanel CodeProblemPanel { get; set; }
	public TransitionRect TransitionRect { get; set; }
	public AnimationPlayer AnimationPlayer { get; set; }
	public DialogueDisplay Dialogue { get; set; }
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
		CodeProblemPanel = GetNode<CodeProblemPanel>("CodeProblemPanel");
		TransitionRect = GetNode<TransitionRect>("TransitionRect");
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Dialogue = GetNode<DialogueDisplay>("Dialogue");
		Player = GetNode<Player>("Player");
		Camera = GetNode<Camera2D>("Camera");

		TransitionRect.Show();

		global.CurrentRoom = this;
		global.PlayerData.SceneFileName = SceneFileName;
		global.PlayerData.Location = LocationName;

		Player.PlayIdleAnimation(global.PlayerDirection);
		if (!string.IsNullOrEmpty(global.LocationMarkerName))
		{
			Player.Position = GetNode<Marker2D>(global.LocationMarkerName).Position;
		}
		if (global.PlayerData.LocationVector != Vector2.Zero)
		{
			Player.Position = global.PlayerData.LocationVector;
		}
		Camera.Position = Player.Position;
		global.PlayerData.LocationVector = Vector2.Zero;

		TransitionRect.CallDeferred(TransitionRect.MethodName.PlayAnimationBackwards);
		global.CanWalk = true;
	}

	public async Task PlayCutscene(string name)
	{
		global.CanWalk = false;
		AnimationPlayer.Play(name, -1, 0.5f);
        await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		global.CanWalk = true;
	}

	public void ShowDialogue(Resource resource, string title)
	{
		Dialogue.ShowDisplay(resource, title);
	}
}
