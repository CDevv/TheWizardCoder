using DialogueManagerRuntime;
using Godot;
using Godot.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class BattleDisplay : CanvasLayer
{
	private Vector2 startingPoint = new Vector2(16, 408);

	[Export]
	public PackedScene CharacterRectScene { get; set; }
	[Export]
	public PackedScene EnemySpriteScene { get; set; }

	private Global global;
	private BattleOptions battleOptions;
	private EnemyHealthBar enemyHealthContainer;
	private TextureProgressBar enemyHealthBar;
	private DamageIndicator damageIndicator;
	private Marker2D enemySpritePoint;
	private Button invisButton;

	private Array<CharacterData> allies = new();
	private Array<CharacterData> enemies = new();
	private Array<CharacterRect> alliesCards = new();
	private Array<EnemySprite> enemySprites = new();
	private int selectedEnemy = 0;

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

	public async void ShowDisplay()
	{
		//Add allies
		AddCharacterRect(global.PlayerData.Stats);

		//Add enemies
		CharacterData newEnemy = new() {
			Name = "Glitch", Health = 100, MaxHealth = 100, Points = 0, MaxPoints = 0, AttackPoints = 20
		};
		AddEnemy(newEnemy);

		battleOptions.UpdateDisplay();
		global.CurrentRoom.TransitionRect.PlayAnimation();
		await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
		Show();
		global.CurrentRoom.TransitionRect.PlayAnimationBackwards();
		battleOptions.ShowOptions();
	}

	private void AddCharacterRect(CharacterData data)
	{
		CharacterRect rect = CharacterRectScene.Instantiate<CharacterRect>();
		rect.Position = startingPoint - new Vector2(0, allies.Count * 98);
		AddChild(rect);
		rect.ApplyData(data);
		alliesCards.Add(rect);

		allies.Add(data);
	}

	private void AddEnemy(CharacterData data)
	{
		int currentIndex = enemySprites.Count;

		EnemySprite enemySprite = EnemySpriteScene.Instantiate<EnemySprite>();
		enemySprite.Position = enemySpritePoint.Position;
		
		AddChild(enemySprite);
		enemySprite.ApplyData(data);
		enemySprite.ButtonPressed += () => {
			AttackEnemy(currentIndex);
		};
		enemySprites.Add(enemySprite);

		enemies.Add(data);
	}

	private void OnAttackButton()
	{
		enemySprites[0].GrabFocus();
		battleOptions.ShowInfoLabel("Select an enemy!");
	}

	private async void AttackEnemy(int index)
	{
		invisButton.GrabFocus();

		EnemySprite sprite = enemySprites[index];
		CharacterData enemyData = enemies[index];

		battleOptions.ShowInfoLabel($"Nolan used sword on {enemyData.Name}!");	
		SpawnDamageIndicator(allies[0].AttackPoints, sprite.Position - new Vector2(damageIndicator.Size.X, 10));

		Vector2 barPosition = sprite.Position - new Vector2(enemyHealthContainer.Size.X/2, -20);
		enemyHealthContainer.Position = barPosition;
		await enemyHealthContainer.ShowHealthBar(enemyData.Health, enemyData.Health - allies[0].AttackPoints, enemyData.MaxHealth);

		enemyData.Health -= allies[0].AttackPoints;
		enemies[index] = enemyData;

		EnemyTurn();
	}

	private async void EnemyTurn()
	{
		//Choose target
		int index = (int)(GD.Randi() % allies.Count);
		battleOptions.ShowInfoLabel($"Glitch used glitch on {allies[index].Name}");

		allies[index].Health -= enemies[0].AttackPoints;
		GD.Print(global.PlayerData.Stats.Health);

		//Visuals
		SpawnDamageIndicator(enemies[0].AttackPoints, alliesCards[index].Position + new Vector2(64, 0));
		alliesCards[index].SetHealthValue(global.PlayerData.Stats.Health);
		await alliesCards[index].TweenDamage();

		battleOptions.ShowOptions();
	}

	private void Reset()
	{
		foreach (CharacterData item in allies)
		{
			item.Free();
		}
		allies.Clear();

		foreach (CharacterData item in enemies)
		{
			item.Free();
		}
		enemies.Clear();
	}

	private void SpawnDamageIndicator(int damage, Vector2 position)
	{
		damageIndicator.Position = position;
		damageIndicator.Text = $"{damage}";
		damageIndicator.PlayAnimation();
	}
}
