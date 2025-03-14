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
        [TestCase("TestShop", ShopType.Item, "Item", "Apple")]
        [TestCase("TestMagicShop", ShopType.Magic, "Fireball", "Fireball2")]
        [TestCase("TestMagicShop", ShopType.Magic, "Magi-hibble", "Fireball")]
        [TestCase("TestShop", ShopType.Item, "Apple Juice", "Apple")]
        [TestCase("TestShop", ShopType.Item, "Cyanberry", "Purple Fruit")]
        public void ValidConstructorCalls(string name, ShopType shopType, string item1, string item2)
        {
            string[] array = new[] { item1, item2 };
            Array<string> items = new(array);

            Shop shop = new(name, shopType, items);

            AssertBool(shop.Name == name).IsTrue();
            AssertBool(shop.Type == shopType).IsTrue();
            AssertArray(shop.Items).ContainsExactly(items);
        }
    }
}
