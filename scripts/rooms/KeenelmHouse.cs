using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class KeenelmHouse : BaseRoom
    {
        [Export]
        public Resource DialogueResource { get; set; }

        public override async void OnReady()
        {
            base.OnReady();

            if (global.PlayerData.HasSolvedFarmGlitch)
            {
                if (!global.PlayerData.HasReceivedAppleFromNara)
                {
                    await ShowDialogue(DialogueResource, "nara_code_solved_1");
                    await PlayCutscene("nara_code_solved_1");
                    await ShowDialogue(DialogueResource, "nara_code_solved_2");
                    await PlayCutscene("nara_code_solved_2");
                    await ShowDialogue(DialogueResource, "nara_code_solved_3");
                    global.PlayerData.HasReceivedAppleFromNara = true;
                }
            }
            else if (!global.PlayerData.HasEncounteredNara)
            {
                await PlayCutscene("nara_1");
                await ShowDialogue(DialogueResource, "nara_1");
                await PlayCutscene("nara_2");
            }
        }
    }
}