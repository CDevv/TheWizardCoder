using Godot;
using System;
using TheWizardCoder.Data;

namespace TheWizardCoder.UI
{
	public partial class SaveFileOption : Button
	{
		private Color normalColor = new Color(255, 255, 255);
		private Color focusedColor = new Color(255, 255, 255, .5f);

		private Label saveNameLabel;
		private Label locationLabel;
		private Label timeSpentLabel;
		
		public override void _Ready()
		{
			saveNameLabel = GetNode<Label>("%SaveNameLabel");
			locationLabel = GetNode<Label>("%LocationLabel");
			timeSpentLabel = GetNode<Label>("%TimeSpentLabel");
		}

		public void SetSaveNameText(string text)
		{
			saveNameLabel.Set(Label.PropertyName.Text, text);
		}

		public void SetLocationText(string text)
		{
			locationLabel.Set(Label.PropertyName.Text, text);
		}

		public void SetTimeSpentText(string text)
		{
			timeSpentLabel.Set(Label.PropertyName.Text, text);
		}

		public void ShowAsEmptyFile()
		{
			SetSaveNameText("NEW GAME");
			locationLabel.Hide();
			timeSpentLabel.Hide();
		}

		public void ShowAsExistingFile()
		{
			locationLabel.Show();
			timeSpentLabel.Show();
		}

		public void ShowData(SaveFileData saveFile)
		{
			var duration = saveFile.TimeSpent;
			if (saveFile.IsSaveEmpty)
			{
				ShowAsEmptyFile();
			}
			else
			{
				ShowAsExistingFile();
				SetSaveNameText(saveFile.SaveName);
				SetLocationText(saveFile.Location);

				string hoursString = duration.Hours > 9 ? $"{duration.Hours}" : $"0{duration.Hours}";
				string minutesString = duration.Minutes > 9 ? $"{duration.Minutes}" : $"0{duration.Minutes}";
				string secondsString = duration.Seconds > 9 ? $"{duration.Seconds}" : $"0{duration.Seconds}";
				SetTimeSpentText($"{hoursString}:{minutesString}:{secondsString}");
			}
		}

		public void OnFocusEntered()
		{
			saveNameLabel.Set("theme_override_colors/font_color", focusedColor);
			locationLabel.Set("theme_override_colors/font_color", focusedColor);
			timeSpentLabel.Set("theme_override_colors/font_color", focusedColor);
		}

		public void OnFocusExited()
		{
			saveNameLabel.Set("theme_override_colors/font_color", normalColor);
			locationLabel.Set("theme_override_colors/font_color", normalColor);
			timeSpentLabel.Set("theme_override_colors/font_color", normalColor);
		}
	}
}