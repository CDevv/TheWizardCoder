using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Subdisplays;
using TheWizardCoder.Enums;
using TheWizardCoder.Autoload;

namespace TheWizardCoder.Data
{
    public class AlliesInputObserver
    {
        private Global global;
        private AlliesSideDisplay allies;
        private EnemiesSideDisplay enemies;

        public AlliesInputObserver(Global global, AlliesSideDisplay allies)
        {
            this.global = global;
            this.allies = allies;
            enemies = allies.Enemies;
        }

        public void OnAttackButton()
        {
            allies.Characters.BattleStates[allies.CurrentCharacter].Action = CharacterAction.Attack;
            enemies.FocusOnFirst();
            allies.BattleOptions.ShowInfoLabel("Select an enemy!");
        }

        public void OnDefendButton()
        {
            allies.Characters.BattleStates[allies.CurrentCharacter].Action = CharacterAction.Defend;
            allies.PassToNext();
        }

        public void OnItemButton(int index, string itemName)
        {
            Item item = global.ItemDescriptions[itemName];

            if (item.Type != ItemType.Key)
            {
                allies.Characters.BattleStates[allies.CurrentCharacter].Action = CharacterAction.Items;
                allies.Characters.BattleStates[allies.CurrentCharacter].ActionModifier = index;
                allies.FocusOnFirst();
                allies.BattleOptions.ShowInfoLabel("Select an ally!");
            }
        }

        public void OnMagicButton(int index)
        {
            Character ally = allies.Characters.BattleStates[allies.CurrentCharacter].Character;

            allies.Characters.BattleStates[allies.CurrentCharacter].Action = CharacterAction.Magic;
            allies.Characters.BattleStates[allies.CurrentCharacter].ActionModifier = index;

            string magicSpellName = ally.MagicSpells[index];
            MagicSpell magicSpell = global.MagicSpells[magicSpellName];
            if (magicSpell.TargetType == CharacterType.Enemy)
            {
                enemies.FocusOnFirst();
                allies.BattleOptions.ShowInfoLabel("Select an enemy to cast on!");
            }
            else
            {
                allies.FocusOnFirst();
                allies.BattleOptions.ShowInfoLabel("Select an ally to cast on!");
            }
        }

        public void OnEnemyPressed(int index)
        {
            allies.Characters.BattleStates[allies.CurrentCharacter].Target = index;
            allies.PassToNext();
        }

        public void OnCharacterCardPressed(int index)
        {
            allies.Characters.BattleStates[allies.CurrentCharacter].Target = index;
            allies.PassToNext();
        }
    }
}
