using Godot;
using System;
using DialogueManagerRuntime;
using System.Linq;
using TheWizardCoder.Autoload;
using TheWizardCoder.Enums;
using TheWizardCoder.UI;

namespace TheWizardCoder.Displays
{
	public partial class GameIntroDisplay : CanvasLayer
	{
		private const double DefaultWaitingTime = 2;


		[Export]
		public Resource DialogueResource { get; set; }

		private Global global;
		private TransitionRect transition;
		private Label label;
		private DialogueLine line;
		private double waitingTime = DefaultWaitingTime;
		private bool waitingForInput = false;
		private string userInput;

		public override async void _Ready()
		{
			global = GetNode<Global>("/root/Global");
			transition = GetNode<TransitionRect>("TransitionRect");
			label = GetNode<Label>("%IntroLabel");

			line = await DialogueManager.GetNextDialogueLine(DialogueResource, "game_intro");

			transition.Show();
			transition.PlayAnimationBackwards();
		}

		public override async void _Process(double delta)
		{
			if (waitingForInput)
			{
				return;
			}

			if (line == null)
			{
				global.CreateSaveFile(global.ChosenSaveSlot, userInput);
				global.ChangeRoom("first_room", "AfterCutsceneMarker", Direction.Down);
			}

			if (waitingTime > 0)
			{
				waitingTime -= delta;
			}
			else
			{
				waitingTime = DefaultWaitingTime;
				if (line.Tags.Contains("format"))
				{
					label.Text += $"{string.Format(line.Text, userInput)}\n";
				}
				else
				{
					label.Text += $"{line.Text}\n";
				}

				if (line.Responses.Count > 0 || line.Tags.Contains("input"))
				{
					waitingForInput = true;
					return;
				}

				line = await DialogueManager.GetNextDialogueLine(DialogueResource, line.NextId);
			}
		}

		public override async void _Input(InputEvent @event)
		{
			if (@event is InputEventKey && @event.IsPressed())
			{
				InputEventKey inputKey = (InputEventKey)@event;
				if (waitingForInput)
				{
					string keyString = inputKey.Keycode.ToString().ToLower();
					
					if (line.Responses.Count > 0)
					{
						DialogueResponse response = line.Responses.FirstOrDefault(x => x.Text == keyString);
						if (response != null)
						{	label.Text += $"{keyString}\n";
							line = await DialogueManager.GetNextDialogueLine(DialogueResource, response.NextId);
							waitingForInput = false;
						}
					}
					else
					{
						if (inputKey.Keycode == Key.Enter)
						{
							label.Text += "\n";
							line = await DialogueManager.GetNextDialogueLine(DialogueResource, line.NextId);
							waitingForInput = false;
						}
						else
						{
							userInput += inputKey.AsTextKeycode();
							label.Text += inputKey.AsTextKeycode();
						}
					}
				}
			}
		}
	}
}