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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		save1button = GetNode<SaveFileOption>("Save1");
		save2button = GetNode<SaveFileOption>("Save2");
		save3button = GetNode<SaveFileOption>("Save3");
		saveButton = GetNode<Button>("%SaveButton");
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

	public void FocusOnFirstSave()
	{
		save1button.GrabFocus();
	}

	public void OnSaveButton()
	{
		global.SaveGame(selectedSaveFileName);
		Hide();
		global.CanWalk = true;
	}

	public void OnSave1()
	{
		selectedSaveFileName = "save1";
		saveButton.GrabFocus();
	}

	public void OnSave2()
	{
		selectedSaveFileName = "save2";
		saveButton.GrabFocus();
	}

	public void OnSave3()
	{
		selectedSaveFileName = "save3";
		saveButton.GrabFocus();
	}
}
