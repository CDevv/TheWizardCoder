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

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		save1button = GetNode<SaveFileOption>("Save1");
		save2button = GetNode<SaveFileOption>("Save2");
		save3button = GetNode<SaveFileOption>("Save3");
		saveButton = GetNode<Button>("%SaveButton");
		closeButton = GetNode<Button>("%CloseButton");
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

	public void OnSaveOption(string saveName)
	{
		global.SaveGame(saveName);
		OnCloseButton();
	}
}
