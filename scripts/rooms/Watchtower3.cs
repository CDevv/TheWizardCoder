using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Watchtower3 : BaseRoom
    {
        [Export]
        public Resource DialogueResource { get; set; }

        public override async void OnReady()
        {
            base.OnReady();
            if (!global.PlayerData.HasMetLinton)
            {
                global.PlayerData.HasMetLinton = true;
                await PlayCutscene("linton_1");
                await ShowDialogue(DialogueResource, "linton_1");
                await PlayCutscene("linton_2");
                await ShowDialogue(DialogueResource, "linton_2");
                await PlayCutscene("linton_3");
            }
            else
            {
                AnimationPlayer.Play("final_pos");
            }
        }
    }
}