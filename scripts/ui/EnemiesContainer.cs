using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;

namespace TheWizardCoder.UI
{
	public partial class EnemiesContainer : CharactersContainer
	{
		[Signal]
		public delegate void EnemyPressedEventHandler(int index);

		[Export]
		public AlliesContainer Allies { get; set; }
		[Export]
		public EnemyHealthBar EnemyHealthBar { get; set; }
		[Export]
		public DamageIndicator DamageIndicator { get; set; }
		[Export]
		public Marker2D BaseEnemyPosition { get; set; }
		[Export]
		public PackedScene EnemySpriteScene { get; set; }

		private Vector2 enemySpritePoint = Vector2.Zero;

		private Array<EnemySprite> enemySprites = new();


		public override void _Ready()
		{
			base._Ready();
			enemySpritePoint = BaseEnemyPosition.Position;
		}

		public override void AddCharacter(CharacterData character)
		{
			int currentIndex = Characters.Count;

			Characters.Add(character.Clone());
            CharacterBattleState characterBattleState = new(character, currentIndex);
            BattleStates.Add(characterBattleState);

			EnemySprite enemySprite = EnemySpriteScene.Instantiate<EnemySprite>();
			BattleDisplay.AddChild(enemySprite);

			enemySprite.Position = enemySpritePoint;
			enemySprite.ApplyData(character);
			enemySprite.ButtonPressed += () => {			
				EmitSignal(SignalName.EnemyPressed, currentIndex);
			};

			enemySprites.Add(enemySprite);
		}

		public void FocusOnFirst()
		{
			enemySprites[0].GrabFocus();
		}

		public override async Task Turn(int index)
		{
			if (BattleDisplay.IsBattleEnded)
			{
				return;
			}

			int targetIndex = (int)(GD.Randi() % Allies.Characters.Count);
			string enemyName = Characters[index].Name;

			BattleOptions.ShowInfoLabel($"{enemyName} attacks {Allies.Characters[targetIndex].Name}!");
			await Allies.DamageCharacter(targetIndex, Characters[index].AttackPoints);
		}

		public override void OnNextCharacterPassed()
        {
            throw new NotImplementedException();
        }

		public override async Task DamageCharacter(int index, int damage)
		{
			await DisplayHealthChange(index, damage);
			await base.DamageCharacter(index, damage);
		}

		public override async Task DisplayHealthChange(int index, int change)
		{
			CharacterData enemyData = Characters[index];
			EnemySprite sprite = enemySprites[index];

			DamageIndicator.PlayAnimation(change, sprite.Position - new Vector2(DamageIndicator.Size.X, 10), new Color(255, 0, 0));

			Vector2 barPosition = sprite.Position - new Vector2(EnemyHealthBar.Size.X/2, -20);
			EnemyHealthBar.Position = barPosition;
			await EnemyHealthBar.ShowHealthBar(enemyData.Health, enemyData.Health - change, enemyData.MaxHealth);
		}

		public override void Clear()
		{
			base.Clear();
			foreach (var item in enemySprites)
			{
				item.QueueFree();
			}
			enemySprites = new();
		}
    }
}