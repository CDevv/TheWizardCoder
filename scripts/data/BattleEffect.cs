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

        public static BattleEffect FromArray(string[] arr)
        {
            BattleEffectType action = Enum.Parse<BattleEffectType>(arr[0]);
            CharacterType targetType = Enum.Parse<CharacterType>(arr[1]);
            int turns = int.Parse(arr[2]);
            int effect = int.Parse(arr[3]);
            bool isNegative = bool.Parse(arr[4]);

            return new BattleEffect(action, targetType, turns, effect, isNegative);
        }

        public BattleEffect Clone()
        {
            return new BattleEffect(Action, TargetType, Turns, Effect, IsNegative);
        }
    }
}
