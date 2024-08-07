using Godot;
using System;

public partial class MainMenuSavedGames : CanvasLayer
{
	[Signal]
	public delegate void BackButtonTrigerredEventHandler();

	[Export]
	public PackedScene FirstRoom { get; set; }
	[Export]
	public DeleteFileConfirmation confirmation;

	private Global global;
	private Button loadButton;
	private SaveFileOption[] saveButtons = new SaveFileOption[3];
	private SaveFileAction mode;

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
			SaveFileData saveData = global.ReadSaveFileData(fileName);
			button.ShowData(saveData);
		}
	}

	private void OnLoadButton()
	{
		mode = SaveFileAction.Load;
		saveButtons[0].GrabFocus();
	}

	private void OnDeleteButton()
	{
		mode = SaveFileAction.Delete;
		saveButtons[0].GrabFocus();
	}

	private void OnSaveButton(int saveNumber)
	{
		if (mode == SaveFileAction.Load)
		{
			global.LoadSaveFile($"save{saveNumber}");
			global.ChangeRoom(global.PlayerData.SceneFileName, global.PlayerData.SceneDefaultMarker, Direction.Down);
		}
		else
		{
			confirmation.ShowDisplay($"save{saveNumber}");
		}
	}

	public void OnBackButton()
	{
		EmitSignal(SignalName.BackButtonTrigerred);
	}

	private void OnFileDeleted()
	{
		UpdateSaveData();
		loadButton.GrabFocus();
	}
}
