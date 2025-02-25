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
	/// <summary>
	/// Base class for all rooms.
	/// </summary>
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
				foreach (Character ally in global.PlayerData.Allies)
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

		/// <summary>
		/// Transition to another room with the provided <paramref name="room"/> name.
		/// The name of the <see cref="Godot.Marker2D"/> is provided with <paramref name="playerLocation"/>
		/// The direction that the <c>Player</c> should be turning to when they enter the room is
		/// provided with the <paramref name="direction"/> parameter.
		/// </summary>
		/// <param name="room">The name of the room the Player should be transitioned to</param>
		/// <param name="playerLocation">The name of the target Marker2D location</param>
		/// <param name="direction">The direction the <c>Player</c> should be turned to</param>
		public void TransitionToRoom(string room, string playerLocation, Direction direction)
		{
			global.ChangeRoom(room, playerLocation, direction);
        }

        /// <summary>
        /// Play a cutscene that is defined in the scene's <see cref="Godot.AnimationPlayer"/>.
        /// Can be awaited to play in the current thread.
        /// <example>
        /// Example with playing a cutscene in a room's <see cref="BaseRoom.OnReady"/> method:
        /// <code>
        /// public override async void OnReady()
        /// {
        ///		GD.Print("This will print before the cutscene");
        ///		await PlayCutscene("example_cutscene");
        ///		GD.Print("This will be print when the cutscene has finished playing.");
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="name"></param>
        public async Task PlayCutscene(string name)
		{
			global.CanWalk = false;
			global.GameDisplayEnabled = false;
			AnimationPlayer.Play(name, -1, 0.5f);
			await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			global.CanWalk = true;
			global.GameDisplayEnabled = true;
		}

		/// <summary>
		/// Show a dialogue with the provided <paramref name="resource"/> and <paramref name="title"/>. 
		/// Can be awaited to pause execution in the current thread.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="title"></param>
		public async Task ShowDialogue(Resource resource, string title)
		{
			Dialogue.ShowDisplay(resource, title);
			await ToSignal(Dialogue, DialogueDisplay.SignalName.DialogueEnded);
		}

		/// <summary>
		/// Tween the <c>Camera</c> to a <paramref name="position"/> for a certain <paramref name="duration"/>.
		/// Can be awaited to pause the execution until the Camera has finished tweening.
		/// </summary>
		/// <param name="position">The position that the <c>Camera</c> should tween to.</param>
		/// <param name="duration">The duration that the <c>Camera</c> should be tweened for.</param>
		public async Task TweenCamera(Vector2 position, float duration)
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(Camera, "position", position, duration);
			tween.Play();

			await ToSignal(tween, Tween.SignalName.Finished);
		}

        /// <summary>
        /// Tween the <c>Camera</c> to the <c>Player</c> for a specified <paramref name="duration"/>.
        /// Can be awaited just like <see cref="BaseRoom.TweenCamera(Vector2, float)"/>
        /// </summary>
        /// <param name="duration">The duration that the <c>Camera</c> should be tweened for.</param>
        /// <returns></returns>
        public async Task TweenCameraToPlayer(float duration)
		{
			await TweenCamera(Player.Position, duration);
		}

		/// <summary>
		/// Get the tile at the specified global <paramref name="position"/> and the <paramref name="layer"/> within
		/// the <c>TileMap</c>.
		/// <example>
		/// Example with requesting a tile and checking a custom property:
		/// <code>
		/// TileData tileData = global.CurrentRoom.GetTileAtPosition(1, new Vector2(20, 55));
		/// if (tileData.GetCustomTile("isWater").AsBool())
		/// {
		///		GD.Print("This tile is a water tile.");
		/// }
		/// </code>
		/// </example>
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="position"></param>
		/// <returns>The data for the quested tile.</returns>
		public TileData GetTileAtPosition(int layer, Vector2 position)
		{
			Vector2 localPosition = TileMap.ToLocal(position);
            Vector2I tileMapPos = TileMap.LocalToMap(localPosition);

			TileData tileData = TileMap.GetCellTileData(layer, tileMapPos);
			return tileData;
		}

		/// <summary>
		/// Show a <c>Display</c> that this scene containsby calling <c>ShowDisplay()</c>
		/// </summary>
		/// <param name="displayName">The display to hide</param>
		public void ShowDisplay(string displayName)
		{
			GetNode(displayName).Call("ShowDisplay");
		}

        /// <summary>
        /// Show a <c>Display</c> that this scene contains with provided arguments by calling <c>ShowDisplay()</c>
        /// </summary>
        /// <remarks>
		/// <see cref="BaseRoom.ShowDisplay(string, Variant[])"/> may be called via Godot's reflection due to dialogue calls
		/// </remarks>
        /// <param name="displayName">The display to hide</param>
        /// <param name="args">Arguments to <c>ShowDisplay()</c></param>
        [JetBrains.Annotations.UsedImplicitly]
        public void ShowDisplay(string displayName, params Variant[] args)
		{
			GetNode(displayName).Call("ShowDisplay", args);
		}

		/// <summary>
		/// Hide a <c>Display</c> with the provided name
		/// </summary>
		/// <param name="displayName">The display to hide</param>
		public void HideDisplay(string displayName)
		{
			GetNode(displayName).Call("HideDisplay");
		}
	}
}
