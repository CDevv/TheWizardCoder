using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const int Speed = 2;

	private Global global;

	private AnimationPlayer animationPlayer;
	private AnimationTree animationTree;
	private Area2D interactableFinder;

    public override void _Ready()	
    {
		global = GetNode<Global>("/root/Global");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationTree = GetNode<AnimationTree>("AnimationTree");
		interactableFinder = GetNode<Area2D>("InteractableFinder");
    }

    public override void _PhysicsProcess(double delta)
	{
		if (!global.CanWalk)
		{
			return;
		}
		animationTree.Set("parameters/conditions/extrastate", false);

		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down").Normalized();
		Vector2 velocity = direction * Speed;

		if (velocity == Vector2.Zero)
		{
			animationTree.Set("parameters/conditions/idle", true);
			animationTree.Set("parameters/conditions/move", false);
		}
		else
		{
			animationTree.Set("parameters/conditions/move", true);
			animationTree.Set("parameters/conditions/idle", false);
		}

		if (direction != Vector2.Zero)
		{
			animationTree.Set("parameters/Idle/blend_position", direction);
			animationTree.Set("parameters/Move/blend_position", direction);
		}

		var collision = MoveAndCollide(velocity);
		if (collision != null)
		{
			if (collision.GetCollider().GetType() == typeof(Warper))
			{
				global.CanWalk = false;
				GD.Print("i collided with a warper");
				TransitionToRoom((Warper)collision.GetCollider());
			}
		}
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_accept"))
		{
			var overlappingAreas = interactableFinder.GetOverlappingAreas();
			if (overlappingAreas.Count > 0 && global.CanWalk)
			{
				global.CanWalk = false;
				(overlappingAreas[0] as Interactable).Action();
			}
		}
    }

	public async void TransitionToRoom(Warper warper)
	{
		global.CurrentRoom.TransitionRect.PlayAnimation();
		await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
		global.ChangeRoom(warper.TargetRoom);
	}

	public void EnableAnimationTree()
	{
		animationTree.Set(AnimationTree.PropertyName.Active, true);
	}

	public void DisableAnimationTree()
	{
		animationTree.Set(AnimationTree.PropertyName.Active, false);
	}

	public void PlayAnimation(string name)
	{
		animationPlayer.Play(name);
	}

	public void PlaySideAnimation(string name)
	{
		animationTree.Set("parameters/conditions/idle", false);
		animationTree.Set("parameters/conditions/move", false);
		animationTree.Set("parameters/conditions/extrastate", true);
		animationTree.Set("parameters/ExtraStates/state/transition_request", name);
	}

	public void PlayIdleAnimation(string directionName)
	{
		animationTree.Set("parameters/conditions/idle", true);
		animationTree.Set("parameters/conditions/move", false);
		animationTree.Set("parameters/conditions/extrastate", false);

		Vector2 direction = Vector2.Zero;
		switch (directionName)
		{
			case "down":
				direction = Vector2.Down;
				break;
			case "up":
				direction = Vector2.Up;
				break;
			case "left":
				direction = Vector2.Left;
				break;
			case "right":
				direction = Vector2.Right;
				break;
		}

		animationTree.Set("parameters/Idle/blend_position", direction);
	}

	public void PlayMoveAnimation(string directionName)
	{
		animationTree.Set("parameters/conditions/idle", false);
		animationTree.Set("parameters/conditions/move", true);
		animationTree.Set("parameters/conditions/extrastate", false);

		Vector2 direction = Vector2.Zero;
		switch (directionName)
		{
			case "down":
				direction = Vector2.Down;
				break;
			case "up":
				direction = Vector2.Up;
				break;
			case "left":
				direction = Vector2.Left;
				break;
			case "right":
				direction = Vector2.Right;
				break;
		}

		animationTree.Set("parameters/Move/blend_position", direction);
	}

	public void TweenToPosition(Vector2 position, double duration)
	{
		var tween = CreateTween();
		tween.TweenProperty(this, "position", position, duration);
	}
}
