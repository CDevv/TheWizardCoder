using Godot;
using System;

public partial class MainMenuDisplay : CanvasLayer
{
	[Export]
	public PackedScene firstRoom;

	private Global global;
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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");

		main = GetNode<Control>("%Main");
		savedGames = GetNode<Control>("%SavedGames");
		options = GetNode<Control>("Options");
		playButton = GetNode<Button>("%PlayButton");
		savedGamesMenu = GetNode<MainMenuSavedGames>("%SavedGamesMenu");
		optionsMenu = GetNode<OptionsMenu>("%OptionsMenu");
		controlsMenu = GetNode<ControlsMenu>("%ControlsMenu");

		savedGamesMenu.UpdateSaveData();
		playButton.CallDeferred(Button.MethodName.GrabFocus);
		optionsMenu.UpdateDisplay();
		controlsMenu.UpdateDisplay();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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

	public void ShowMainMenu()
	{
		main.Show();
		savedGames.Hide();
		options.Hide();
		optionsMenu.Hide();
		controlsMenu.Hide();
		playButton.GrabFocus();
	}

	public void ShowSavedGamesMenu()
	{
		main.Hide();
		savedGamesMenu.Show();
		options.Hide();
		savedGamesMenu.FocusFirst();
	}

	public void ShowOptions()
	{
		main.Hide();
		savedGamesMenu.Hide();
		options.Show();

		optionsMenu.Show();
		controlsMenu.Hide();

		optionsMenu.FocusFirst();
	}

	public void ShowControls()
	{
		optionsMenu.Hide();
		controlsMenu.Show();

		controlsMenu.FocusFirst();
	}
}
