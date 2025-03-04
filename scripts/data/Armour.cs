using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class Armour
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Effect { get; private set; }
        public BattleEffectType EffectType { get; private set; }

        public Armour(string name, string description, int effect, BattleEffectType effectType)
        {
            Name = name;
            Description = description;
            Effect = effect;
            EffectType = effectType;
        }
    }
}
