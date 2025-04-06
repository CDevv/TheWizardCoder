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
    public class EnemiesCommands
    {
        private Global global;
        private EnemiesSideDisplay enemies;
        private AlliesSideDisplay allies;
        private BattleOptions battleOptions;

        public EnemiesCommands(Global global, EnemiesSideDisplay enemiesSideDisplay)
        {
            this.global = global;
            this.enemies = enemiesSideDisplay;
            allies = enemiesSideDisplay.Allies;
            battleOptions = enemiesSideDisplay.BattleOptions;
        }

        public async Task OnAttack(int index, Character enemy, int targetIndex, Character target)
        {
            battleOptions.ShowInfoLabel($"{enemy.Name} attacks {target.Name}!");
            await allies.ChangeHealth(targetIndex, -enemy.AttackPoints);
        }

        public async Task OnMagic(int index, Character enemy, int targetIndex, Character target)
        {
            string magicSpellName = enemy.GetRandomMagicSpell();
            MagicSpell magicSpell = global.MagicSpells[magicSpellName];

            battleOptions.ShowInfoLabel($"{enemy.Name} casts {magicSpellName} on {target.Name}!");
            if (magicSpell.TargetType == CharacterType.Enemy)
            {
                switch (magicSpell.SpellType)
                {
                    case MagicSpellType.Attack:
                        await allies.ChangeHealth(targetIndex, -magicSpell.Effect);

                        break;
                    case MagicSpellType.Heal:
                        break;
                    case MagicSpellType.ApplyBattleEffect:
                        if (allies.Characters.BattleStates[targetIndex].HasBattleEffect)
                        {
                            await allies.ChangeHealth(targetIndex, -magicSpell.Effect);
                        }
                        else
                        {
                            await allies.ApplyBattleEffect(targetIndex, magicSpell.BattleEffect.Clone());
                        }

                        break;
                }
            }
        }
    }
}
