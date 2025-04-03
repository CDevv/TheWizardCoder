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
    public partial class EnemiesSideDisplay : BattleSideDisplay
    {
        [Signal]
        public delegate void EnemyPressedEventHandler(int index);

        private CharacterCardsList cardsList;
        private EnemySpritesList sprites;

        [Export]
        public AlliesSideDisplay Allies { get; set; }
        [Export]
        public DamageIndicator DamageIndicator { get; set; }

        public CharacterCardsList CardsList => cardsList;

        public override void _Ready()
        {
            base._Ready();
            cardsList = GetNode<CharacterCardsList>("CardsList");
            CardsList.DamageIndicator = DamageIndicator;
            cardsList.NegativeY = true;

            sprites = GetNode<EnemySpritesList>("SpritesList");
            sprites.EnemyPressed += (int index) => EmitSignal(SignalName.EnemyPressed, index);
        }

        public override void AddCharacter(Character character)
        {
            Characters.AddCharacter(character);
            cardsList.AddCharacter(character);
            sprites.AddSprite(character);
        }

        public override Task ApplyBattleEffect(int index, BattleEffect battleEffect)
        {
            throw new NotImplementedException();
        }

        public override Task ChangeHealth(int index, int change)
        {
            throw new NotImplementedException();
        }

        public override Task ChangeMana(int index, int change)
        {
            throw new NotImplementedException();
        }

        public override Task DefendCharacter(int index)
        {
            throw new NotImplementedException();
        }

        public override void FocusOnFirst()
        {
            throw new NotImplementedException();
        }

        public override Task SelectAction(int index)
        {
            throw new NotImplementedException();
        }

        public override void OnNextCharacterPassed()
        {
            throw new NotImplementedException();
        }

        public override void StartTurn()
        {
            throw new NotImplementedException();
        }
    }
}
