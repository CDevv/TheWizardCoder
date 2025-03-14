using GdUnit4;
using TheWizardCoder.Data;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class CharacterTests
    {
        [TestCase(10, 100, 10, 20)]
        [TestCase(10, 100, 20, 30)]
        [TestCase(50, 100, 50, 100)]
        [TestCase(70, 100, 40, 100)]
        public void AddHealthExpected(int health, int maxHealth, int addedHealth, int expected)
        {
            Character character = new() { Health = health, MaxHealth = maxHealth };
            character.AddHealth(addedHealth);
            AssertBool(character.Health == expected).IsTrue();
        }

        [TestCase(10, 100, 10, 0)]
        [TestCase(50, 50, 10, 40)]
        [TestCase(100, 100, 50, 50)]
        [TestCase(10, 100, 25, 0)]
        public void RemoveHealthExpected(int health, int maxHealth, int removedHealth, int expected)
        {
            Character character = new() { Health = health, MaxHealth = maxHealth };
            character.RemoveHealth(removedHealth);
            AssertBool(character.Health == expected).IsTrue();
        }

        [TestCase(10, 100, 10, 20)]
        [TestCase(10, 20, 10, 20)]
        [TestCase(50, 100, 70, 100)]
        [TestCase(25, 50, 40, 50)]
        public void AddManaExpected(int mana, int maxMana, int addedMana, int expected)
        {
            Character character = new() { Points = mana, MaxPoints = maxMana };
            character.AddMana(addedMana);
            AssertBool(character.Points == expected).IsTrue();
        }

        [TestCase(10, 100, 10, 0)]
        [TestCase(20, 20, 10, 10)]
        [TestCase(5, 20, 10, 0)]
        [TestCase(5, 50, 10, 0)]
        public void RemoveManaExpected(int mana, int maxMana, int removedMana, int expected)
        {
            Character character = new() { Points = mana, MaxPoints = maxMana };
            character.RemoveMana(removedMana);
            AssertBool(character.Points == expected).IsTrue();
        }

        [TestCase(0, 5, 1, 5)]
        [TestCase(5, 5, 2, 0)]
        [TestCase(4, 8, 2, 2)]
        [TestCase(2, 5, 1, 7)]
        public void AddLevelPointsExpected(int levelPoints, int addedLevelPoints, int expectedLevel, int expectedLevelPoints)
        {
            Character character = new() { LevelPoints = levelPoints, Level = 1 };
            character.AddLevelPoints(addedLevelPoints);
            AssertBool(character.Level == expectedLevel).IsTrue();
            AssertBool(character.LevelPoints == expectedLevelPoints).IsTrue();
        }

        [TestCase(1, 10)]
        [TestCase(2, 20)]
        [TestCase(5, 50)]
        [TestCase(10, 100)]
        public void GetMaxLevelPointsExpected(int level, int expected)
        {
            Character character = new() { Level = level };
            int result = character.GetMaxLevelPoints();
            AssertBool(result == expected).IsTrue();
        }

        [TestCase(10)]
        [TestCase(55)]
        [TestCase(100)]
        [TestCase(75)]
        public void SetMaxHealth(int value)
        {
            Character character = new();
            character.SetMaxHealth(value);
            AssertBool(character.MaxHealth == value).IsTrue();
            AssertBool(character.Health == value).IsTrue();
        }

        [TestCase(25)]
        [TestCase(50)]
        [TestCase(100)]
        [TestCase(75)]
        public void SetMaxPoints(int value)
        {
            Character character = new();
            character.SetMaxPoints(value);
            AssertBool(character.MaxPoints == value).IsTrue();
            AssertBool(character.Points == value).IsTrue();
        }
    }
}
