using System;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class BattleEffect
    {
        public BattleEffectType Action { get; set; }
        public CharacterType TargetType { get; set; }
        public int Turns { get; set; }
        public int Effect { get; set; }
        public bool IsNegative { get; set; } = false;

        public BattleEffect(BattleEffectType action, CharacterType targetType, int turns, int effect, bool isNegative)
        {
            Action = action;
            TargetType = targetType;
            Turns = turns;
            Effect = effect;
            IsNegative = isNegative;

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
