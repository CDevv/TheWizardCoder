using GdUnit4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;

using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class BattleEffectTests
    {
        [TestCase("Defense", "Ally", "3", "5")]
        [TestCase("Attack", "Ally", "3", "5")]
        [TestCase("Defense", "Enemy", "3", "5")]
        [TestCase("Attack", "Enemy", "3", "5")]
        public void ValidConstructorCalls(string action, string targetType, string turns, string effect)
        {
            string[] arr = new string[] { action, targetType, turns, effect };
            BattleEffect battleEffect = new(arr);

            AssertBool(battleEffect.Action == Enum.Parse<BattleEffectType>(action)).IsTrue();
            AssertBool(battleEffect.TargetType == Enum.Parse<CharacterType>(targetType)).IsTrue();
            AssertBool(battleEffect.Turns == int.Parse(turns)).IsTrue();
            AssertBool(battleEffect.Effect == int.Parse(effect)).IsTrue();
        }

        [TestCase("Banichka", "Ally", "3", "5")]
        [TestCase("Attack", "Ally", "-1", "5")]
        [TestCase("Defense", "Enemy", "3", "-5")]
        [TestCase("Trufel", "Enemy", "3", "5")]
        public void InvalidConstructorCalls(string action, string targetType, string turns, string effect)
        {
            string[] arr = new string[] { action, targetType, turns, effect };
            BattleEffect battleEffect;

            AssertThrown(() =>
            {
                battleEffect = new(arr);
            });
        }
    }
}
