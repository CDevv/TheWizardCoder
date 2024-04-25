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

		Vector2 direction = Input.GetVector("left", "right", "up", "down").Normalized();
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
			ChangeDirection(direction);
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
		if (string.IsNullOrEmpty(warper.TargetLocation))
		{
			global.ChangeRoom(warper.TargetRoomName);
		}
		else
		{
			global.ChangeRoom(warper.TargetRoomName, warper.TargetLocation, warper.PlayerDirection);
		}
	}

	public void ChangeDirection(Vector2 vectorDirection)
	{
		animationTree.Set("parameters/Idle/blend_position", vectorDirection);
		animationTree.Set("parameters/Move/blend_position", vectorDirection);
	}

	public void ChangeDirection(Direction direction)
	{
		Vector2 resultVector = Vector2.Down;
		switch (direction)
		{
			case Direction.Up:
				resultVector = Vector2.Up;
				break;
			case Direction.Down:
				resultVector = Vector2.Down;
				break;
			case Direction.Left:
				resultVector = Vector2.Left;
				break;
			case Direction.Right:
				resultVector = Vector2.Right;
				break;
			default:
				break;
		}

		animationTree.Set("parameters/Idle/blend_position", resultVector);
		animationTree.Set("parameters/Move/blend_position", resultVector);
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

	public void PlayIdleAnimation(Direction direction)
	{
		animationTree.Set("parameters/conditions/idle", true);
		animationTree.Set("parameters/conditions/move", false);
		animationTree.Set("parameters/conditions/extrastate", false);

		ChangeDirection(direction);
	}

	public void PlayMoveAnimation(Direction direction)
	{
		animationTree.Set("parameters/conditions/idle", false);
		animationTree.Set("parameters/conditions/move", true);
		animationTree.Set("parameters/conditions/extrastate", false);

		ChangeDirection(direction);
	}

	public void TweenToPosition(Vector2 position, double duration)
	{
		var tween = CreateTween();
		tween.TweenProperty(this, "position", position, duration);
	}
}
