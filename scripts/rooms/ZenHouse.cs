using Godot;
using System;
using System.Collections.Generic;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Interactables;
using TheWizardCoder.Data;

namespace TheWizardCoder.Rooms
{
	public partial class ZenHouse : BaseRoom
	{
		[Export]
		public Resource DialogueResource { get; set; }

		private AnimatedSprite2D basket;
		private DialoguePoint basketDialogue;

		public override async void OnReady()
		{
			base.OnReady();
			basket = GetNode<AnimatedSprite2D>("Basket");
			basketDialogue = GetNode<DialoguePoint>("BasketDialoguePoint");

			if (!global.PlayerData.HasVisitedZenHouse)
			{
				await ShowDialogue(DialogueResource, "zen_intro_1");
				await PlayCutscene("zen_intro");
				await ShowDialogue(DialogueResource, "zen_intro_2");
			}
			else if (global.PlayerData.HasSolvedZenHouse)
			{
				basketDialogue.DialogueTitle = "basket_full";
				AnimationPlayer.Play("final_pos");
			}
			
		}

		private void OnCodeProblemItemsChanged()
		{
			List<CodeProblemItem> codeProblemItems = CodeProblemPanel.ProblemItems;

			if (codeProblemItems[0].CurrentAnswer == "+" && codeProblemItems[1].CurrentAnswer == ".Fill")
			{
				basket.Frame = 1;
				basketDialogue.DialogueTitle = "basket_full";
			}
			else
			{
				basket.Frame = 2;
				basketDialogue.DialogueTitle = "basket_corrupted";
			}
		}

		private async void OnCodeProblemSolved()
		{
			await PlayCutscene("code_solved");
			await ShowDialogue(DialogueResource, "zen_code_solved");
			AnimationPlayer.Play("zen_down");
			global.PlayerData.AddToInventory("'t'");
		}
	}
}