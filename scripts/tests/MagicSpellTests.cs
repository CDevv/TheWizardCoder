using GdUnit4;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using static GdUnit4.Assertions;
using Moq;
using Godot;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class MagicSpellTests
    {
        [TestCase("Fireball", "Desc.", 15, 5, 5, CharacterType.Enemy)]
        [TestCase("Fireball2", "Desc.", 15, 5, 5, CharacterType.Ally)]
        [TestCase("Hibble", "Desc.", 15, 10, 5, CharacterType.Ally)]
        [TestCase("Fireball", "Omens Optant Mundum Regere", 15, 5, 5, CharacterType.Enemy)]
        public void ValidConstructorCalls(string name, string description, int effect, int cost, int shopPrice, CharacterType targetType)
        {
            MagicSpell magicSpell = new(name, description, effect, cost, shopPrice, targetType);

            AssertBool(magicSpell.Name == name).IsTrue();
            AssertBool(magicSpell.Description == description).IsTrue();
            AssertBool(magicSpell.Effect == effect).IsTrue();
            AssertBool(magicSpell.Cost == cost).IsTrue();
            AssertBool(magicSpell.ShopPrice == shopPrice).IsTrue();
            AssertBool(magicSpell.TargetType == targetType).IsTrue();
        }

        [TestCase("Fireball", "Desc.", -15, 5, 5, CharacterType.Enemy)]
        [TestCase("Fireball", "Desc.", 15, -50, 5, CharacterType.Enemy)]
        [TestCase("Fireball", "Desc.", -5, 5, 5, CharacterType.Enemy)]
        [TestCase("Fireball", "Desc.", 10, 5, -5, CharacterType.Enemy)]
        public void InvalidConstructorCalls(string name, string description, int effect, int cost, int shopPrice, CharacterType targetType)
        {
            MagicSpell magicSpell;

            AssertThrown(() =>
            {
                magicSpell = new(name, description, effect, cost, shopPrice, targetType);
            });
        }
    }
}
