using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;

namespace TheWizardCoder.Subdisplays
{
    public abstract partial class BattleSideDisplay : Display
    {
        [Export]
        public BattleDisplay BattleDisplay { get; set; }
        [Export]
        public BattleOptions BattleOptions { get; set; }

        public BattleSide Characters { get; protected set; }
        public int CurrentCharacter { get; protected set; }
        public List<int> CollectedExperience { get; protected set; }

        public override void _Ready()
        {
            base._Ready();
            Characters = new BattleSide();
            CollectedExperience = new List<int>();
        }

        public override void ShowDisplay()
        {
            Show();
        }

        public void AddExperience(int index, int experience)
        {
            CollectedExperience[index] += experience;
        }

        public void AddExperience(Character character, int experience)
        {
            int index = Characters.Characters.IndexOf(character);
            CollectedExperience[index] += experience;
        }

        public void AwardExperience()
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].AddLevelPoints(CollectedExperience[i]);
            }
        }

        public abstract void AddCharacter(Character character);
        public abstract void FocusOnFirst();
        public abstract Task ChangeHealth(int index, int change);
        public abstract Task ChangeMana(int index, int change);
        public abstract Task ApplyBattleEffect(int index, BattleEffect battleEffect);
        public abstract void RemoveBattleEffect(int index);
        public abstract void UpdateBattleEffect(int index);
        public abstract Task DefendCharacter(int index);
        public async Task OnTurn(int index)
        {
            if (BattleDisplay.IsBattleEnded)
            {
                return;
            }

            Character character = Characters[index];
            CharacterBattleState state = Characters.GetCharacterState(character);

            if (character.Health <= 0)
            {
                return;
            }

            if (state.HasBattleEffect)
            {
                state.BattleEffect.Turns--;
                UpdateBattleEffect(index);

                if (state.BattleEffect.Turns == 0)
                {
                    RemoveBattleEffect(index);
                }
            }

            await SelectAction(index);
        }

        public abstract Task SelectAction(int index);

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
        public abstract void StartTurn();
        public abstract void Clear();
    }
}
