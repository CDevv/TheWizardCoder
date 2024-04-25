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
	private SaveFileOption save1button;
	private SaveFileOption save2button;
	private SaveFileOption save3button;
	private OptionButton resolutionOption;
	private ControlSelectButton upControlOption;
	private NinePatchRect optionsMainPage;
	private NinePatchRect controlsPage;

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
		loadButton = GetNode<Button>("%LoadButton");
		save1button = GetNode<SaveFileOption>("%Save1");
		save2button = GetNode<SaveFileOption>("%Save2");
		save3button = GetNode<SaveFileOption>("%Save3");
		resolutionOption = GetNode<OptionButton>("%ResolutionOptions");
		upControlOption = GetNode<ControlSelectButton>("%UpButton");
		optionsMainPage = GetNode<NinePatchRect>("%OptionsRect");
		controlsPage = GetNode<NinePatchRect>("%ControlsRect");

		UpdateSaveData();
		playButton.CallDeferred(Button.MethodName.GrabFocus);
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

    private void UpdateSaveData()
	{
		SaveFileOption[] buttons = { save1button, save2button, save3button };
		for (int i = 0; i < buttons.Length; i++)
		{
			SaveFileOption button = buttons[i];
			string fileName = buttons[i].Name.ToString().ToLower();
			SaveFileData save1data = global.ReadSaveFileData(fileName);
			button.ShowData(save1data);
		}
	}

	public void ShowMainMenu()
	{
		main.Show();
		savedGames.Hide();
		options.Hide();
		playButton.GrabFocus();
	}

	public void ShowSavedGamesMenu()
	{
		main.Hide();
		savedGames.Show();
		options.Hide();
		loadButton.GrabFocus();
	}

	public void ShowOptions()
	{
		main.Hide();
		savedGames.Hide();
		options.Show();
		resolutionOption.GrabFocus();
	}

	public void OnLoad()
	{
		save1button.GrabFocus();
	}

	public void OnGoBack()
	{
		ShowMainMenu();
	}

	public void OnSave1()
	{
		global.LoadGame("save1");
		global.ChangeRoom(firstRoom);
	}

	public void OnSave2()
	{
		global.LoadGame("save2");
		global.ChangeRoom(firstRoom);
	}

	public void OnSave3()
	{
		global.LoadGame("save3");
		global.ChangeRoom(firstRoom);
	}

	public void OnWindowSizeChanged(int optionId)
	{
		WindowSize size = (WindowSize)optionId;
		global.ChangeWindowSize(size);
		global.Settings.SaveSettings();
	}

	public void OnFullscreenToggled(bool toggled)
	{
		global.Settings.ToggleFullscreen(toggled);
	}

	public void OnControlsPage()
	{
		optionsMainPage.Hide();
		controlsPage.Show();
		upControlOption.GrabFocus();
	}

	public void OnOptionsPage()
	{
		optionsMainPage.Show();
		controlsPage.Hide();
		resolutionOption.GrabFocus();
	}

	public void OnControlChangeButton(string action)
	{
		actionName = action;
		waitingInput = true;
	}
}
