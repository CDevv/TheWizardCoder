using Godot;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Utils;

namespace TheWizardCoder.Subdisplays
{
	public partial class DeleteFileConfirmation : Display
	{
		[Signal]
		public delegate void FileDeletedEventHandler();

		private Label label;
		private Button yesButton;
		private string saveName;

		public override void _Ready()
		{
			base._Ready();
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

		private void ConfirmAction(bool fileIsDeleted)
		{
			if (fileIsDeleted)
			{
				global.SaveFiles.DeleteSaveFile(saveName);
				EmitSignal(SignalName.FileDeleted);
			}
			Hide();
		}
	}
}