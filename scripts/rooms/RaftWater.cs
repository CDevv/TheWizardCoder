using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class RaftWater : BaseRoom
{
	[Export]
	public Resource DialogueResource { get; set; }

    public override async void OnReady()
    {
        base.OnReady();

		await PlayCutscene("water_1");
    }
}
