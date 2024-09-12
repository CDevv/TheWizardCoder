using Godot;
using System;

public partial class Village4 : BaseRoom
{
	[Export]
	public Resource DialogueResource { get; set; }

	private Warper watchtowerWarper;
	private CodeProblemPoint codeProblem;
	private int tutorialProgress = 1;

	public override async void OnReady()
	{
		base.OnReady();
		watchtowerWarper = GetNode<Warper>("WatchtowerWarper");
		codeProblem = GetNode<CodeProblemPoint>("CodeProblemPoint");

		if (global.PlayerData.HasSolvedWatchtowerGlitch)
		{
			AnimationPlayer.Play("final_pos");
			codeProblem.Solved = true;
			watchtowerWarper.Enabled = true;

			if (!global.PlayerData.LintonDummyCutscene && global.PlayerData.HasMetLinton)
			{
				global.PlayerData.LintonDummyCutscene = true;
				await PlayCutscene("linton_4");
				await ShowDialogue(DialogueResource, "linton_3");
				BattleDisplay.IsTutorial = true;
				BattleDisplay.ShowDisplay(new() {"Dummy"});
			}
			else if(global.PlayerData.LintonDummyCutscene)
			{
				AnimationPlayer.Play("linton_7");
			}
		}
	}

	private async void OnCodeSolved()
	{
		await PlayCutscene("code_solved");
		watchtowerWarper.Enabled = true;
		global.PlayerData.HasSolvedWatchtowerGlitch = true;
	}

	private async void OnBattleFinished()
	{
		await PlayCutscene("linton_6");
		await ShowDialogue(DialogueResource, "linton_4");
		await PlayCutscene("linton_7");		
	}

	private async void OnTurnFinished()
	{
		if (tutorialProgress < 3)
		{
			await ShowDialogue(DialogueResource, $"tutorial_battle_{tutorialProgress}");
			global.GameDisplayEnabled = false;
			tutorialProgress++;
		}
	}
}
