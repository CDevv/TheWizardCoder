using Godot;
using System;
using System.Collections.Generic;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Interactables;
using TheWizardCoder.Enums;
using TheWizardCoder.Data;

public partial class RaftWater : BaseRoom
{
    private const float RaftSpeed = 4;
    private const int MaxScreenY = 480;

	[Export]
	public Resource DialogueResource { get; set; }
    [Export]
    public PackedScene TextBoxScene { get; set; }

    private List<RaftWaterChallenge> challenges = new();

    private CharacterBody2D raft;
    private BoxStack boxStack;
    private Area2D interactableScanner;
    private CanvasLayer challengeDisplay;
    private ConsoleBoxText boxCountText;
    private ConsoleBoxText conditionText;
    private List<Vector2> spawnPositions;
    private List<TextBoxInteractable> challengeBoxes = new();
    private string[] boxTitles = new string[4];
    private Action[] boxTypeFuncs = new Action[4];
    private double passedTime = 0;
    private bool canMoveRaft;
    private bool cutsceneSkippable;
    private int currentChallenge = 0;

    public override async void OnReady()
    {
        base.OnReady();
        raft = GetNode<CharacterBody2D>("Raft");
        boxStack = GetNode<BoxStack>("%BoxStack");
        interactableScanner = GetNode<Area2D>("%InteractableScanner");
        challengeDisplay = GetNode<CanvasLayer>("ChallengeDisplay");
        boxCountText = GetNode<ConsoleBoxText>("%BoxCountText");
        conditionText = GetNode<ConsoleBoxText>("%ChallengeConditionText");

        spawnPositions = new List<Vector2>() 
        {
            GetNode<Marker2D>("TextBoxBase1").Position,
            GetNode<Marker2D>("TextBoxBase2").Position,
            GetNode<Marker2D>("TextBoxBase3").Position,
            GetNode<Marker2D>("TextBoxBase4").Position
        };

        boxTypeFuncs[(int)ChallengeTextBoxType.AddBox] = () => { boxStack.AddBox(); };
        boxTypeFuncs[(int)ChallengeTextBoxType.RemoveBox] = () => { boxStack.RemoveBox(); };
        boxTypeFuncs[(int)ChallengeTextBoxType.ClearAllBoxes] = () => { boxStack.ClearBoxes(); };

        boxTitles[(int)ChallengeTextBoxType.AddBox] = "AddBox();";
        boxTitles[(int)ChallengeTextBoxType.RemoveBox] = "RemoveBox();";
        boxTitles[(int)ChallengeTextBoxType.ClearAllBoxes] = "ClearBoxes();";

        challenges.Add(new(() => boxStack.Count == 1, "Count == 1", new ChallengeTextBoxType[4]{
            ChallengeTextBoxType.AddBox, ChallengeTextBoxType.RemoveBox, ChallengeTextBoxType.RemoveBox, ChallengeTextBoxType.RemoveBox
        }));
        challenges.Add(new(() => boxStack.Count == 2, "Count == 2", new ChallengeTextBoxType[4]{
            ChallengeTextBoxType.AddBox, ChallengeTextBoxType.RemoveBox, ChallengeTextBoxType.RemoveBox, ChallengeTextBoxType.RemoveBox
        }));

		await PlayCutscene("water_1");
        if (global.CurrentRoom.Player.Follower != null)
        {
            global.CurrentRoom.Player.Follower.DisableFollowing();
        }
        
        global.CurrentRoom.Player.CameraEnabled = false;
        canMoveRaft = true;
        global.CanWalk = false;
        global.GameDisplayEnabled = false;
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

            var collision = raft.MoveAndCollide(velocity);

            if (collision == null)
            {
                global.CurrentRoom.Player.Position += velocity;
                global.CurrentRoom.Gertrude.Position += velocity;
            }        
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (cutsceneSkippable)
            {
                StartSequence();
                cutsceneSkippable = false;
            }
        }
    }

    private void StartSequence()
    {
        AnimationPlayer.Stop();
        AnimationPlayer.Play("water_final");

        global.CanWalk = false;
        global.GameDisplayEnabled = false;
        challengeDisplay.Show();

        SpawnMany();
    }

    private void OnInteractableEntered(Area2D area)
    {
        GD.Print("area entered");
        Interactable interactable = (Interactable)area;
        
        interactable.Action();
    }

    private void SpawnTextBox(int position, ChallengeTextBoxType boxType)
    {
        float boxY = GD.Randi() % MaxScreenY;
        Vector2 boxPosition = spawnPositions[position];

        TextBoxInteractable textBox = TextBoxScene.Instantiate<TextBoxInteractable>();
        textBox.Text = boxTitles[(int)boxType];
        textBox.Position = boxPosition;
        textBox.Touched += () => 
        {
            try
            {
                boxTypeFuncs[(int)boxType]();
            }
            catch (InvalidOperationException)
            {
                challengeDisplay.Hide();
                global.CurrentRoom.GameOverDisplay.ShowDisplay();
            }

            if (challenges[currentChallenge-1].Condition())
            {
                SpawnMany();
            }
            else
            {
                challengeDisplay.Hide();
                global.CurrentRoom.GameOverDisplay.ShowDisplay();
            }
        };

        CallDeferred(TextBoxInteractable.MethodName.AddChild, textBox);
        challengeBoxes.Add(textBox);
    }

    private void ClearTextBoxes()
    {
        foreach (var item in challengeBoxes)
        {
            item.QueueFree();
        }
        challengeBoxes.Clear();
    }  

    private void SpawnMany()
    {
        boxCountText.SetText($"Count = {boxStack.Count}");
        conditionText.SetText($"if ({challenges[currentChallenge].ConditionString})");
        
        ClearTextBoxes();
        for (var i = 0; i < 4; i++)
        {
            ChallengeTextBoxType chosenType = challenges[currentChallenge].BoxTypes[i];
            SpawnTextBox(i, chosenType);
        }
        currentChallenge++;
    }

    private void OnDialogueEnded(string initialTitle, string message)
    {
        StartSequence();
        
        GD.Print(passedTime);
        GD.Print(global.CurrentRoom.Player.Position);
        GD.Print(global.CurrentRoom.Gertrude.Position);
    }

    private void SetCutsceneSkippable(bool skippable)
    {
        this.cutsceneSkippable = skippable;
    }
}
