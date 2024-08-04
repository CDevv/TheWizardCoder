using DialogueManagerRuntime;
using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
	[Signal]
	public delegate void AnimationFinishedEventHandler();
	public Direction Direction { get; private set; }
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

    public override async void _PhysicsProcess(double delta)
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
			Direction = global.GetDirectionFromVector(direction);
		}

		if (global.CurrentRoom.Camera != null)
		{
			global.CurrentRoom.Camera.Set(Camera2D.PropertyName.Position, Position + velocity);
		}

		if (global.PlayerIsOnStairs && velocity.X != 0)
		{
			float addedY = (float)(100 * delta);
			if (velocity.X > 0)
			{
				if (global.StairsGoUp)
				{
					velocity.Y += global.StairsInverted ? -addedY : addedY;
				}
			}
			else
			{
				velocity.Y -= global.StairsInverted ? -addedY : addedY;
			}
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
			else if (collision.GetCollider().GetType() == typeof(BattlePoint))
			{
				global.CanWalk = false;
				global.GameDisplayEnabled = false;
				GD.Print("Battle point");
				await global.CurrentRoom.BattleDisplay.ShowDisplay();
			}
		}	
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_accept"))
		{
			if (global.IsInCutscene)
			{
				return;
			}

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
		Vector2 resultVector = global.GetDirectionVector(direction);

		animationTree.Set("parameters/Idle/blend_position", resultVector);
		animationTree.Set("parameters/Move/blend_position", resultVector);
	}

	public void PlaySideAnimation(string name)
	{
		animationTree.Set("parameters/conditions/idle", false);
		animationTree.Set("parameters/conditions/move", false);
		animationTree.Set("parameters/conditions/extrastate", true);
		animationTree.Set("parameters/ExtraStates/state/transition_request", name);

		Task finishedAnimation = new Task(async () => {
			await ToSignal(animationTree, AnimationTree.SignalName.AnimationFinished);
			EmitSignal(SignalName.AnimationFinished);
		});
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

	public void ShowDialogueBallon(Resource resource, string title)
	{
		DialogueManager.ShowDialogueBalloon(resource, title);
	}
}
