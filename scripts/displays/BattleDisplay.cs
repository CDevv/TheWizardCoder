using DialogueManagerRuntime;
using Godot;
using Godot.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TheWizardCoder.Autoload;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using TheWizardCoder.Subdisplays;
using TheWizardCoder.UI;

namespace TheWizardCoder.Displays
{
	public partial class BattleDisplay : Display
	{
		private Vector2 startingPoint = new Vector2(16, 408);

		[Signal]
		public delegate void BattleEndedEventHandler();
		[Signal]
		public delegate void TurnFinishedEventHandler();

		[Export]
		public Resource TutorialDialogueResource { get; set; }
		[Export]
		public PackedScene CharacterRectScene { get; set; }
		[Export]
		public PackedScene EnemySpriteScene { get; set; }
		[Export]
		public AlliesContainer Allies { get; private set; }
		[Export]
		public EnemiesContainer Enemies { get; private set; }

		public bool IsBattleEnded { get; private set; } = false;
		public bool IsTutorial { get; set; } = false;


		private AlliesContainer alliesContainer;
		private EnemiesContainer enemiesContainer;
		private BattleOptions battleOptions;
		private EnemyHealthBar enemyHealthContainer;
		private TextureProgressBar enemyHealthBar;
		private DamageIndicator damageIndicator;
		private Marker2D enemySpritePoint;
		private Button invisButton;
		private TextureRect backgroundRect;

		public override void _Ready()
		{
			base._Ready();
			battleOptions = GetNode<BattleOptions>("BattleOptions");
			enemyHealthContainer = GetNode<EnemyHealthBar>("EnemyHealthBar");
			damageIndicator = GetNode<DamageIndicator>("DamageIndicator");
			enemySpritePoint = GetNode<Marker2D>("EnemySpritePoint");
			invisButton = GetNode<Button>("InvisButton");
			backgroundRect = GetNode<TextureRect>("Background");
		}

		public override void _Input(InputEvent inputEvent)
		{
			if (Input.IsActionJustPressed("ui_cancel"))
			{
				battleOptions.ShowOptions();
			}
		}

		public override void ShowDisplay()
		{
			ShowDisplay(new() {"Glitch"}, backgroundRect.Texture);
		}

		public async void ShowDisplay(Array<string> enemies)
		{
			ShowDisplay(enemies, backgroundRect.Texture);
		}

		public async void ShowDisplay(Array<string> enemies, Texture2D background)
		{
			IsBattleEnded = false;
			backgroundRect.Texture = background;

			//Add allies
			Allies.AddCharacter(global.PlayerData.Stats);
			if (global.PlayerData.Allies.Count > 0)
			{
				foreach (CharacterData ally in global.PlayerData.Allies)
				{
					Allies.AddCharacter(ally);
				}
			}

			//Add enemies
			foreach (string enemyName in enemies)
			{
				CharacterData newEnemy = global.Characters[enemyName];
				Enemies.AddCharacter(newEnemy);
			}

			global.CurrentRoom.TransitionRect.PlayAnimation();
			await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
			Show();
			battleOptions.ShowDisplay();
			global.CurrentRoom.TransitionRect.PlayAnimationBackwards();

			if (IsTutorial)
			{

				await global.CurrentRoom.ShowDialogue(TutorialDialogueResource, "tutorial_battle_0");
				global.GameDisplayEnabled = false;
			}

			Allies.StartTurn();
		}

		private void Clear()
		{
			global.PlayerData.Gold += 50;
			for (int i = 0; i < Allies.Characters.Count; i++)
			{
				CharacterData character = Allies.Characters[i];
				if (character.Health <= 0)
				{
					character.Health = 5;
				}
			}

			Allies.Clear();
			Enemies.Clear();
		}

		public async Task Routine()
		{
			invisButton.GrabFocus();

			List<CharacterBattleState> participants = new();
			participants.AddRange(Allies.BattleStates);
			participants.AddRange(Enemies.BattleStates);
			participants = participants.OrderByDescending((p) => p.Character.AgilityPoints).ToList();

			foreach (CharacterBattleState participant in participants)
			{
				switch (participant.Character.Type)
				{
					case CharacterType.Ally:
						await Allies.Turn(participant.InternalIndex);
						break;
					case CharacterType.Enemy:
						await Enemies.Turn(participant.InternalIndex);
						break;
				}

				if (Enemies.GetTotalHealth() <= 0)
				{
					HideDisplay();
					break;
				}
				else if (Allies.GetTotalHealth() <= 0)
				{
					IsBattleEnded = true;
					global.CurrentRoom.TransitionRect.PlayAnimation();
					await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);

					global.CurrentRoom.GameOverDisplay.ShowDisplay();
					Clear();
				}
				else
				{
					Allies.StartTurn();
				}
			}

			if (Visible)
			{
				EmitSignal(SignalName.TurnFinished);
			}
		}

		public override async void HideDisplay()
		{
			IsBattleEnded = true;
			IsTutorial = false;
			battleOptions.ShowInfoLabel("You won! 50 Gold obtained.");

			await StartTransition();
			
			Hide();
			battleOptions.HideDisplay();	
			
			Clear();

			global.CanWalk = true;
			global.GameDisplayEnabled = true;
			await EndTransition();
			EmitSignal(SignalName.BattleEnded);
		}

		private async Task StartTransition()
		{
			SceneTreeTimer timer = GetTree().CreateTimer(2);
			await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
			global.CurrentRoom.TransitionRect.PlayAnimation();
			await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
		}

		private async Task EndTransition()
		{
			global.CurrentRoom.TransitionRect.PlayAnimationBackwards();
		}
	}
}