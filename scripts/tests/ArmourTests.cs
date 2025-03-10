using GdUnit4;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class ArmourTests
    {
        [TestCase("Iron Armour", "A basic iron armour.", ArmourType.PrimaryArmour, 5, BattleEffectType.Defense)]
        [TestCase("Steel Armour", "A strong steel armour.", ArmourType.PrimaryArmour, 10, BattleEffectType.Defense)]
        public void ValidConstructorCalls(string name, string description, ArmourType type, int effect, BattleEffectType effectType)
        {
            Armour armour = new(name, description, type, effect, effectType);
            AssertBool(armour.Name == name).IsTrue();
            AssertBool(armour.Description == description).IsTrue();
            AssertBool(armour.Type == type).IsTrue();
            AssertBool(armour.Effect == effect).IsTrue();
            AssertBool(armour.EffectType == effectType).IsTrue();
        }

        [TestCase("Iron Armour", "A basic iron armour.", ArmourType.PrimaryArmour, -5, BattleEffectType.Defense)]
        [TestCase("Steel Armour", "A strong steel armour.", ArmourType.PrimaryArmour, 10, BattleEffectType.Attack)]
        public void InvalidConstructorCalls(string name, string description, ArmourType type, int effect, BattleEffectType effectType)
        {
            Armour armour;
            AssertThrown(() =>
            {
                armour = new(name, description, type, effect, effectType);
            });
        }
    }
}
