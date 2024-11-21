using Godot;
using System;

namespace TheWizardCoder.UI
{
	public partial class TransitionRect : CanvasLayer
	{
		[Signal]
		public delegate void AnimationFinishedEventHandler();

		private AnimationPlayer animationPlayer;
		private ColorRect colorRect;

		public override void _Ready()
		{
			animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			colorRect = GetNode<ColorRect>("ColorRect");
		}

		public async void PlayAnimation()
		{
			animationPlayer.Play("transition");
			await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			EmitSignal(SignalName.AnimationFinished);
		}

		public async void PlayAnimation(float duration)
		{
			colorRect.Modulate = new Color(255, 255, 255, 0);
			Show();

			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(colorRect, "modulate", new Color(255, 255, 255), duration);

			tween.Play();

			await ToSignal(tween, Tween.SignalName.Finished);
		}

		public async void PlayAnimationBackwards()
		{
			animationPlayer.PlayBackwards("transition");
			await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			EmitSignal(SignalName.AnimationFinished);
		}

		public async void PlayAnimationBackwards(float duration)
		{
			colorRect.Modulate = new Color(255, 255, 255);
			Show();

			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(colorRect, "modulate", new Color(255, 255, 255, 0), duration);

			tween.Play();
			GD.Print("played");

			await ToSignal(tween, Tween.SignalName.Finished);
		}
	}
}