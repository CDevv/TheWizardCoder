using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class RaftWater : BaseRoom
{
    private const float RaftSpeed = 4;

	[Export]
	public Resource DialogueResource { get; set; }

    private Sprite2D raft;
    private BoxStack boxStack;
    private double passedTime = 0;
    private bool canMoveRaft;

    public override async void OnReady()
    {
        base.OnReady();
        raft = GetNode<Sprite2D>("Raft");
        boxStack = GetNode<BoxStack>("%BoxStack");

		await PlayCutscene("water_1");
        if (global.CurrentRoom.Player.Follower != null)
        {
            global.CurrentRoom.Player.Follower.DisableFollowing();
        }
        
        global.CurrentRoom.Player.CameraEnabled = false;
        canMoveRaft = true;
        global.CanWalk = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (canMoveRaft)
        {
            Vector2 velocity = Input.GetVector("left", "right", "up", "down").Normalized();
            velocity *= RaftSpeed;
            
            global.CurrentRoom.Player.Position += velocity;
            global.CurrentRoom.Gertrude.Position += velocity;
            raft.Position += velocity;
        }
    }

    public override void _Process(double delta)
    {
        passedTime += delta;
    }

    private void OnDialogueEnded(string initialTitle, string message)
    {
        AnimationPlayer.Play("water_final");
        GD.Print(passedTime);
        GD.Print(global.CurrentRoom.Player.Position);
        GD.Print(global.CurrentRoom.Gertrude.Position);

        boxStack.AddBox();
    }
}
