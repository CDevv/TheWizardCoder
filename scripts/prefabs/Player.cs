using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const int Speed = 2;

	private AnimationTree animationTree;

    public override void _Ready()	
    {
        animationTree = GetNode<AnimationTree>("AnimationTree");
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
}
