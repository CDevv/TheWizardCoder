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
		private string[] partNames = new string[] { "intro_1", "intro_2", "intro_3" };

		[Export]
		public Resource DialogueResource { get; set; }

		private Global global;
		private TransitionRect transition;
		private Label label;
		private Label userInputLabel;
		private DialogueLine line;
		private double waitingTime = DefaultWaitingTime;
		private bool waitingForInput = false;
		private string userInput;
		private int currentLine = 0;
		private AnimationPlayer animationPlayer;

		public override async void _Ready()
		{
			global = GetNode<Global>("/root/Global");
			animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			transition = GetNode<TransitionRect>("TransitionRect");
			label = GetNode<Label>("%IntroLabel");
			userInputLabel = GetNode<Label>("%UserInputLabel");

			line = await DialogueManager.GetNextDialogueLine(DialogueResource, "game_intro");

			transition.Show();
			transition.PlayAnimationBackwards();

			label.Text = global.GameIntroStrings["intro_1"];
		}

		public override async void _Input(InputEvent @event)
		{
			if (Input.IsActionJustPressed("ui_accept"))
			{
				animationPlayer.Play("hide");
				await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);

				currentLine++;
				if (currentLine >= partNames.Length)
				{
					global.CreateSaveFile(global.ChosenSaveSlot, userInput);
					global.ChangeRoom("first_room", "AfterCutsceneMarker", Direction.Down);
					return;
				}

				if (currentLine == partNames.Length - 1)
				{
					waitingForInput = true;
				}

				string title = partNames[currentLine];
				label.Text = global.GameIntroStrings[title];

				animationPlayer.Play(title);
				await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);

				animationPlayer.PlayBackwards("hide");
				await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			}

			if (@event is InputEventKey && @event.IsPressed())
			{
				InputEventKey inputKey = (InputEventKey)@event;
				if (waitingForInput)
				{
					string keyString = inputKey.Keycode.ToString().ToLower();

					if (inputKey.Keycode == Key.Enter)
					{
						label.Text += "\n";
						waitingForInput = false;

						global.CreateSaveFile(global.ChosenSaveSlot, userInput);
						global.ChangeRoom("first_room", "AfterCutsceneMarker", Direction.Down);
						return;
					}
					else
					{
						char keyChar = (char)inputKey.Unicode;
						if (char.IsLetter(keyChar))
						{
							userInput += inputKey.AsTextKeycode();
							userInputLabel.Text = userInput;
						}

						if (inputKey.Keycode == Key.Backspace)
						{
							userInput = userInput.Substring(0, userInput.Length - 1);
							userInputLabel.Text = userInput;
						}
					}
				}
			}
		}
	}
}