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
using TheWizardCoder.Abstractions;
using System.Linq;

namespace TheWizardCoder.UI
{
	public partial class AlliesContainer : CharactersContainer
	{
		[Signal]
		public delegate void AllyPressedEventHandler(int index);

		[Export]
		public EnemiesContainer Enemies { get; set; }
		[Export]
		public DamageIndicator DamageIndicator { get; set; }

		[Export]
		public Marker2D BaseCardPosition { get; set; }
		[Export]
		public PackedScene CharacterRectScene { get; set; }

		private Vector2 startingPoint = Vector2.Zero;
		private Array<CharacterRect> alliesCards = new();
		private System.Collections.Generic.Dictionary<int, int> collectedExperience = new();

		public System.Collections.Generic.Dictionary<int, int> CollectedExperiences
		{
			get { return collectedExperience; }
		}

		public override void _Ready()
		{
			base._Ready();
			startingPoint = BaseCardPosition.Position;
		}

        public override void AddCharacter(Character character)
        {
            base.AddCharacter(character);

			int currentIndex = Characters.Count - 1;

			CharacterRect rect = CharacterRectScene.Instantiate<CharacterRect>();
			BattleDisplay.AddChild(rect);

			rect.Position = startingPoint - new Vector2(0, currentIndex * (rect.Size.Y + 4) * 2);
			rect.ApplyData(character);
			rect.Pressed += () => OnCharacterCardPressed(currentIndex);

            if (alliesCards.Count >= 1)
            {
                rect.FocusNeighborBottom = alliesCards[alliesCards.Count - 1].GetPath();
				alliesCards[alliesCards.Count - 1].FocusNeighborTop = rect.GetPath();

				rect.FocusNeighborTop = alliesCards[0].GetPath();
                alliesCards[0].FocusNeighborBottom = rect.GetPath();
            }
            

			alliesCards.Add(rect);
			collectedExperience[currentIndex] = 0;
        }

        public override void StartTurn()
		{
			alliesCards[CurrentCharacter].ShowAsCurrentCharacter();
			BattleOptions.UpdateDisplay(Characters[CurrentCharacter]);
			BattleOptions.ShowOptions();
		}

		public void FocusOnFirst()
		{
			alliesCards[0].GrabFocus();
		}

        public override void OnNextCharacterPassed()
        {
            alliesCards[CurrentCharacter - 1].HideBackground();
        }

        public async override Task Turn(int i)
		{
			if (BattleDisplay.IsBattleEnded)
			{
				return;
			}

			CharacterBattleState state = BattleStates[i];
			Character ally = state.Character;

			if (state.HasBattleEffect)
			{
				BattleStates[i].BattleEffect.Turns--;
				alliesCards[i].UpdateTurnsLabel(BattleStates[i].BattleEffect.Turns);

                if (BattleStates[i].BattleEffect.Turns == 0)
                {
					RemoveBattleEffect(i);
                }
            }

			if (ally.Health <= 0)
			{
				return;
			}

			switch (state.Action)
			{
				case CharacterAction.Attack:
					BattleOptions.ShowInfoLabel($"{ally.Name} attacks {Enemies.Characters[state.Target].Name}!");
					await Enemies.DamageCharacter(state.Target, ally.AttackPoints);

					collectedExperience[i] += ally.Level;
					break;
				case CharacterAction.Defend:
					await DefendCharacter(i);
					break;
				case CharacterAction.Items:
					int healerIndex = i;
                    string itemName = global.PlayerData.Inventory[state.ActionModifier];
                    Item item = global.ItemDescriptions[itemName];
                    Character target = BattleStates[state.Target].Character;

                    if (state.Target == healerIndex)
                    {
                        BattleOptions.ShowInfoLabel($"{state.Character.Name} gave {itemName} to themselves!");
                    }
                    else
                    {
                        BattleOptions.ShowInfoLabel($"{state.Character.Name} gave {itemName} to {target.Name}!");
                    }

                    switch (item.Type)
					{
						case ItemType.Heal:
                            await HealCharacter(state.Target, item.Effect);

                            break;
						case ItemType.Magic:
                            BattleEffectType action = Enum.Parse<BattleEffectType>(item.AdditionalData[0]);
                            CharacterType targetType = Enum.Parse<CharacterType>(item.AdditionalData[1]);
                            int turns = int.Parse(item.AdditionalData[2]);
                            int effect = int.Parse(item.AdditionalData[3]);

                            BattleEffect battleEffect = new BattleEffect(action, targetType, turns, effect);
							await ApplyBattleEffect(state.Target, battleEffect);

							break;
						case ItemType.Mana:
							await AddManaToCharacter(state.Target, item.Effect);

							break;
					}

					global.PlayerData.RemoveFromInventory(state.ActionModifier);
                    break;
				case CharacterAction.Magic:
					string currentMagicSpellName = ally.MagicSpells[state.ActionModifier];
					MagicSpell currentMagicSpell = global.MagicSpells[currentMagicSpellName];

                    ally.RemoveMana(currentMagicSpell.Cost);
                    alliesCards[i].SetPointsValue(ally.Points);

                    if (currentMagicSpell.TargetType == CharacterType.Enemy)
					{
						string enemyName = Enemies.Characters[state.Target].Name;
						BattleOptions.ShowInfoLabel($"{ally.Name} casted {currentMagicSpellName} on {enemyName}!");

						await Enemies.DamageCharacter(state.Target, currentMagicSpell.Effect);
					}
					else if (currentMagicSpell.TargetType == CharacterType.Ally)
                    {
						string healedAllyName = Characters[state.Target].Name;

                        if (ally.Name == healedAllyName)
                        {
                            BattleOptions.ShowInfoLabel($"{ally.Name} casted {currentMagicSpellName} on themselves!");
                        }
                        else
                        {
                            BattleOptions.ShowInfoLabel($"{ally.Name} casted {currentMagicSpellName} on {healedAllyName}!");
                        }

                        await HealCharacter(state.Target, currentMagicSpell.Effect);
                    }

                    collectedExperience[i] += (ally.Level + 2);

                    break;
			}
		}

