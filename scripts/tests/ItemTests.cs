using GdUnit4;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class ItemTests
    {
        [TestCase("Test Item", "Test Description", 10, ItemType.Heal)]
        [TestCase("Boom Juice", "Test Description", 25, ItemType.Heal)]
        [TestCase("Magic Wand", "Test Description", 50, ItemType.Magic)]
        [TestCase("A Key", "Test Description", 0, ItemType.Key)]
        public void ValidConstructorCalls(string name, string description, int effect, ItemType itemType)
        {
            Item item = new(name, description, effect, itemType);

            AssertBool(item.Name == name).IsTrue();
            AssertBool(item.Description == description).IsTrue();
            AssertBool(item.Effect == effect).IsTrue();
            AssertBool(item.Type == itemType).IsTrue();
        }

        [TestCase("Test Item", "Test Description", -10, ItemType.Heal)]
        [TestCase("Magi-hibble", "Test Description", -25, ItemType.Magic)]
        [TestCase("Boom Juice", "Test Description", -50, ItemType.Heal)]
        [TestCase("Kris", "Test Description", -100, ItemType.Heal)]
        public void InvalidConstructorCalls(string name, string description, int effect, ItemType itemType)
        {
            Item item;
            AssertThrown(() =>
            {
                item = new(name, description, effect, itemType);
            });
        }

        [TestCase(100)]
        [TestCase(20)]
        [TestCase(15)]
        [TestCase(10)]
        public void MakeSellableSuccess(int price)
        {
            Item item = new("Test Item", "Test Description", 10, ItemType.Key);
            item.MakeSellable(price);

            AssertBool(item.Sellable).IsTrue();
            AssertBool(item.Price == price).IsTrue();
        }

        [TestCase(-100)]
        [TestCase(-20)]
        [TestCase(-15)]
        [TestCase(-10)]
        public void MakeSellableThrows(int price)
        {
            Item item = new("Test Item", "Test Description", 10, ItemType.Key);
            AssertThrown(() =>
            {
                item.MakeSellable(price);
            });
        }

        [TestCase("Display", "CodeMessage", "GoUp();")]
        [TestCase("PlayerMethod", "EquipItem", "Fishing Rod")]
        [TestCase("Display", "CodeMessage", "GoRight();")]
        [TestCase("Defense", "Ally", "3")]
        public void AddAdditionalDataSuccess(string item1, string item2, string item3)
        {
            Item item = new("Test Item", "Test Description", 10, ItemType.Key);

            string[] additionalData = { item1, item2, item3 };
            item.AdditionalData = additionalData;

            AssertBool(item.AdditionalData.Length == 3).IsTrue();
            AssertArray(item.AdditionalData).ContainsExactly(additionalData);
        }
    }
}
