using Godot;
using System;

public partial class DeleteFileConfirmation : CanvasLayer
{
	[Signal]
	public delegate void FileDeletedEventHandler();

	private Global global;
	private Label label;
	private Button yesButton;
	private string saveName;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		label = GetNode<Label>("%WarningLabel");
		yesButton = GetNode<Button>("%YesButton");
	}

	public void ShowDisplay(string saveName)
	{
		this.saveName = saveName;
		label.Text = $"Are you sure you want to delete {saveName}?";		
		Show();
		yesButton.GrabFocus();
	}

	private void ConfirmAction(bool fileIsDeleted)
	{
		if (fileIsDeleted)
		{
			global.DeleteSaveFile(saveName);
			EmitSignal(SignalName.FileDeleted);
		}
		Hide();
	}
}
