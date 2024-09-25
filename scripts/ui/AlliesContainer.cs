using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheWizardCoder.Autoload;
using TheWizardCoder.Enums;
using TheWizardCoder.Displays;
using TheWizardCoder.Subdisplays;
using TheWizardCoder.Data;

namespace TheWizardCoder.UI
{
	public partial class AlliesContainer : Node
	{
		[Signal]
		public delegate void AllyPressedEventHandler(int index);

		[Export]
		public EnemiesContainer Enemies { get; set; }
		[Export]
		public DamageIndicator DamageIndicator { get; set; }
		[Export]
		public BattleOptions BattleOptions { get; set; }
		[Export]
		public BattleDisplay BattleDisplay { get; set; }
		[Export]
		public Marker2D BaseCardPosition { get; set; }
		[Export]
		public PackedScene CharacterRectScene { get; set; }

		private Global global;
		private Vector2 startingPoint = Vector2.Zero;
		private int currentCharacter = 0;
		private Array<CharacterRect> alliesCards = new();
		private List<CharacterBattleState> allies = new();

		public List<CharacterBattleState> Characters 
		{
			get { return allies; }	
		}

		public override void _Ready()
		{
			global = GetNode<Global>("/root/Global");
			startingPoint = BaseCardPosition.Position;
		}

		public void AddAlly(CharacterData data)
		{
			int currentIndex = allies.Count;

			CharacterRect rect = CharacterRectScene.Instantiate<CharacterRect>();
			rect.Position = startingPoint - new Vector2(0, allies.Count * (rect.Size.Y + 4) * 2);
			GD.Print(allies.Count * (rect.Size.Y));
			BattleDisplay.AddChild(rect);
			rect.ApplyData(data);
			rect.Pressed += () => OnCharacterCardPressed(currentIndex);
			alliesCards.Add(rect);

			CharacterBattleState state = new(data, currentIndex);
			allies.Add(state);
		}

		public void StartTurn()
		{
			alliesCards[currentCharacter].ShowAsCurrentCharacter();
			BattleOptions.UpdateDisplay(allies[currentCharacter].Character);
			BattleOptions.ShowOptions();
		}

		public void FocusOnFirst()
		{
			alliesCards[0].GrabFocus();
		}

		public async void PassToNextAlly()
		{
			currentCharacter++;
			if (currentCharacter >= allies.Count)
			{
				alliesCards[currentCharacter-1].HideBackground();
				currentCharacter = 0;
				await BattleDisplay.Routine();
			}
			else
			{
				alliesCards[currentCharacter-1].HideBackground();
				StartTurn();
			}
		}

		public async Task AllyTurn(int i)
		{
			if (BattleDisplay.BattleEnded)
			{
				return;
			}

			CharacterBattleState state = allies[i];
			CharacterData ally = state.Character;
			switch (allies[i].Action)
			{
				case CharacterAction.Attack:
					BattleOptions.ShowInfoLabel($"{ally.Name} attacks {Enemies.GetEnemyName(state.Target)}!");
					await Enemies.DamageEnemy(state.Target, ally.AttackPoints);
					break;
				case CharacterAction.Defend:
					await DefendAlly(i);
					break;
				case CharacterAction.Items:
					await HealAlly(i);
					break;
				case CharacterAction.Magic:
					string currentMagicSpellName = ally.MagicSpells[allies[i].ActionModifier];
					MagicSpell currentMagicSpell = global.MagicSpells[currentMagicSpellName];
					if (currentMagicSpell.TargetType == CharacterType.Enemy)
					{
						string enemyName = Enemies.GetEnemyName(state.Target);
						BattleOptions.ShowInfoLabel($"{ally.Name} casted {currentMagicSpellName} on {enemyName}!");
						await Enemies.DamageEnemy(state.Target, currentMagicSpell.Effect);
					}
					break;
			}
		}

		public async Task AlliesTurn()
		{
			for (int i = 0; i < allies.Count; i++)
			{
				if (BattleDisplay.BattleEnded)
				{
					break;
				}

				CharacterBattleState state = allies[i];
				CharacterData ally = state.Character;
				switch (allies[i].Action)
				{
					case CharacterAction.Attack:
						BattleOptions.ShowInfoLabel($"{ally.Name} attacks {Enemies.GetEnemyName(state.Target)}!");
						await Enemies.DamageEnemy(state.Target, ally.AttackPoints);
						break;
					case CharacterAction.Defend:
						await DefendAlly(i);
						break;
					case CharacterAction.Items:
						await HealAlly(i);
						break;
					case CharacterAction.Magic:
						string currentMagicSpellName = ally.MagicSpells[allies[i].ActionModifier];
						MagicSpell currentMagicSpell = global.MagicSpells[currentMagicSpellName];
						if (currentMagicSpell.TargetType == CharacterType.Enemy)
						{
							string enemyName = Enemies.GetEnemyName(state.Target);
							BattleOptions.ShowInfoLabel($"{ally.Name} casted {currentMagicSpellName} on {enemyName}!");
							await Enemies.DamageEnemy(state.Target, currentMagicSpell.Effect);
						}
						break;
				}
			}
		}

