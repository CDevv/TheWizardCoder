using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class RaftWater : BaseRoom
{
	[Export]
	public Resource DialogueResource { get; set; }

    private double passedTime = 0;

    public override async void OnReady()
    {
        base.OnReady();

		await PlayCutscene("water_1");

    }

    public override void _Process(double delta)
    {
        passedTime += delta;
    }

    private void OnDialogueEnded(string initialTitle, string message)
    {
        GD.Print(passedTime);
        GD.Print(global.CurrentRoom.Player.Position);
        GD.Print(global.CurrentRoom.Gertrude.Position);
    }
}
