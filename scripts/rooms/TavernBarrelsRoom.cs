using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Interactables;

namespace TheWizardCoder.Rooms
{
	public partial class TavernBarrelsRoom : BaseRoom
	{
		const int tilesetY = 7;
		int[] tilesetX = { 5, 6, 7, 8, 9, 10 , 11, 12, 13, 14};
		Vector2I atlasBarrel = new(11, 5);

		[Export]
		public Resource DialogueResource { get; set; }

		private CharacterDialoguePoint berry;
		private TileMap tileMap;
		private NinePatchRect infoBoard;
		private Label indexLabel;
		private Label quantityLabel;


		public override async void OnReady()
		{
			base.OnReady();
			berry = GetNode<CharacterDialoguePoint>("Berry");
			tileMap = GetNode<TileMap>("TileMap");
			infoBoard = GetNode<NinePatchRect>("%InfoBoard");
			indexLabel = GetNode<Label>("%IndexLabel");
			quantityLabel = GetNode<Label>("%QuantityLabel");

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

		private async void OnSubmitted(bool solved)
		{
            List<CodeProblemItem> items = global.CurrentRoom.CodeProblemPanel.LastProblemItems;
			items.Reverse();
			GD.Print(items.Count);
			
			if (items.Count > 0)
			{
				infoBoard.Show();
                int quantity = 0;
				int toAdd = 0;
                indexLabel.Text = "i = 0";
				quantityLabel.Text = "q = 0";

				for (int i = 0; i < items.Count; i++)
				{
					GD.Print(items[i].CurrentAnswer);
				}

				if (items.Count > 1)
				{
					if (items[1].CurrentAnswer == "Fill(25);")
					{
						toAdd = 25;
					}
					else if (items[1].CurrentAnswer == "Fill(5);")
					{
						toAdd = 5;
					}
				}

				if (items[0].CurrentAnswer == "i < 7;")
				{
					for (int i = 0; i < 7; i++)
					{						
						quantity += toAdd;

						indexLabel.Text = $"i = {i}";
						quantityLabel.Text = $"q = {quantity}";

						if (i != 0)
						{
							tileMap.SetCell(0, new Vector2I(tilesetX[i-1], tilesetY), 0, atlasBarrel, 0);
						}
						
						tileMap.SetCell(0, new Vector2I(tilesetX[i], tilesetY), 0, atlasBarrel, 1);

						SceneTreeTimer timerA = GetTree().CreateTimer(1);
						await ToSignal(timerA, SceneTreeTimer.SignalName.Timeout);
					}

					tileMap.SetCell(0, new Vector2I(tilesetX[6], tilesetY), 0, atlasBarrel, 0);
				}
				else if (items[0].CurrentAnswer == "i < 10;")
				{
					for (int i = 0; i < 10; i++)
					{						
						quantity += toAdd;

						indexLabel.Text = $"i = {i}";
						quantityLabel.Text = $"q = {quantity}";

						if (i != 0)
						{
							tileMap.SetCell(0, new Vector2I(tilesetX[i-1], tilesetY), 0, atlasBarrel, 0);
						}
						
						tileMap.SetCell(0, new Vector2I(tilesetX[i], tilesetY), 0, atlasBarrel, 1);

						SceneTreeTimer timerA = GetTree().CreateTimer(1);
						await ToSignal(timerA, SceneTreeTimer.SignalName.Timeout);
					}

					tileMap.SetCell(0, new Vector2I(tilesetX[9], tilesetY), 0, atlasBarrel, 0);

					if (solved)
					{
						OnProblemSolved();
					}
				}

				infoBoard.Hide();

				
			}
		}

		private async void OnProblemSolved()
		{
			await PlayCutscene("code_solved");
			global.PlayerData.AddToInventory("'r'");
			global.PlayerData.HasSolvedTavernGlitch = true;
		}
	}
}