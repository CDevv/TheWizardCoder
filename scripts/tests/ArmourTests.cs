using GdUnit4;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class ArmourTests
    {
        [TestCase("Iron Armour", "A basic iron armour.", ArmourType.PrimaryArmour, 5, BattleEffectType.Defense, 10)]
        [TestCase("Steel Armour", "A strong steel armour.", ArmourType.PrimaryArmour, 10, BattleEffectType.Defense, 10)]
        public void ValidConstructorCalls(string name, string description, ArmourType type, int effect, BattleEffectType effectType, int price)
        {
            Armour armour = new(name, description, type, effect, effectType, price);
            AssertBool(armour.Name == name).IsTrue();
            AssertBool(armour.Description == description).IsTrue();
            AssertBool(armour.Type == type).IsTrue();
            AssertBool(armour.Effect == effect).IsTrue();
            AssertBool(armour.EffectType == effectType).IsTrue();
            AssertBool(armour.Price == price).IsTrue();
        }

        [TestCase("Iron Armour", "A basic iron armour.", ArmourType.PrimaryArmour, -5, BattleEffectType.Defense, 5)]
        [TestCase("Steel Armour", "A strong steel armour.", ArmourType.PrimaryArmour, 10, BattleEffectType.Attack, -10)]
        public void InvalidConstructorCalls(string name, string description, ArmourType type, int effect, BattleEffectType effectType, int price)
        {
            Armour armour;
            AssertThrown(() =>
            {
                armour = new(name, description, type, effect, effectType, price);
            });
        }
    }
}
