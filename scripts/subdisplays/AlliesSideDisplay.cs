﻿using Godot;
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
        private AlliesCommands commands;
        private AlliesInputObserver observer;

        [Export]
        public DamageIndicator DamageIndicator { get; set; }
        [Export]
        public EnemiesSideDisplay Enemies { get; set; }

        public CharacterCardsList CardsList => cardsList;

        public override void _Ready()
        {
            base._Ready();
            commands = new AlliesCommands(global, this);
            observer = new AlliesInputObserver(global, this);

            cardsList = GetNode<CharacterCardsList>("CardsList");
            cardsList.DamageIndicator = DamageIndicator;

            BattleOptions.FightButtonTriggered += () => observer.OnAttackButton();
            BattleOptions.DefenseButtonTriggered += () => observer.OnDefendButton();
            BattleOptions.ItemsButtonTriggered += (int itemIndex, string itemName) => observer.OnItemButton(itemIndex, itemName);
            BattleOptions.MagicButtonTriggered += (int index) => observer.OnMagicButton(index);

            Enemies.EnemyPressed += (int index) => observer.OnEnemyPressed(index);
        }

        public override void AddCharacter(Character character)
        {
            Characters.AddCharacter(character);
            cardsList.AddCharacter(character);
        }

        public override async Task ApplyBattleEffect(int index, BattleEffect battleEffect)
        {
            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

            Characters.ApplyBattleEffect(index, battleEffect);

            cardsList[index].ShowEffectIndicator(battleEffect.Action, battleEffect.Effect, battleEffect.IsNegative);
            cardsList[index].UpdateTurnsLabel(battleEffect.Turns);
        }

        public override async Task ChangeHealth(int index, int change)
        {
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
            cardsList.FocusOnFirst();
        }

        public override async Task SelectAction(int index)
        {
            Character character = Characters[index];
            CharacterBattleState state = Characters.GetCharacterState(character);

            switch (state.Action)
            {
                case Enums.CharacterAction.Attack:
                    await commands.OnAttack(index, character);
                    break;
                case Enums.CharacterAction.Defend:
                    await commands.OnDefend(index, character);
                    break;
                case Enums.CharacterAction.Items:
                    await commands.OnItem(index, character);
                    break;
                case Enums.CharacterAction.Magic:
                    await commands.OnMagic(index, character);
                    break;
            }
        }

        public override void OnNextCharacterPassed()
        {
            cardsList[CurrentCharacter - 1].HideBackground();
        }

        public override void StartTurn()
        {
            cardsList[CurrentCharacter].ShowAsCurrentCharacter();
            BattleOptions.UpdateDisplay(Characters[CurrentCharacter]);
            BattleOptions.ShowOptions();
        }
    }
}
