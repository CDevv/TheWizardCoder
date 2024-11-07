using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;
using TheWizardCoder.Subdisplays;

namespace TheWizardCoder.Abstractions
{
	public abstract partial class CharactersContainer : Node
	{
		[Export]
		public BattleDisplay BattleDisplay { get; set; }
		[Export]
		public BattleOptions BattleOptions { get; set; }

		public List<CharacterData> Characters { get; private set; } = new();
		public List<CharacterBattleState> BattleStates { get; set; } = new();
		public int CurrentCharacter { get; set; } = 0;
		protected Global global;

		public override void _Ready()
		{
			global = GetNode<Global>("/root/Global");
		}

		public virtual void AddCharacter(CharacterData character)
		{
			int currentIndex = Characters.Count;

			Characters.Add(character);
			CharacterBattleState characterBattleState = new(character, currentIndex);
			BattleStates.Add(characterBattleState);
		}

		public virtual void StartTurn()
		{}

		public async void PassToNext()
		{
			CurrentCharacter++;
			if (CurrentCharacter >= Characters.Count)
			{
				OnNextCharacterPassed();
				CurrentCharacter = 0;
				await BattleDisplay.Routine();
			}
			else
			{
				OnNextCharacterPassed();
				if (Characters[CurrentCharacter].Health > 0)
				{
					StartTurn();
				}
				else
				{
					CurrentCharacter++;

					if (CurrentCharacter < Characters.Count)
					{
						StartTurn();
					}
					else
					{
						CurrentCharacter = 0;
						await BattleDisplay.Routine();
					}
				}
			}
		}

		public abstract void OnNextCharacterPassed();

		public abstract Task Turn(int index);

		public abstract Task DisplayHealthChange(int index, int change);

		public virtual async Task DamageCharacter(int index, int damage)
		{
			Characters[index].Health -= damage;
		}

		public async Task HealCharacter(int healerIndex)
		{
			CharacterBattleState state = BattleStates[healerIndex];
			string itemName = global.PlayerData.Inventory[state.ActionModifier];
			Item item = global.ItemDescriptions[itemName];
			CharacterData target = BattleStates[state.Target].Character;

			if (state.Target == healerIndex)
			{
				BattleOptions.ShowInfoLabel($"{state.Character.Name} gave {itemName} to themselves!");
			}
			else
			{
				BattleOptions.ShowInfoLabel($"{state.Character.Name} gave {itemName} to {target.Name}!");
			}
			
			int healthChange = Mathf.Clamp(item.Effect, 0, target.MaxHealth - target.Health);
			Characters[state.Target].AddHealth(healthChange);
			global.PlayerData.RemoveFromInventory(state.ActionModifier);

			await DisplayHealthChange(state.Target, healthChange);

			SceneTreeTimer timer = GetTree().CreateTimer(3);
			await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		}

		public async Task DefendCharacter(int index)
		{
			BattleOptions.ShowInfoLabel($"{Characters[index].Name} defends!");
			SceneTreeTimer timer = GetTree().CreateTimer(3);
			await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		}

		public int GetTotalHealth()
		{
			int result = 0;
			foreach (var item in Characters)
			{
				result += item.Health;
			}
			return result;
		}

		public virtual void Clear()
		{
			Characters = new();
			BattleStates = new();
		}
	}
}
