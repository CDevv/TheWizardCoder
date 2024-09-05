using DialogueManagerRuntime;
using Godot;
using Godot.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class BattleDisplay : CanvasLayer
{
	private Vector2 startingPoint = new Vector2(16, 408);

	[Export]
	public PackedScene CharacterRectScene { get; set; }
	[Export]
	public PackedScene EnemySpriteScene { get; set; }
	[Export]
	public AlliesContainer Allies { get; set; }
	[Export]
	public EnemiesContainer Enemies { get; set; }

	public bool BattleEnded { get; private set; } = false;

	private Global global;
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
		global = GetNode<Global>("/root/Global");
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

	public async void ShowDisplay(Array<string> enemies)
	{
		BattleEnded = false;
		//Add allies
		Allies.AddAlly(global.PlayerData.Stats);
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
		global.CurrentRoom.TransitionRect.PlayAnimationBackwards();

		Allies.StartTurn();
	}

	private void Clear()
	{
		Allies.Clear();
		Enemies.Clear();
	}

	public async Task Routine()
	{
		invisButton.GrabFocus();

		await Allies.AlliesTurn();
		await Enemies.EnemiesTurn();

		if (Enemies.GetTotalHealth() <= 0)
		{
			HideDisplay();
		}
		else if (Allies.GetTotalHealth() <= 0)
		{
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

	public async void HideDisplay()
	{
		BattleEnded = true;
		global.CurrentRoom.TransitionRect.PlayAnimation();
		await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
		Hide();
		Clear();
		global.CanWalk = true;
		global.GameDisplayEnabled = true;

		global.CurrentRoom.TransitionRect.PlayAnimationBackwards();
	}
}
