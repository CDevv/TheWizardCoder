using Godot;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Interactables;

namespace TheWizardCoder.Rooms
{
	public partial class VillageFarm : BaseRoom
	{
		[Export]
		public Resource DialogueResource { get; set; }

		private CutscenePoint cutscenePoint;

		public override void OnReady()
		{
			base.OnReady();
			cutscenePoint = GetNode<CutscenePoint>("CutscenePoint");

			if (global.PlayerData.HasEncounteredKeenelm)
			{
				DisableCutscene();
			}
			if (global.PlayerData.HasSolvedFarmGlitch)
			{
				AnimationPlayer.Play("final_pos");
			}
		}

		private async void OnKeenelmEncounter()
		{
			await PlayCutscene("keenelm_1");
			await ShowDialogue(DialogueResource, "keenelm_1");
			await PlayCutscene("keenelm_2");
			await ShowDialogue(DialogueResource, "keenelm_2");
		}

		private void DisableCutscene()
		{
			cutscenePoint.Active = false;
		}

		private async void OnProblemSolved()
		{
			await PlayCutscene("problem_solved");
			if (global.PlayerData.HasPlayedKeenelmCutscene)
			{
				await PlayCutscene("keenelm_code_solved_cutscene");
			}
			else
			{
				await PlayCutscene("keenelm_code_solved");
			}
			await ShowDialogue(DialogueResource, "keenelm_code_solved");
			global.PlayerData.AddToInventory("'a'");

			global.PlayerData.Stats.AddLevelPoints(3);
		}
	}
}