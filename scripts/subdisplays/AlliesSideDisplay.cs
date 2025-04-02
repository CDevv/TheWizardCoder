using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Data;
using TheWizardCoder.UI;

namespace TheWizardCoder.Subdisplays
{
    public partial class AlliesSideDisplay : BattleSideDisplay
    {
        private CharacterCardsList cardsList;

        [Export]
        public DamageIndicator DamageIndicator { get; set; }

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

        public override async Task DisplayBattleEffect(int index)
        {
            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

            BattleEffect battleEffect = Characters.BattleStates[index].BattleEffect;

            cardsList[index].ShowEffectIndicator(battleEffect.Action, battleEffect.Effect, battleEffect.IsNegative);
            cardsList[index].UpdateTurnsLabel(battleEffect.Turns);
        }

        public override async Task DisplayHealthChange(int index, int change)
        {
            Character character = Characters[index];

            character.ChangeHealth(change);
            await cardsList.TweenDamage(index, character.Health, character.MaxHealth, change);
        }

        public override async Task DisplayManaChange(int index, int change)
        {
            Character character = Characters[index];

            character.ChangeMana(change);
            await cardsList.TweenManaChange(index, character.Points, change);

            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

        public void StartTurn()
        {
            cardsList[CurrentCharacter].ShowAsCurrentCharacter();
            BattleOptions.UpdateDisplay(Characters[CurrentCharacter]);
            BattleOptions.ShowOptions();
        }
    }
}
