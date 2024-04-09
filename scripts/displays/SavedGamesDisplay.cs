using Godot;
using System;

public partial class SavedGamesDisplay : CanvasLayer
{
	private string selectedSaveFileName;
	private Global global;
	private SaveFileOption save1button;
	private SaveFileOption save2button;
	private SaveFileOption save3button;
	private Button saveButton;
	private Button closeButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		save1button = GetNode<SaveFileOption>("Save1");
		save2button = GetNode<SaveFileOption>("Save2");
		save3button = GetNode<SaveFileOption>("Save3");
		saveButton = GetNode<Button>("%SaveButton");
		closeButton = GetNode<Button>("%CloseButton");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateDisplay()
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

	public void FocusOnSaveButton()
	{
		saveButton.GrabFocus();
	}

	public void OnSaveButton()
	{
		save1button.GrabFocus();
	}

	public void OnCloseButton()
	{
		Hide();
		global.CanWalk = true;
	}

	public void OnSave1()
	{
		global.SaveGame("save1");
		OnCloseButton();
	}

	public void OnSave2()
	{
		global.SaveGame("save2");
		OnCloseButton();
	}

	public void OnSave3()
	{
		global.SaveGame("save3");
		OnCloseButton();
	}
}
