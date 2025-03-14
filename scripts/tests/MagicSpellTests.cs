using GdUnit4;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class MagicSpellTests
    {
        [TestCase("Fireball", "Desc.", 15, 5, 5, CharacterType.Enemy, MagicSpellType.Attack)]
        [TestCase("Fireball2", "Desc.", 15, 5, 5, CharacterType.Ally, MagicSpellType.Heal)]
        [TestCase("Hibble", "Desc.", 15, 10, 5, CharacterType.Ally, MagicSpellType.ApplyBattleEffect)]
        [TestCase("Fireball", "Omens Optant Mundum Regere", 15, 5, 5, CharacterType.Enemy, MagicSpellType.Attack)]
        public void ValidConstructorCalls(string name, string description, int effect, int cost, int shopPrice, CharacterType targetType, MagicSpellType spellType)
        {
            MagicSpell magicSpell = new(name, description, effect, cost, shopPrice, targetType, spellType);

            AssertBool(magicSpell.Name == name).IsTrue();
            AssertBool(magicSpell.Description == description).IsTrue();
            AssertBool(magicSpell.Effect == effect).IsTrue();
            AssertBool(magicSpell.Cost == cost).IsTrue();
            AssertBool(magicSpell.ShopPrice == shopPrice).IsTrue();
            AssertBool(magicSpell.TargetType == targetType).IsTrue();
            AssertBool(magicSpell.SpellType == spellType).IsTrue();
        }

        [TestCase("Fireball", "Desc.", -15, 5, 5, CharacterType.Enemy, MagicSpellType.Attack)]
        [TestCase("Fireball", "Desc.", 15, -50, 5, CharacterType.Enemy, MagicSpellType.Attack)]
        [TestCase("Fireball", "Desc.", -5, 5, 5, CharacterType.Enemy, MagicSpellType.Attack)]
        [TestCase("Fireball", "Desc.", 10, 5, -5, CharacterType.Enemy, MagicSpellType.Attack)]
        public void InvalidConstructorCalls(string name, string description, int effect, int cost, int shopPrice, CharacterType targetType, MagicSpellType spellType)
        {
            MagicSpell magicSpell;

            AssertThrown(() =>
            {
                magicSpell = new(name, description, effect, cost, shopPrice, targetType, spellType);
            });
        }
    }
}
