using Godot;
using System;

public partial class TransitionRect : ColorRect
{
	[Signal]
	public delegate void AnimationFinishedEventHandler();

	private AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public async void PlayAnimation()
	{
		animationPlayer.Play("transition");
		await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		EmitSignal(SignalName.AnimationFinished);
	}

	public void PlayAnimationBackwards()
	{
		animationPlayer.PlayBackwards("transition");
	}
}
