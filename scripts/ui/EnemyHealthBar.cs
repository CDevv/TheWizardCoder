using Godot;
using System;
using System.Threading.Tasks;

namespace TheWizardCoder.UI
{
	public partial class EnemyHealthBar : NinePatchRect
	{
		private TextureProgressBar progressBar;
		public override void _Ready()
		{
			progressBar = GetNode<TextureProgressBar>("%ProgressBar");
		}

		public async Task ShowHealthBar(int value, int newValue, int maxValue)
		{
			Show();
			Modulate = new Color(255, 255, 255, 1);
			progressBar.Value = value;
			progressBar.MaxValue = maxValue;

			await PlayAnimation(newValue);
			Hide();
		}

		private async Task PlayAnimation(int newValue)
		{
			Tween tween = CreateTween();
			tween.TweenProperty(progressBar, "value", newValue, 1);
			tween.TweenProperty(this, "modulate", new Color(255, 255, 255, 0), 1);
			await ToSignal(tween, Tween.SignalName.Finished);
			tween.Stop();
		}
	}
}