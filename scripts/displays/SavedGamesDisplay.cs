using Godot;
using System;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;
using TheWizardCoder.Abstractions;
using TheWizardCoder.UI;

namespace TheWizardCoder.Displays
{
	public partial class SavedGamesDisplay : Display
	{
		private string selectedSaveFileName;
		private SaveFileOption save1button;
		private SaveFileOption save2button;
		private SaveFileOption save3button;
		private Button saveButton;
		private Button closeButton;

		public override void _Ready()
		{
			base._Ready();
			save1button = GetNode<SaveFileOption>("Save1");
			save2button = GetNode<SaveFileOption>("Save2");
			save3button = GetNode<SaveFileOption>("Save3");
			saveButton = GetNode<Button>("%SaveButton");
			closeButton = GetNode<Button>("%CloseButton");
		}

		public override void UpdateDisplay()
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

		public override void ShowDisplay()
		{
			global.GameDisplayEnabled = false;
			Show();
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
			global.GameDisplayEnabled = true;
		}

		public void OnSaveOption(string saveName)
		{
			global.UpdateSaveFile(saveName);
			OnCloseButton();
		}

	}
}