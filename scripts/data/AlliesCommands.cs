using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Autoload;
using TheWizardCoder.Subdisplays;
using TheWizardCoder.Enums;
using Godot;

namespace TheWizardCoder.Data
{
    public class AlliesCommands
    {
        private Global global;
        private AlliesSideDisplay allies;
        private BattleOptions battleOptions;

        public AlliesCommands(Global global, AlliesSideDisplay alliesSideDisplay)
        {
            this.global = global;
            this.allies = alliesSideDisplay;
            battleOptions = alliesSideDisplay.BattleOptions;
        }

        public async Task OnAttack(int index, Character ally)
        {
            CharacterBattleState state = allies.Characters.GetCharacterState(index);

            allies.BattleOptions.ShowInfoLabel($"{ally.Name} attacks {allies.Enemies.Characters[state.Target].Name}!");
            await allies.Enemies.ChangeHealth(state.Target, -ally.AttackPoints);
            allies.AddExperience(ally, ally.Level);
        }

        public async Task OnDefend(int index, Character ally)
        {
            await allies.DefendCharacter(index);
        }

        public async Task OnItem(int index, Character ally)
        {
            CharacterBattleState healerState = allies.Characters.GetCharacterState(index);
            int healerIndex = index;

            string itemName = global.PlayerData.Inventory[healerState.ActionModifier];
            Item item = global.ItemDescriptions[itemName];

            GD.Print(healerState.Target);
            Character target = allies.Characters.BattleStates[healerState.Target].Character;

            if (healerState.Target == healerIndex)
            {
                battleOptions.ShowInfoLabel($"{ally.Name} gave {itemName} to themselves!");
            }
            else
            {
                battleOptions.ShowInfoLabel($"{ally.Name} gave {itemName} to {target.Name}!");
            }

            switch (item.Type)
            {
                case ItemType.Heal:
                    await allies.ChangeHealth(healerState.Target, item.Effect);
                    break;
                case ItemType.Mana:
                    await allies.ChangeMana(healerState.Target, item.Effect);
                    break;
                case ItemType.Magic:
                    GD.Print(item.AdditionalData);
                    BattleEffect effect = BattleEffect.FromArray(item.AdditionalData);
                    GD.Print(effect.Effect);
                    await allies.ApplyBattleEffect(index, effect);
                    break;
            }

            global.RemoveFromInventory(itemName);
        }

        public async Task OnMagic(int index, Character ally)
        {
            CharacterBattleState state = allies.Characters.GetCharacterState(ally);

            string currentMagicSpellName = ally.MagicSpells[state.ActionModifier];
            MagicSpell currentMagicSpell = global.MagicSpells[currentMagicSpellName];

            await allies.ChangeMana(index, -currentMagicSpell.Cost);

            if (currentMagicSpell.TargetType == CharacterType.Enemy)
            {
                string enemyName = allies.Enemies.Characters[state.Target].Name;
                allies.BattleOptions.ShowInfoLabel($"{ally.Name} casted {currentMagicSpellName} on {enemyName}!");

                await allies.Enemies.ChangeHealth(state.Target, -currentMagicSpell.Effect);
            }
            else if (currentMagicSpell.TargetType == CharacterType.Ally)
            {
                string healedAllyName = allies.Characters[state.Target].Name;

                if (ally.Name == healedAllyName)
                {
                    allies.BattleOptions.ShowInfoLabel($"{ally.Name} casted {currentMagicSpellName} on themselves!");
                }
                else
                {
                    allies.BattleOptions.ShowInfoLabel($"{ally.Name} casted {currentMagicSpellName} on {healedAllyName}!");
                }

                await allies.ChangeHealth(state.Target, currentMagicSpell.Effect);
            }

            allies.AddExperience(index, ally.Level + 2);
        }
    }
}
