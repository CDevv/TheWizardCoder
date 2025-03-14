using GdUnit4;
using Godot;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    class CharacterPathwayTests
    {
        [TestCase(Direction.Down, 20, 20, 1)]
        [TestCase(Direction.Up, 20, 120, 2)]
        [TestCase(Direction.Left, 210, 20, 1)]
        [TestCase(Direction.Right, 220, 20, 2)]
        public void ValidConstructorCalls(Direction direction, int x, int y, int speed)
        {
            CharacterPathway characterPathway = new(direction, new Vector2(x, y), speed);

            AssertBool(characterPathway.Direction == direction).IsTrue();
            AssertBool(characterPathway.Position.X == x).IsTrue();
            AssertBool(characterPathway.Position.Y == y).IsTrue();
            AssertBool(characterPathway.Speed == speed).IsTrue();
        }

        [TestCase(Direction.Down, 20, 20, -1)]
        [TestCase(Direction.Left, 40, 20, -2)]
        [TestCase(Direction.Up, 20, 210, -2)]
        [TestCase(Direction.Right, 20, 20, -1)]
        public void InvalidConstructorCalls(Direction direction, int x, int y, int speed)
        {
            CharacterPathway characterPathway;

            AssertThrown(() =>
            {
                characterPathway = new CharacterPathway(direction, new Vector2(x, y), speed);
            });
        }
    }
}
