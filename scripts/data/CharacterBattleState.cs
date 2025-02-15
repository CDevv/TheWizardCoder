using System;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class CharacterBattleState
    {
        public int Target { get; set; }
        public CharacterAction Action { get; set; }
        public int ActionModifier { get; set; }
        public Character Character { get; set; }
        public int InternalIndex { get; private set; }
        public bool HasBattleEffect { get; set; }
        public BattleEffect BattleEffect { get; set; }

        public CharacterBattleState(Character character, int internalIndex)
        {
            Target = 0;
            Action = CharacterAction.Attack;
            ActionModifier = 0;

            Character = character;
            InternalIndex = internalIndex;

            HasBattleEffect = false;

            if (internalIndex < 0)
            {
                throw new ArgumentException("InternalIndex cannot be negative.");
            }
        }

        public void Reset()
        {
            Target = 0;
            Action = CharacterAction.Attack;
            ActionModifier = 0;

            HasBattleEffect = false;
        }
    }
}