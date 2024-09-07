using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class EnemiesContainer : Node
{
	[Signal]
	public delegate void EnemyPressedEventHandler(int index);

	[Export]
	public AlliesContainer Allies { get; set; }
	[Export]
	public BattleDisplay BattleDisplay { get; set; }
	[Export]
	public EnemyHealthBar EnemyHealthBar { get; set; }
	[Export]
	public DamageIndicator DamageIndicator { get; set; }
	[Export]
	public Marker2D BaseEnemyPosition { get; set; }
	[Export]
	public PackedScene EnemySpriteScene { get; set; }

	private Global global;
	private Vector2 enemySpritePoint = Vector2.Zero;

	private Array<EnemySprite> enemySprites = new();
	private List<CharacterData> enemies = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		enemySpritePoint = BaseEnemyPosition.Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddEnemy(CharacterData data)
	{
		int currentIndex = enemySprites.Count;

		EnemySprite enemySprite = EnemySpriteScene.Instantiate<EnemySprite>();
		enemySprite.Position = enemySpritePoint;
		
		BattleDisplay.AddChild(enemySprite);
		enemySprite.ApplyData(data);
		enemySprite.ButtonPressed += () => {			
			EmitSignal(SignalName.EnemyPressed, currentIndex);
		};
		enemySprites.Add(enemySprite);

		enemies.Add(data);
	}

	public void FocusOnFirst()
	{
		enemySprites[0].GrabFocus();
	}

	public async Task EnemiesTurn()
	{
		for (int i = 0; i < enemies.Count; i++)
		{
			if (BattleDisplay.BattleEnded)
			{
				break;
			}

			await Allies.DamageRandomAlly(enemies[i].Name, enemies[i].AttackPoints);
		}
	}

	public string GetEnemyName(int index)
	{
		return enemies[index].Name;
	}

	public async Task DamageEnemy(int index, int damage)
	{
		CharacterData enemyData = enemies[index];
		EnemySprite sprite = enemySprites[index];
		
		DamageIndicator.PlayAnimation(damage, sprite.Position - new Vector2(DamageIndicator.Size.X, 10), new Color(255, 0, 0));

		Vector2 barPosition = sprite.Position - new Vector2(EnemyHealthBar.Size.X/2, -20);
		EnemyHealthBar.Position = barPosition;
		await EnemyHealthBar.ShowHealthBar(enemyData.Health, enemyData.Health - damage, enemyData.MaxHealth);

		enemyData.Health -= damage;
		enemies[index] = enemyData;
	}

	public int GetTotalHealth()
	{
		int result = 0;
		foreach(CharacterData data in enemies)
		{
			result += data.Health;
		}
		return result;
	}

	public void Clear()
	{
		enemies.Clear();
		enemySprites.Clear();
	}
}
