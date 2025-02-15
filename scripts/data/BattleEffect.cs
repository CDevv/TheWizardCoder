using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class BattleEffect
    {
        public BattleEffectType Action { get; set; }
        public CharacterType TargetType { get; set; }
        public int Turns { get; set; }
        public int Effect { get; set; }

        public BattleEffect(BattleEffectType action, CharacterType targetType, int turns, int effect)
        {
            Action = action;
            TargetType = targetType;
            Turns = turns;
            Effect = effect;

            if (Turns < 0)
            {
                throw new ArgumentException("Turns cannot be negative.");
            }

            if (Effect < 0)
            {
                throw new ArgumentException("Effect cannot be negative.");
            }
        }
    }
}
