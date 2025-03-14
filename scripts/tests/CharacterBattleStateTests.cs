using GdUnit4;
using Moq;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class CharacterBattleStateTests
    {
        [TestCase("Test Character", 0)]
        [TestCase("Nolan", 2)]
        [TestCase("Dude", 3)]
        [TestCase("Test Character", 4)]
        public void ValidConstructorCalls(string characterName, int internalIndex)
        {
            Mock<Character> character = new();
            character.SetupGet(c => c.Name).Returns(characterName);

            CharacterBattleState characterBattleState = new(character.Object, internalIndex);

            AssertBool(characterBattleState.Target == 0).IsTrue();
            AssertBool(characterBattleState.Action == CharacterAction.Attack).IsTrue();
            AssertBool(characterBattleState.ActionModifier == 0).IsTrue();
            AssertBool(characterBattleState.Character.Name == characterName).IsTrue();
            AssertBool(characterBattleState.InternalIndex == internalIndex).IsTrue();
            AssertBool(characterBattleState.HasBattleEffect == false).IsTrue();
        }

        [TestCase("Test Character", -1)]
        [TestCase("Test Character", -5)]
        [TestCase("Test Character", -9)]
        [TestCase("Test Character", -2)]
        public void InvalidConstructorCalls(string characterName, int internalIndex)
        {
            Mock<Character> character = new();
            character.SetupGet(c => c.Name).Returns(characterName);

            CharacterBattleState characterBattleState;
            AssertThrown(() =>
            {
                characterBattleState = new(character.Object, internalIndex);
            });
        }

        [TestCase(CharacterAction.Defend, 2)]
        [TestCase(CharacterAction.Items, 0)]
        [TestCase(CharacterAction.Items, 1)]
        [TestCase(CharacterAction.Magic, 3)]
        public void Reset(CharacterAction action, int target)
        {
            Mock<Character> character = new();
            character.SetupGet(c => c.Name).Returns("Test Character");
            CharacterBattleState characterBattleState = new(character.Object, 0);

            characterBattleState.Action = action;
            characterBattleState.Target = target;
            characterBattleState.Reset();

            AssertBool(characterBattleState.Target == 0).IsTrue();
            AssertBool(characterBattleState.Action == CharacterAction.Attack).IsTrue();
            AssertBool(characterBattleState.ActionModifier == 0).IsTrue();
            AssertBool(characterBattleState.HasBattleEffect == false).IsTrue();
        }
    }
}
