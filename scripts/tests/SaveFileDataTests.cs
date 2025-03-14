using GdUnit4;
using Moq;
using System;
using TheWizardCoder.Data;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class SaveFileDataTests
    {
        [TestCase("Nolan")]
        [TestCase("Gertrude")]
        [TestCase("Dummy")]
        [TestCase("Kris")]
        public void ValidConstructorCalls(string characterName)
        {
            Mock<Character> character = new Mock<Character>();
            character.SetupGet(c => c.Name).Returns(characterName);

            SaveFileData saveFileData = new(character.Object);

            AssertBool(saveFileData.Stats.Name == characterName).IsTrue();
            AssertBool(saveFileData.IsSaveEmpty).IsTrue();
            AssertBool(saveFileData.StartedOn.Day == DateTime.Now.Day).IsTrue();
            AssertBool(saveFileData.LastSaved.Day == DateTime.Now.Day).IsTrue();
            AssertBool(saveFileData.TimeSpent == TimeSpan.Zero).IsTrue();
            AssertBool(saveFileData.SceneFileName == "first_room").IsTrue();
            AssertBool(saveFileData.Location == "Home").IsTrue();
            AssertBool(saveFileData.SceneDefaultMarker == "AfterCutsceneMarker").IsTrue();
            AssertBool(saveFileData.Gold == 0).IsTrue();

            //Free memory from possible orphan node
            saveFileData.Free();
        }
    }
}
