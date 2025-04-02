using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Data;
using TheWizardCoder.Tests;
using TheWizardCoder.UI;

namespace TheWizardCoder.Subdisplays
{
    public partial class AlliesSideDisplay : BattleSideDisplay
    {
        private CharacterCardsList cardsList;

        [Export]
        public DamageIndicator DamageIndicator { get; set; }

        public CharacterCardsList CardsList => cardsList;

        public override void _Ready()
        {
            base._Ready();
            cardsList = GetNode<CharacterCardsList>("CardsList");
            cardsList.DamageIndicator = DamageIndicator;
        }

        public override void AddCharacter(Character character)
        {
            Characters.AddCharacter(character);
            cardsList.AddCharacter(character);
        }

        public override async Task DisplayBattleEffect(int index, BattleEffect battleEffect)
        {
            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

            Characters.ApplyBattleEffect(index, battleEffect);

            cardsList[index].ShowEffectIndicator(battleEffect.Action, battleEffect.Effect, battleEffect.IsNegative);
            cardsList[index].UpdateTurnsLabel(battleEffect.Turns);
        }

        public override async Task DisplayHealthChange(int index, int change)
        {
            Characters.ChangeHealth(index, change);
            await cardsList.TweenDamage(index, change);
        }

        public override async Task DisplayManaChange(int index, int change)
        {
            Characters.ChangeMana(index, change);
            await cardsList.TweenManaChange(index, change);

            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

        public void StartTurn()
        {
            cardsList[CurrentCharacter].ShowAsCurrentCharacter();
            BattleOptions.UpdateDisplay(Characters[CurrentCharacter]);
            BattleOptions.ShowOptions();
        }

        public override async Task SelectAction(CharacterBattleState state)
        {
            Character character = state.Character;

            switch (state.Action)
            {
                case Enums.CharacterAction.Attack:
                    break;
                case Enums.CharacterAction.Defend:
                    break;
                case Enums.CharacterAction.Items:
                    break;
                case Enums.CharacterAction.Magic:
                    break;
                default:
                    break;
            }
        }
    }
}
