using Godot;
using TheWizardCoder.Abstractions;

public partial class TimothyHouse : BaseRoom
{
    [Export]
    public Resource DialogueResource { get; set; }

    public override async void OnReady()
    {
        base.OnReady();

        if (!global.PlayerData.VisitedTimothyHouse)
        {
            await PlayCutscene("timothy_intro");
            await ShowDialogue(DialogueResource, "timothy_intro");

            global.PlayerData.VisitedTimothyHouse = true;
        }
    }
}
