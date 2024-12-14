using Godot;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.UI;
using TheWizardCoder.Subdisplays;

namespace TheWizardCoder.Displays
{
	public partial class MainMenuDisplay : Display
	{
		private TransitionRect transition;
		private Control main;
		private Control savedGames;
		private Control options;
		private Button playButton;
		private Button loadButton;
		private bool waitingInput = false;
		private string actionName;

		public override void _Ready()
		{
			base._Ready();

			transition = GetNode<TransitionRect>("TransitionRect");
			main = GetNode<Control>("Main");
			playButton = GetNode<Button>("%PlayButton");

			AddSubdisplay("SavedGames", GetNode<MainMenuSavedGames>("SavedGamesMenu"));
			AddSubdisplay("Options", GetNode<OptionsMenu>("OptionsMenu"));
			AddSubdisplay("Controls", GetNode<ControlsMenu>("ControlsMenu"));

			playButton.CallDeferred(Button.MethodName.GrabFocus);

			UpdateDisplay();
			ShowDisplay();
		}

		public override void _Input(InputEvent @event)
		{
			if (waitingInput)
			{
				if (@event is InputEventKey && @event.IsPressed())
				{
					global.Settings.ChangeControl(actionName, @event);
					waitingInput = false;
				}
			}
		}

		public override async void ShowDisplay()
		{
			Show();
			ShowMainMenu();
			transition.PlayAnimation();
			await ToSignal(transition, TransitionRect.SignalName.AnimationFinished);
		}

		public override void UpdateDisplay()
		{
			UpdateAllSubdisplays();
		}

		public void ShowMainMenu()
		{
			main.Show();
			HideAllSubdisplays();
			playButton.GrabFocus();
			playButton.CallDeferred(Button.MethodName.GrabFocus);
		}

		public void ShowSavedGamesMenu()
		{
			main.Hide();
			ChangeSubdisplay("SavedGames");
		}

		public void ShowOptions()
		{
			main.Hide();
			ChangeSubdisplay("Options");
		}

		public void ShowControls()
		{
			ChangeSubdisplay("Controls");
		}
	}
}