		private async Task DefendAlly(int index)
		{
			BattleOptions.ShowInfoLabel($"{allies[index].Character.Name} defends!");
			SceneTreeTimer timer = GetTree().CreateTimer(3);
			await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		}

		public async Task HealAlly(int healerIndex)
		{
			CharacterBattleState state = allies[healerIndex];
			string itemName = global.PlayerData.Inventory[state.ActionModifier];
			Item item = global.ItemDescriptions[itemName];
			CharacterData target = allies[state.Target].Character;
			if (state.Target == healerIndex)
			{
				BattleOptions.ShowInfoLabel($"{state.Character.Name} gave {itemName} to themselves!");
			}
			else
			{
				BattleOptions.ShowInfoLabel($"{state.Character.Name} gave {itemName} to {target.Name}!");
			}
			
			int newHealth = Mathf.Clamp(target.Health + item.Effect, target.Health, target.MaxHealth);
			allies[state.Target].Character.Health = newHealth;

			await alliesCards[state.Target].TweenDamage(new Color(0, 255, 0));
			DamageIndicator.PlayAnimation(target.MaxHealth - newHealth, alliesCards[state.Target].Position + new Vector2(64, 0), new Color(0, 255, 0));
			
			alliesCards[state.Target].SetHealthValue(newHealth);

			global.PlayerData.RemoveFromInventory(state.ActionModifier);
			
			SceneTreeTimer timer = GetTree().CreateTimer(3);
			await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		}

		public async Task DamageRandomAlly(string enemyName, int damage)
		{
			int targetIndex = (int)(GD.Randi() % allies.Count);

			if (allies[targetIndex].Action == CharacterAction.Defend)
			{
				damage = Mathf.Clamp(damage - allies[targetIndex].Character.DefensePoints, 0, damage);
			}
			allies[targetIndex].Character.Health -= damage;

			//Visuals
			BattleOptions.ShowInfoLabel($"{enemyName} attacks {allies[targetIndex].Character.Name}!");
			DamageIndicator.PlayAnimation(damage, alliesCards[targetIndex].Position + new Vector2(64, 0), new Color(255, 0, 0));
			alliesCards[targetIndex].SetHealthValue(global.PlayerData.Stats.Health);
			await alliesCards[targetIndex].TweenDamage(new Color(255, 0, 0));
		}

		private void OnCharacterCardPressed(int index)
		{
			allies[currentCharacter].Target = index;
			PassToNextAlly();
			EmitSignal(SignalName.AllyPressed, index);
		}

		private void OnEnemyPressed(int index)
		{
			allies[currentCharacter].Target = index;
			if (allies[currentCharacter].Action == CharacterAction.Magic)
			{
				int indexInInventory = allies[currentCharacter].ActionModifier;
				
				string magicSpellName = allies[currentCharacter].Character.MagicSpells[indexInInventory];
				MagicSpell magicSpell = global.MagicSpells[magicSpellName];

				allies[currentCharacter].Character.Points -= magicSpell.Cost;
				alliesCards[currentCharacter].SetPointsValue(allies[currentCharacter].Character.Points);
			}

			PassToNextAlly();
		}

		private void OnAttackButton()
		{
			allies[currentCharacter].Action = CharacterAction.Attack;
			Enemies.FocusOnFirst();
			BattleOptions.ShowInfoLabel("Select an enemy!");
		}

		private void OnDefendButton()
		{
			allies[currentCharacter].Action = CharacterAction.Defend;
			PassToNextAlly();
		}

		private void OnItemButton(int index)
		{
			allies[currentCharacter].Action = CharacterAction.Items;
			allies[currentCharacter].ActionModifier = index;
			alliesCards[0].GrabFocus();
			BattleOptions.ShowInfoLabel("Select an ally!");
		}

		private void OnMagicButton(int index)
		{
			CharacterData ally = allies[currentCharacter].Character;

			allies[currentCharacter].Action = CharacterAction.Magic;
			allies[currentCharacter].ActionModifier = index;

			string magicSpellName = ally.MagicSpells[index];
			MagicSpell magicSpell = global.MagicSpells[magicSpellName];
			if (magicSpell.TargetType == CharacterType.Enemy)
			{
				Enemies.FocusOnFirst();
				BattleOptions.ShowInfoLabel("Select an enemy to cast on!");
			}
		}

		public int GetTotalHealth()
		{
			int result = 0;
			foreach (CharacterBattleState ally in allies)
			{
				result += ally.Character.Health;
			}
			return result;
		}

		public void Clear()
		{
			allies.Clear();
			alliesCards.Clear();
			allies.Clear();
		}
	}
}