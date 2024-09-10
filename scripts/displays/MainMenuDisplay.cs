using Godot;
using System;

public partial class MainMenuDisplay : Display
{
	private Control main;
	private Control savedGames;
	private Control options;
	private Button playButton;
	private Button loadButton;
	private MainMenuSavedGames savedGamesMenu;
	private OptionsMenu optionsMenu;
	private ControlsMenu controlsMenu;

	private bool waitingInput = false;
	private string actionName;

	public override void _Ready()
	{
		base._Ready();

		main = GetNode<Control>("%Main");
		options = GetNode<Control>("Options");
		playButton = GetNode<Button>("%PlayButton");
		savedGamesMenu = GetNode<MainMenuSavedGames>("%SavedGamesMenu");
		optionsMenu = GetNode<OptionsMenu>("%OptionsMenu");
		controlsMenu = GetNode<ControlsMenu>("%ControlsMenu");

		savedGamesMenu.UpdateDisplay();
		playButton.CallDeferred(Button.MethodName.GrabFocus);
		optionsMenu.UpdateDisplay();
		controlsMenu.UpdateDisplay();
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
		global.CurrentRoom.TransitionRect.PlayAnimation();
		await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
    }

    public override void UpdateDisplay()
    {
        optionsMenu.UpdateDisplay();
		controlsMenu.UpdateDisplay();
		savedGamesMenu.UpdateDisplay();
    }

    public void ShowMainMenu()
	{
		main.Show();
		savedGamesMenu.HideDisplay();
		optionsMenu.HideDisplay();
		controlsMenu.HideDisplay();
		playButton.GrabFocus();
	}

	public void ShowSavedGamesMenu()
	{
		main.Hide();
		optionsMenu.HideDisplay();
		savedGamesMenu.ShowDisplay();	
	}

	public void ShowOptions()
	{
		main.Hide();
		savedGamesMenu.HideDisplay();
		controlsMenu.HideDisplay();
		optionsMenu.ShowDisplay();
	}

	public void ShowControls()
	{
		optionsMenu.HideDisplay();
		controlsMenu.ShowDisplay();
	}
}
