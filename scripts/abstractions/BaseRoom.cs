using Godot;
using System;
using System.Threading.Tasks;
using DialogueManagerRuntime;
using TheWizardCoder.Autoload;
using TheWizardCoder.Displays;
using TheWizardCoder.Components;
using TheWizardCoder.UI;
using TheWizardCoder.Interactables;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Abstractions
{
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
		public AudioStreamPlayer AudioPlayer { get; private set; }
		public SavedGamesDisplay SavedGamesDisplay { get; private set; }
		public BattleDisplay BattleDisplay { get; private set; }
		public CodeProblemPanel CodeProblemPanel { get; private set; }
		public TransitionRect TransitionRect { get; private set; }
		public AnimationPlayer AnimationPlayer { get; private set; }
		public DialogueDisplay Dialogue { get; private set; }
		public GameOverDisplay GameOverDisplay { get; private set; }
		public ShopDisplay ShopDisplay { get; private set; }
		public CodeMessageDisplay CodeMessage { get; private set; }
		public LevelUpDisplay LevelUp { get; private set; }
		public Player Player { get; private set; }
		public Camera2D Camera { get; private set; }
		public Actor Gertrude { get; private set; }
		public TileMap TileMap { get; private set; }

		private bool WillLoadNextRoom { get; set; }
        private string NextRoomPath { get; set; }
		private Godot.Collections.Array loadProgress;


		public override void _Ready()
		{
            TransitionRect = GetNode<TransitionRect>("TransitionRect");
            TransitionRect.Show();
			TransitionRect.ShowPitchBlack();

            OnReady();

			TransitionRect.PlayAnimationBackwards();
		}

		public virtual void OnReady()
		{
            global = GetNode<Global>("/root/Global");
			GameDisplay = GetNode<GameDisplay>("GameDisplay");
			AudioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");
			SavedGamesDisplay = GetNode<SavedGamesDisplay>("SavedGamesDisplay");
			BattleDisplay = GetNode<BattleDisplay>("BattleDisplay");
			CodeProblemPanel = GetNode<CodeProblemPanel>("CodeProblemPanel");
			
			AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			Dialogue = GetNode<DialogueDisplay>("Dialogue");
			GameOverDisplay = GetNode<GameOverDisplay>("GameOverDisplay");
			ShopDisplay = GetNode<ShopDisplay>("ShopDisplay");
			CodeMessage = GetNode<CodeMessageDisplay>("CodeMessageDisplay");
			LevelUp = GetNode<LevelUpDisplay>("LevelUpDisplay");
			Player = GetNode<Player>("Player");
			Camera = GetNode<Camera2D>("Camera");
			TileMap = GetNode<TileMap>("TileMap");

			Gertrude = GetNode<Actor>("Gertrude");

            global.PlayerData.Stats.LeveledUp += () => 
			{
				global.CurrentRoom.LevelUp.ShowDisplay();
			};

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
				global.PlayerData.LocationVector = Vector2.Zero;
			}
			Camera.Position = Player.Position;
			global.PlayerData.LocationVector = Vector2.Zero;

			if (global.PlayerData.Allies.Count > 0)
			{
				foreach (CharacterData ally in global.PlayerData.Allies)
				{
					var allyCharacter = Player.AddAlly(ally.Name, true);
					allyCharacter.PlayIdleAnimation(global.PlayerDirection);
				}
			}
			Player.DistanceWalked = 0;
			
			global.CanWalk = true;
		}

        public override void _Process(double delta)
        {
            if (WillLoadNextRoom)
            {
				GD.Print("loading");
				ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(NextRoomPath);

                if (status == ResourceLoader.ThreadLoadStatus.Loaded)
                {
					Resource resource = ResourceLoader.LoadThreadedGet(NextRoomPath);
					GetTree().ChangeSceneToPacked((PackedScene)resource);
                }
				else if (status == ResourceLoader.ThreadLoadStatus.Failed)
                {
                    GD.Print("loading scene failed.");
                }
                else if (status == ResourceLoader.ThreadLoadStatus.InvalidResource)
                {
                    GD.Print("invalid scene resource.");
                }
            }
        }

		public void TransitionToRoom(string room, string playerLocation, Direction direction)
		{
			global.ChangeRoom(room, playerLocation, direction);
            /*
			global.LocationMarkerName = playerLocation;
			global.PlayerDirection = direction;

            NextRoomPath = $"res://scenes/rooms/{room}.tscn";
            WillLoadNextRoom = true;
            ResourceLoader.LoadThreadedRequest(NextRoomPath);
			*/
        }

        public async Task PlayCutscene(string name)
		{
			global.CanWalk = false;
			global.GameDisplayEnabled = false;
			AnimationPlayer.Play(name, -1, 0.5f);
			await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			global.CanWalk = true;
			global.GameDisplayEnabled = true;
		}

		public async Task ShowDialogue(Resource resource, string title)
		{
			Dialogue.ShowDisplay(resource, title);
			await ToSignal(Dialogue, DialogueDisplay.SignalName.DialogueEnded);
		}

		public async Task TweenCamera(Vector2 position, float duration)
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(Camera, "position", position, duration);
			tween.Play();

			await ToSignal(tween, Tween.SignalName.Finished);
		}

		public async Task TweenCameraToPlayer(float duration)
		{
			await TweenCamera(Player.Position, duration);
		}

		public TileData GetTileAtPosition(int layer, Vector2 position)
		{
			Vector2 localPosition = TileMap.ToLocal(position);
            Vector2I tileMapPos = TileMap.LocalToMap(localPosition);

			TileData tileData = TileMap.GetCellTileData(layer, tileMapPos);
			return tileData;
		}

		public void ShowDisplay(string displayName)
		{
			GetNode(displayName).Call("ShowDisplay");
		}

		public void ShowDisplay(string displayName, params Variant[] args)
		{
			GetNode(displayName).Call("ShowDisplay", args);
		}

		public void HideDisplay(string displayName)
		{
			GetNode(displayName).Call("HideDisplay");
		}
	}
}
