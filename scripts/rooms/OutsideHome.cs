using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Displays;

namespace TheWizardCoder.Rooms
{
    public partial class OutsideHome : BaseRoom
    {
        [Export]
        public Resource DialogueResource { get; set; }
        public override async void OnReady()
        {
            base.OnReady();

            if (!global.PlayerData.HasMessageFromShimble)
            {
                global.IsInCutscene = true;

                global.CanWalk = false;
                await PlayCutscene("intro_1");
                Dialogue.ShowDisplay(DialogueResource, "intro_mailman");
                await ToSignal(Dialogue, DialogueDisplay.SignalName.DialogueEnded);
                global.CanWalk = false;
                await PlayCutscene("intro_2");
                Dialogue.ShowDisplay(DialogueResource, "nolan_hmm");

                global.IsInCutscene = false;
                global.PlayerData.HasMessageFromShimble = true;
            }
            else
            {
                AnimationPlayer.Play("hide_zen");
            }
        }
    }
}