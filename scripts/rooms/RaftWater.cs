using Godot;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Interactables;

public partial class RaftWater : BaseRoom
{
    private const float RaftSpeed = 4;
    private const int MaxScreenY = 480;

	[Export]
	public Resource DialogueResource { get; set; }
    [Export]
    public PackedScene TextBoxScene { get; set; }

    private Sprite2D raft;
    private BoxStack boxStack;
    private Marker2D textBoxBasePos;
    private Area2D interactableScanner;
    private double passedTime = 0;
    private bool canMoveRaft;

    public override async void OnReady()
    {
        base.OnReady();
        raft = GetNode<Sprite2D>("Raft");
        boxStack = GetNode<BoxStack>("%BoxStack");
        textBoxBasePos = GetNode<Marker2D>("TextBoxBase");
        interactableScanner = GetNode<Area2D>("%InteractableScanner");

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

    private void OnInteractableEntered(Node2D node)
    {
        if (node.GetType() == typeof(Interactable))
        {
            ((Interactable)node).Action();
        }
    }

    public override void _Process(double delta)
    {
        passedTime += delta;
    }

    private void SpawnTextBox()
    {
        float boxY = GD.Randi() % MaxScreenY;
        Vector2 boxPosition = new Vector2(textBoxBasePos.Position.X, textBoxBasePos.Position.Y + boxY);

        TextBoxInteractable textBox = TextBoxScene.Instantiate<TextBoxInteractable>();
        textBox.Position = boxPosition;
        AddChild(textBox);
    }

    private void OnDialogueEnded(string initialTitle, string message)
    {
        global.CanWalk = false;

        AnimationPlayer.Play("water_final");
        GD.Print(passedTime);
        GD.Print(global.CurrentRoom.Player.Position);
        GD.Print(global.CurrentRoom.Gertrude.Position);

        boxStack.AddBox();
        SpawnTextBox();
    }
}
