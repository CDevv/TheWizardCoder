using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const int Speed = 2;

	private AnimationTree animationTree;
	private Area2D interactableFinder;

    public override void _Ready()	
    {
        animationTree = GetNode<AnimationTree>("AnimationTree");
		interactableFinder = GetNode<Area2D>("InteractableFinder");
    }

    public override void _PhysicsProcess(double delta)
	{
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

		MoveAndCollide(velocity);
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_accept"))
		{
			var overlappingAreas = interactableFinder.GetOverlappingAreas();
			if (overlappingAreas.Count > 0)
			{
				//global.CanWalk = false;
				(overlappingAreas[0] as Interactable).ShowDialogue();
			}
		}
    }
}
