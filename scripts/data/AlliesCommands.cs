using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Autoload;
using TheWizardCoder.Subdisplays;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class AlliesCommands
    {
        private Global global;
        private AlliesSideDisplay alliesSideDisplay;
        private BattleOptions battleOptions;

        public AlliesCommands(Global global, AlliesSideDisplay alliesSideDisplay)
        {
            this.global = global;
            this.alliesSideDisplay = alliesSideDisplay;
            battleOptions = alliesSideDisplay.BattleOptions;
        }

        public async Task OnAttack(Character ally)
        {
            //alliesSideDisplay.BattleOptions.ShowInfoLabel($"{ally.Name} attacks {Enemies.Characters[state.Target].Name}!");

            alliesSideDisplay.AddExperience(ally, ally.Level);
        }

        public async Task OnDefend()
        {

        }

        public async Task OnItem(int index)
        {
            CharacterBattleState healerState = alliesSideDisplay.Characters.GetCharacterState(index);
            int healerIndex = index;

            string itemName = global.PlayerData.Inventory[healerState.ActionModifier];
            Item item = global.ItemDescriptions[itemName];

            Character target = alliesSideDisplay.Characters.BattleStates[healerState.Target].Character;

            if (healerState.Target == healerIndex)
            {
                battleOptions.ShowInfoLabel($"{healerState.Character.Name} gave {itemName} to themselves!");
            }
            else
            {
                battleOptions.ShowInfoLabel($"{healerState.Character.Name} gave {itemName} to {target.Name}!");
            }

            switch (item.Type)
            {
                case ItemType.Heal:
                    await alliesSideDisplay.DisplayHealthChange(healerState.Target, item.Effect);
                    break;
                case ItemType.Mana:
                    await alliesSideDisplay.DisplayManaChange(healerState.Target, item.Effect);
                    break;
                case ItemType.Magic:
                    
                    break;
            }
        }
    }
}
