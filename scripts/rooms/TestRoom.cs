using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class TestRoom : BaseRoom
    {
        public override void _Ready()
        {
            base._Ready();
        }

        private async void TestCutscene()
        {
            await PlayCutscene("test_anim");
            //testDummy.MakeFollower();
            global.CurrentRoom.Player.AddAlly("ZenDummy", false);
        }
    }
}
