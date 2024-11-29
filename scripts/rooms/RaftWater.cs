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
    private bool cutsceneSkippable;

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

    public override void _Process(double delta)
    {
        passedTime += delta;
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

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (cutsceneSkippable)
            {
                global.CanWalk = false;
                canMoveRaft = true;

                AnimationPlayer.Play("water_final");
                boxStack.AddBox();
                SpawnTextBox();

                cutsceneSkippable = false;
            }
        }
    }

    private void OnInteractableEntered(Area2D area)
    {
        GD.Print("area entered");
        Interactable interactable = (Interactable)area;
        
        interactable.Action();
    }

    private void SpawnTextBox()
    {
        float boxY = GD.Randi() % MaxScreenY;
        Vector2 boxPosition = new Vector2(textBoxBasePos.Position.X, textBoxBasePos.Position.Y + boxY);

        TextBoxInteractable textBox = TextBoxScene.Instantiate<TextBoxInteractable>();
        textBox.Text = "Hello.";
        textBox.Position = boxPosition;
        textBox.Touched += () => boxStack.AddBox();

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

    private void SetCutsceneSkippable(bool skippable)
    {
        this.cutsceneSkippable = skippable;
    }
}
