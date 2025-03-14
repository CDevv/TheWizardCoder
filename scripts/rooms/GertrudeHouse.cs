using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class GertrudeHouse : BaseRoom
    {
        [Export]
        public Resource DialogueResource { get; set; }

        public override async void OnReady()
        {
            base.OnReady();

            if (!global.PlayerData.HasMetGertrude)
            {
                await PlayCutscene("gertrude_1");
                await ShowDialogue(DialogueResource, "gertrude_intro_1");
                await PlayCutscene("gertrude_2");
                await ShowDialogue(DialogueResource, "gertrude_intro_2");

                global.CurrentRoom.Player.AddAlly("Gertrude", false);
                global.PlayerData.HasMetGertrude = true;
            }
        }
    }
}
