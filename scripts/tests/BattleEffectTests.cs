using GdUnit4;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class BattleEffectTests
    {
        [TestCase(BattleEffectType.Defense, CharacterType.Ally,3, 5)]
        [TestCase(BattleEffectType.Attack, CharacterType.Ally, 3, 5)]
        [TestCase(BattleEffectType.Defense, CharacterType.Enemy, 3, 5)]
        [TestCase(BattleEffectType.Attack,  CharacterType.Enemy, 3, 5)]
        public void ValidConstructorCalls(BattleEffectType action, CharacterType targetType, int turns, int effect)
        {
            BattleEffect battleEffect = new(action, targetType, turns, effect);

            AssertBool(battleEffect.Action == action).IsTrue();
            AssertBool(battleEffect.TargetType == targetType).IsTrue();
            AssertBool(battleEffect.Turns == turns).IsTrue();
            AssertBool(battleEffect.Effect == effect).IsTrue();
        }

        [TestCase(BattleEffectType.Attack, CharacterType.Ally, 3, -5)]
        [TestCase(BattleEffectType.Defense, CharacterType.Ally, -1, 5)]
        public void InvalidConstructorCalls(BattleEffectType action, CharacterType targetType, int turns, int effect)
        {
            BattleEffect battleEffect;

            AssertThrown(() =>
            {
                battleEffect = new(action, targetType, turns, effect);
            });
        }
    }
}
