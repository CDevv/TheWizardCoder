using Godot;
using System;

public partial class ShimbleHouse : BaseRoom
{
	[Export]
	public Resource DialogueResource { get; set; }

	private CodeProblemPoint codeProblem;

    public override async void OnReady()
    {
        base.OnReady();
		if (!global.PlayerData.HasQuestFromShimble)
		{
			await PlayCutscene("shimble_1");
			ShowDialogue(DialogueResource, "intro_shimble_1");
			await ToSignal(Dialogue, DialogueDisplay.SignalName.DialogueEnded);
			await PlayCutscene("shimble_2");
			ShowDialogue(DialogueResource, "intro_shimble_2");
			await ToSignal(Dialogue, DialogueDisplay.SignalName.DialogueEnded);
		}
		else if (global.PlayerData.HasQuestFromShimble && !global.PlayerData.HasSolvedShimbleChair)
		{
			AnimationPlayer.Play("shimble_new_pos");
		}
		else
		{
			AnimationPlayer.Play("shimble_new_pos");
			AnimationPlayer.Play("problem_solved_perm");
		}
    }

	private async void OnProblemSolved()
	{
		await PlayCutscene("problem_solved");
		ShowDialogue(DialogueResource, "shimble_chair_solved");
		await ToSignal(Dialogue, DialogueDisplay.SignalName.DialogueEnded);
	}
}
