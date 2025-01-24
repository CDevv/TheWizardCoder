using System;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class CharacterBattleState
    {
        public int Target { get; set; }
        public CharacterAction Action { get; set; }
        public int ActionModifier { get; set; }
        public CharacterData Character { get; set; }
        public int InternalIndex { get; private set; }
        public bool HasBattleEffect { get; set; }
        public BattleEffect BattleEffect { get; set; }

        public CharacterBattleState(CharacterData character, int internalIndex)
        {
            Target = 0;
            Action = CharacterAction.Attack;
            ActionModifier = 0;

            Character = character;
            InternalIndex = internalIndex;

            HasBattleEffect = false;
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