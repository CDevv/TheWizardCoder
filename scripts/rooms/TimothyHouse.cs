using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class TimothyHouse : BaseRoom
{
    public override async void OnReady()
    {
        base.OnReady();

        if (!global.PlayerData.VisitedTimothyHouse)
        {
            await PlayCutscene("timothy_intro");
        }
    }
}
