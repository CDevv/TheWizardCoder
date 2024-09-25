using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;

namespace TheWizardCoder.UI
{
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
		private List<CharacterBattleState> enemies = new();

		public List<CharacterBattleState> Characters
		{
			get { return enemies; }
		}

		public override void _Ready()
		{
			global = GetNode<Global>("/root/Global");
			enemySpritePoint = BaseEnemyPosition.Position;
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

			CharacterBattleState state = new(data, currentIndex);
			enemies.Add(state);
		}

		public void FocusOnFirst()
		{
			enemySprites[0].GrabFocus();
		}

		public async Task EnemyTurn(int i)
		{
			if (BattleDisplay.BattleEnded)
			{
				return;
			}

			await Allies.DamageRandomAlly(enemies[i].Character.Name, enemies[i].Character.AttackPoints);
		}

		public async Task EnemiesTurn()
		{
			for (int i = 0; i < enemies.Count; i++)
			{
				if (BattleDisplay.BattleEnded)
				{
					break;
				}

				await Allies.DamageRandomAlly(enemies[i].Character.Name, enemies[i].Character.AttackPoints);
			}
		}

		public string GetEnemyName(int index)
		{
			return enemies[index].Character.Name;
		}

		public async Task DamageEnemy(int index, int damage)
		{
			CharacterData enemyData = enemies[index].Character;
			EnemySprite sprite = enemySprites[index];
			
			DamageIndicator.PlayAnimation(damage, sprite.Position - new Vector2(DamageIndicator.Size.X, 10), new Color(255, 0, 0));

			Vector2 barPosition = sprite.Position - new Vector2(EnemyHealthBar.Size.X/2, -20);
			EnemyHealthBar.Position = barPosition;
			await EnemyHealthBar.ShowHealthBar(enemyData.Health, enemyData.Health - damage, enemyData.MaxHealth);

			enemyData.Health -= damage;
			enemies[index].Character = enemyData;
		}

		public int GetTotalHealth()
		{
			int result = 0;
			foreach(CharacterBattleState state in enemies)
			{
				result += state.Character.Health;
			}
			return result;
		}

		public void Clear()
		{
			enemies.Clear();
			enemySprites.Clear();
		}
	}
}