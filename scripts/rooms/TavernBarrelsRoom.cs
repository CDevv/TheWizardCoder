using Godot;
using System;

public partial class TavernBarrelsRoom : BaseRoom
{
	[Export]
	public Resource DialogueResource { get; set; }

	private CharacterDialoguePoint berry;

	public override async void OnReady()
	{
		base.OnReady();
		berry = GetNode<CharacterDialoguePoint>("Berry");
		if (!global.PlayerData.TavernPuzzleIntro)
		{
			await PlayCutscene("berry_2");
			await ShowDialogue(DialogueResource, "berry_2");
			await PlayCutscene("berry_3");
			global.PlayerData.TavernPuzzleIntro = true;
			berry.Position = Vector2.Zero;
		}
		else
		{
			berry.Visible = false;
		}

		if (global.PlayerData.HasSolvedTavernGlitch)
		{
			AnimationPlayer.Play("final_pos");
		}
	}

	private async void OnProblemSolved()
	{
		await PlayCutscene("code_solved");
		global.PlayerData.HasSolvedTavernGlitch = true;
	}
}
