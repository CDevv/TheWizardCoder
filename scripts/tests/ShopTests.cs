using GdUnit4;
using Godot.Collections;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class ShopTests
    {
        [TestCase("TestShop", "Item", "Apple")]
        [TestCase("TestMagicShop", "Fireball", "Fireball2")]
        [TestCase("TestMagicShop", "Magi-hibble", "Fireball")]
        [TestCase("TestShop", "Apple Juice", "Apple")]
        [TestCase("TestShop", "Cyanberry", "Purple Fruit")]
        public void ValidConstructorCalls(string name, string item1, string item2)
        {
            string[] array = new[] { item1, item2 };
            Array<string> items = new(array);

            Shop shop = new(name, items);

            AssertBool(shop.Name == name).IsTrue();
            AssertArray(shop.Items).ContainsExactly(items);
        }
    }
}
