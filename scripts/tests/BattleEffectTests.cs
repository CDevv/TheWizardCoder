using GdUnit4;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class BattleEffectTests
    {
        [TestCase(BattleEffectType.Defense, CharacterType.Ally,3, 5, false)]
        [TestCase(BattleEffectType.Attack, CharacterType.Ally, 3, 5, true)]
        [TestCase(BattleEffectType.Defense, CharacterType.Enemy, 3, 5, false)]
        [TestCase(BattleEffectType.Attack,  CharacterType.Enemy, 3, 5, false)]
        public void ValidConstructorCalls(BattleEffectType action, CharacterType targetType, int turns, int effect, bool isNegative)
        {
            BattleEffect battleEffect = new(action, targetType, turns, effect, isNegative);

            AssertBool(battleEffect.Action == action).IsTrue();
            AssertBool(battleEffect.TargetType == targetType).IsTrue();
            AssertBool(battleEffect.Turns == turns).IsTrue();
            AssertBool(battleEffect.Effect == effect).IsTrue();
            AssertBool(battleEffect.IsNegative == isNegative).IsTrue();
        }

        [TestCase(BattleEffectType.Attack, CharacterType.Ally, 3, -5, false)]
        [TestCase(BattleEffectType.Defense, CharacterType.Ally, -1, 5, false)]
        public void InvalidConstructorCalls(BattleEffectType action, CharacterType targetType, int turns, int effect, bool isNegative)
        {
            BattleEffect battleEffect;

            AssertThrown(() =>
            {
                battleEffect = new(action, targetType, turns, effect, isNegative);
            });
        }
    }
}
