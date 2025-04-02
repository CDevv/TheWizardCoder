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
        public abstract Task DisplayHealthChange(int index, int change);
        public abstract Task DisplayManaChange(int index, int change);
        public abstract Task DisplayBattleEffect(int index, BattleEffect battleEffect);
        public async Task OnTurn(int index)
        {
            if (BattleDisplay.IsBattleEnded)
            {
                return;
            }

            Character ally = Characters[index];
            CharacterBattleState state = Characters.GetCharacterState(ally);


            if (ally.Health <= 0)
            {
                return;
            }

            await SelectAction(state);
        }

        public abstract Task SelectAction(CharacterBattleState state);
    }
}