        public override async Task DamageCharacter(int index, int damage)
        {
			CharacterBattleState battleState = BattleStates[index];

			if (battleState.Character.Health <= 0)
			{
				while (battleState.Character.Health <= 0)
				{
					index++;

					if (index >= BattleStates.Count)
					{
						index = 0;
					}

					battleState = BattleStates[index];
				}
			}

			if (battleState.Action == CharacterAction.Defend)
			{
				damage = Mathf.Clamp(damage - battleState.Character.DefensePoints, 0, damage);
			}

            base.DamageCharacter(index, damage);
			await DisplayHealthChange(index, -damage);
        }

		public override async Task DisplayHealthChange(int index, int healthChange)
		{
			Color backgroundColor = new(0, 255, 0);
			if (healthChange < 0)
			{
				backgroundColor = new(255, 0, 0);
			}

			alliesCards[index].SetHealthValue(Characters[index].Health, Characters[index].MaxHealth);
			DamageIndicator.PlayAnimation(healthChange, alliesCards[index].Position + new Vector2(64, 0), backgroundColor);
			await alliesCards[index].TweenDamage(backgroundColor);
		}

        public override async Task DisplayManaChange(int index, int change)
        {
			Color backgroundColor = new("ff30ff00");

			alliesCards[index].SetPointsValue(Characters[index].Points);
            DamageIndicator.PlayAnimation(change, alliesCards[index].Position + new Vector2(64, 0), backgroundColor);
            await alliesCards[index].TweenDamage(backgroundColor);
        }

        public override async Task DisplayBattleEffect(int index)
        {
            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

			BattleEffect battleEffect = BattleStates[index].BattleEffect;

            alliesCards[index].ShowEffectIndicator(battleEffect.Action, battleEffect.Effect);
			alliesCards[index].UpdateTurnsLabel(battleEffect.Turns);
        }

        public override void HideBattleEffect(int index)
        {
            alliesCards[index].HideEffectIndicator();
        }

        private void OnCharacterCardPressed(int index)
		{
			BattleStates[CurrentCharacter].Target = index;
			PassToNext();
			EmitSignal(SignalName.AllyPressed, index);
		}

		private void OnEnemyPressed(int index)
		{
			BattleStates[CurrentCharacter].Target = index;

            PassToNext();
		}

		private void OnAttackButton()
		{
			BattleStates[CurrentCharacter].Action = CharacterAction.Attack;
			Enemies.FocusOnFirst();
			BattleOptions.ShowInfoLabel("Select an enemy!");
		}

		private void OnDefendButton()
		{
			BattleStates[CurrentCharacter].Action = CharacterAction.Defend;
			PassToNext();
		}

		private void OnItemButton(int index, string itemName)
		{
			Item item = global.ItemDescriptions[itemName];

            if (item.Type != ItemType.Key)
            {
                BattleStates[CurrentCharacter].Action = CharacterAction.Items;
                BattleStates[CurrentCharacter].ActionModifier = index;
                alliesCards[0].GrabFocus();
                BattleOptions.ShowInfoLabel("Select an ally!");
            }
        }

		private void OnMagicButton(int index)
		{
			Character ally = BattleStates[CurrentCharacter].Character;

			BattleStates[CurrentCharacter].Action = CharacterAction.Magic;
			BattleStates[CurrentCharacter].ActionModifier = index;

			string magicSpellName = ally.MagicSpells[index];
			MagicSpell magicSpell = global.MagicSpells[magicSpellName];
			if (magicSpell.TargetType == CharacterType.Enemy)
			{
				Enemies.FocusOnFirst();
				BattleOptions.ShowInfoLabel("Select an enemy to cast on!");
			}
            else
            {
				alliesCards.First().GrabFocus();
                BattleOptions.ShowInfoLabel("Select an ally to cast on!");
            }
        }

		public override void Clear()
		{
			base.Clear();
			alliesCards.Clear();
		}

		public void AwardExperience()
		{
			for (int i = 0; i < collectedExperience.Count; i++)
			{
				Characters[i].AddLevelPoints(collectedExperience[i]);
			}
		}
	}
}