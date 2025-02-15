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

		public List<Character> Characters { get; private set; } = new();
		public List<CharacterBattleState> BattleStates { get; set; } = new();
		public int CurrentCharacter { get; set; } = 0;
		protected Global global;

		public override void _Ready()
		{
			global = GetNode<Global>("/root/Global");
		}

		public virtual void AddCharacter(Character character)
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
		public abstract Task DisplayManaChange(int index, int change);

		public abstract Task DisplayBattleEffect(int index);

		public abstract void HideBattleEffect(int index);

		public virtual async Task DamageCharacter(int index, int damage)
		{
			Characters[index].RemoveHealth(damage);
		}

		public async Task HealCharacter(int targetIndex, int addedHealth)
		{
			CharacterBattleState state = BattleStates[targetIndex];

            int healthChange = Mathf.Clamp(addedHealth, 0, state.Character.MaxHealth - state.Character.Health);
			Characters[state.Target].AddHealth(healthChange);	

			await DisplayHealthChange(targetIndex, healthChange);

			SceneTreeTimer timer = GetTree().CreateTimer(3);
			await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		}

		public async Task DefendCharacter(int index)
		{
			BattleOptions.ShowInfoLabel($"{Characters[index].Name} defends!");
			SceneTreeTimer timer = GetTree().CreateTimer(3);
			await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		}

		public async Task AddManaToCharacter(int targetIndex, int addedMana)
		{
            CharacterBattleState state = BattleStates[targetIndex];

            int manaChange = Mathf.Clamp(addedMana, 0, state.Character.MaxPoints - state.Character.Points);
			Characters[targetIndex].Points += manaChange;

			await DisplayManaChange(targetIndex, manaChange);

            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

		public async Task ApplyBattleEffect(int targetIndex, BattleEffect effect)
		{
			BattleStates[targetIndex].BattleEffect = effect;
			BattleStates[targetIndex].HasBattleEffect = true;

			GD.Print(effect.Effect);

			switch (effect.Action)
			{
				case Enums.BattleEffectType.Attack:
					Characters[targetIndex].AttackPoints += effect.Effect;
					break;
				case Enums.BattleEffectType.Defense:
					Characters[targetIndex].DefensePoints += effect.Effect;
					break;
			}

            GD.Print(Characters[targetIndex].DefensePoints);

            await DisplayBattleEffect(targetIndex);
        }

		public void RemoveBattleEffect(int targetIndex)
		{
			BattleEffect effect = BattleStates[targetIndex].BattleEffect;

            switch (effect.Action)
            {
                case Enums.BattleEffectType.Attack:
                    Characters[targetIndex].AttackPoints -= effect.Effect;
                    break;
                case Enums.BattleEffectType.Defense:
                    Characters[targetIndex].DefensePoints -= effect.Effect;
                    break;
            }

			GD.Print(Characters[targetIndex].DefensePoints);

			BattleStates[targetIndex].BattleEffect = null;
			BattleStates[targetIndex].HasBattleEffect = false;

			HideBattleEffect(targetIndex);
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
