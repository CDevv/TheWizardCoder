using GdUnit4;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using static GdUnit4.Assertions;
using Moq;
using Godot;
using TheWizardCoder.Displays;
using System.Threading.Tasks;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class GameDisplayTests
    {
        [TestCase]
        public void ShowDisplaySuccess()
        {
            ISceneRunner runner = ISceneRunner.Load("res://scenes/displays/game_display.tscn", true);
            Node scene = runner.Scene();
            GameDisplay display = (GameDisplay)scene;

            display.ShowDisplay();

            AssertBool(scene.Get("visible").AsBool()).IsTrue();
            AssertBool(display.Subdisplays["Inventory"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Options"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Controls"].Visible).IsFalse();
            AssertBool(display.Subdisplays["PartyMembers"].Visible).IsTrue();
            AssertBool(display.Subdisplays["Status"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Magic"].Visible).IsFalse();
        }

        [TestCase]
        public async Task OnItemsMenu()
        {
            ISceneRunner runner = ISceneRunner.Load("res://scenes/displays/game_display.tscn", true);
            Node scene = runner.Scene();
            GameDisplay display = (GameDisplay)scene;

            display.ShowDisplay();

            runner.SimulateActionPressed("ui_accept");
            await runner.AwaitIdleFrame();

            AssertBool(scene.Get("visible").AsBool()).IsTrue();
            AssertBool(display.Subdisplays["Inventory"].Visible).IsTrue();
            AssertBool(display.Subdisplays["Options"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Controls"].Visible).IsFalse();
            AssertBool(display.Subdisplays["PartyMembers"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Status"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Magic"].Visible).IsFalse();
        }

        [TestCase]
        public async Task OnOptionsMenuAsync()
        {
            ISceneRunner runner = ISceneRunner.Load("res://scenes/displays/game_display.tscn", true);
            Node scene = runner.Scene();
            GameDisplay display = (GameDisplay)scene;

            display.ShowDisplay();

            runner.SimulateActionPressed("ui_down");
            await runner.AwaitIdleFrame();
            runner.SimulateActionPressed("ui_down");
            await runner.AwaitIdleFrame();
            runner.SimulateActionPressed("ui_down");
            await runner.AwaitIdleFrame();
            runner.SimulateActionPressed("ui_accept");
            await runner.AwaitIdleFrame();

            AssertBool(scene.Get("visible").AsBool()).IsTrue();
            AssertBool(display.Subdisplays["Inventory"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Options"].Visible).IsTrue();
            AssertBool(display.Subdisplays["Controls"].Visible).IsFalse();
            AssertBool(display.Subdisplays["PartyMembers"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Status"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Magic"].Visible).IsFalse();
        }

        [TestCase]
        public async Task OnStatusMenu()
        {
            ISceneRunner runner = ISceneRunner.Load("res://scenes/displays/game_display.tscn", true);
            Node scene = runner.Scene();
            GameDisplay display = (GameDisplay)scene;

            display.ShowDisplay();

            runner.SimulateActionPressed("ui_down");
            await runner.AwaitIdleFrame();
            runner.SimulateActionPressed("ui_accept");
            await runner.AwaitIdleFrame();
            runner.SimulateActionPressed("ui_accept");
            await runner.AwaitIdleFrame();

            AssertBool(scene.Get("visible").AsBool()).IsTrue();
            AssertBool(display.Subdisplays["Inventory"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Options"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Controls"].Visible).IsFalse();
            AssertBool(display.Subdisplays["PartyMembers"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Status"].Visible).IsTrue();
            AssertBool(display.Subdisplays["Magic"].Visible).IsFalse();
        }

        [TestCase]
        public async Task OnMagicMenu()
        {
            ISceneRunner runner = ISceneRunner.Load("res://scenes/displays/game_display.tscn", true);
            Node scene = runner.Scene();
            GameDisplay display = (GameDisplay)scene;

            display.ShowDisplay();

            runner.SimulateActionPressed("ui_down");
            await runner.AwaitIdleFrame();
            runner.SimulateActionPressed("ui_down");
            await runner.AwaitIdleFrame();
            runner.SimulateActionPressed("ui_accept");
            await runner.AwaitIdleFrame();
            runner.SimulateActionPressed("ui_accept");
            await runner.AwaitIdleFrame();

            AssertBool(scene.Get("visible").AsBool()).IsTrue();
            AssertBool(display.Subdisplays["Inventory"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Options"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Controls"].Visible).IsFalse();
            AssertBool(display.Subdisplays["PartyMembers"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Status"].Visible).IsFalse();
            AssertBool(display.Subdisplays["Magic"].Visible).IsTrue();
        }
    }
}
