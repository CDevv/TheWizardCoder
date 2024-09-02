using Godot;
using System;

public partial class DamageIndicator : Label
{
	[Signal]
	public delegate void AnimationFinishedEventHandler();

	private AnimationPlayer player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.AnimationFinished += (animName) => {
			EmitSignal(SignalName.AnimationFinished);
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlayAnimation()
	{
		Modulate = new Color(255, 255, 255, 0);
		Tween tweenA = CreateTween().SetParallel();
		tweenA.TweenProperty(this, "modulate", new Color(255, 255, 255, 1), 0.2);
		tweenA.Finished += () => {
			tweenA.Stop();
			Tween tweenB = CreateTween().SetParallel();
			tweenB.TweenProperty(this, "modulate", new Color(255, 255, 255, 0), 0.2);
			tweenB.TweenProperty(this, "position", new Vector2(Position.X, Position.Y-10), 0.2);
		};
	}
}
