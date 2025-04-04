using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using TheWizardCoder.UI;

namespace TheWizardCoder.Subdisplays
{
    public partial class EnemiesSideDisplay : BattleSideDisplay
    {
        [Signal]
        public delegate void EnemyPressedEventHandler(int index);

        private CharacterCardsList cardsList;
        private EnemySpritesList sprites;
        private EnemiesCommands commands;

        [Export]
        public AlliesSideDisplay Allies { get; set; }

        public CharacterCardsList CardsList => cardsList;

        public override void _Ready()
        {
            base._Ready();
            commands = new EnemiesCommands(global, this);

            cardsList = GetNode<CharacterCardsList>("CardsList");
            cardsList.NegativeY = true;

            sprites = GetNode<EnemySpritesList>("SpritesList");
            sprites.EnemyPressed += (int index) => EmitSignal(SignalName.EnemyPressed, index);
        }

        public override void AddCharacter(Character character)
        {
            Character enemy = character.Clone();

            Characters.AddCharacter(enemy);
            cardsList.AddCharacter(enemy);
            sprites.AddSprite(enemy);
        }

        public override async Task ApplyBattleEffect(int index, BattleEffect battleEffect)
        {
            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

            Characters.ApplyBattleEffect(index, battleEffect);

            cardsList[index].ShowEffectIndicator(battleEffect.Action, battleEffect.Effect, battleEffect.IsNegative);
            cardsList[index].UpdateTurnsLabel(battleEffect.Turns);
        }

        public override void UpdateBattleEffect(int index)
        {
            BattleEffect battleEffect = Characters.BattleStates[index].BattleEffect;
            cardsList[index].UpdateTurnsLabel(battleEffect.Turns);
        }

        public override void RemoveBattleEffect(int index)
        {
            Characters.RemoveBattleEffect(index);
            cardsList[index].HideEffectIndicator();
        }

        public override async Task ChangeHealth(int index, int change)
        {
            sprites.ShowHealthBar(index, change, Characters[index]);
            Characters.ChangeHealth(index, change);          
            await cardsList.TweenDamage(index, change);
        }

        public override async Task ChangeMana(int index, int change)
        {
            Characters.ChangeMana(index, change);
            await cardsList.TweenManaChange(index, change);

            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

        public override async Task DefendCharacter(int index)
        {
            BattleOptions.ShowInfoLabel($"{Characters[index].Name} defends!");
            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

        public override void FocusOnFirst()
        {
            sprites.FocusOnFirst();
        }

        public override async Task SelectAction(int index)
        {
            Character character = Characters[index];

            int targetIndex = Allies.Characters.GetRandomCharacter();
            Character ally = Allies.Characters[targetIndex];

            CharacterAction action = character.ChooseBehaviour();

            switch (action)
            {
                case CharacterAction.Attack:
                    await commands.OnAttack(index, character, targetIndex, ally);
                    break;
                case CharacterAction.Defend:
                    break;
                case CharacterAction.Items:
                    break;
                case CharacterAction.Magic:
                    await commands.OnMagic(index, character, targetIndex, ally);
                    break;
            }
        }

        public override void OnNextCharacterPassed()
        {
            throw new NotImplementedException();
        }

        public override void StartTurn()
        {
        }

        public override void Clear()
        {
            Characters.Clear();
            cardsList.Clear();
            sprites.Clear();
        }
    }
}
