using Godot;
using System;

public partial class DeleteFileConfirmation : Display
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

    public override void ShowDisplay()
    {
        throw new NotImplementedException();
    }

    public void ShowDisplay(string saveName)
	{
		this.saveName = saveName;
		label.Text = $"Are you sure you want to delete {saveName}?";		
		Show();
		yesButton.GrabFocus();
	}

    public override void UpdateDisplay()
    {
        throw new NotImplementedException();
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
