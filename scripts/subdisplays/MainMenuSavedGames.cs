using Godot;
using System;

public partial class MainMenuSavedGames : CanvasLayer
{
	[Signal]
	public delegate void BackButtonTrigerredEventHandler();

	[Export]
	public PackedScene FirstRoom { get; set; }

	private Global global;
	private Button loadButton;
	private SaveFileOption[] saveButtons = new SaveFileOption[3];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		loadButton = GetNode<Button>("%LoadButton");
		saveButtons = new SaveFileOption[3] {
			GetNode<SaveFileOption>("Save1"),
			GetNode<SaveFileOption>("Save2"),
			GetNode<SaveFileOption>("Save3")
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionPressed("ui_cancel"))
		{
			loadButton.GrabFocus();
		}
    }

	public void FocusFirst()
	{
		loadButton.GrabFocus();
	}

    public void UpdateSaveData()
	{
		for (int i = 0; i < saveButtons.Length; i++)
		{
			SaveFileOption button = saveButtons[i];
			string fileName = saveButtons[i].Name.ToString().ToLower();
			SaveFileData save1data = global.ReadSaveFileData(fileName);
			button.ShowData(save1data);
		}
	}

	public void OnLoadButton()
	{
		saveButtons[0].GrabFocus();
	}

	public void OnSaveButton(int saveNumber)
	{
		global.LoadGame($"save{saveNumber}");
		global.ChangeRoom(FirstRoom);
	}

	public void OnBackButton()
	{
		EmitSignal(SignalName.BackButtonTrigerred);
	}
}
