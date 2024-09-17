using DialogueManagerRuntime;
using Godot;
using Godot.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public partial class BattleDisplay : Display
{
	private Vector2 startingPoint = new Vector2(16, 408);

	[Signal]
	public delegate void BattleFinishedEventHandler();
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

	public bool BattleEnded { get; private set; } = false;
	public bool IsTutorial { get; set; } = false;


	private AlliesContainer alliesContainer;
	private EnemiesContainer enemiesContainer;
	private BattleOptions battleOptions;
	private EnemyHealthBar enemyHealthContainer;
	private TextureProgressBar enemyHealthBar;
	private DamageIndicator damageIndicator;
	private Marker2D enemySpritePoint;
	private Button invisButton;

	public override void _Ready()
	{
		base._Ready();
		battleOptions = GetNode<BattleOptions>("BattleOptions");
		enemyHealthContainer = GetNode<EnemyHealthBar>("EnemyHealthBar");
		damageIndicator = GetNode<DamageIndicator>("DamageIndicator");
		enemySpritePoint = GetNode<Marker2D>("EnemySpritePoint");
		invisButton = GetNode<Button>("InvisButton");
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
        ShowDisplay(new() {"Glitch"});
    }

    public async void ShowDisplay(Array<string> enemies)
	{
		BattleEnded = false;
		//Add allies
		Allies.AddAlly(global.PlayerData.Stats);

		//Add enemies
		foreach (string enemyName in enemies)
		{
			CharacterData newEnemy = global.Characters[enemyName];
			Enemies.AddEnemy(newEnemy);
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

    public override void UpdateDisplay()
    {
        throw new NotImplementedException();
    }

    private void Clear()
	{
		Allies.Clear();
		Enemies.Clear();
	}

	public async Task Routine()
	{
		invisButton.GrabFocus();

		List<CharacterBattleState> participants = new();
		participants.AddRange(Allies.Characters);
		participants.AddRange(Enemies.Characters);
		participants = participants.OrderByDescending((p) => p.Character.AgilityPoints).ToList();

		foreach (CharacterBattleState participant in participants)
		{
			switch (participant.Character.Type)
			{
				case CharacterType.Ally:
					await Allies.AllyTurn(participant.InternalIndex);
					break;
				case CharacterType.Enemy:
					await Enemies.EnemyTurn(participant.InternalIndex);
					break;
			}

			if (Enemies.GetTotalHealth() <= 0)
			{
				HideDisplay();
			}
			else if (Allies.GetTotalHealth() <= 0)
			{
				BattleEnded = true;
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
		BattleEnded = true;
		IsTutorial = false;
		global.CurrentRoom.TransitionRect.PlayAnimation();
		await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
		Hide();
		battleOptions.HideDisplay();
		Clear();
		global.CanWalk = true;
		global.GameDisplayEnabled = true;

		global.CurrentRoom.TransitionRect.PlayAnimationBackwards();
		EmitSignal(SignalName.BattleFinished);
	}
}
