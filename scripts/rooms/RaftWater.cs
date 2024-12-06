using Godot;
using System;
using System.Collections.Generic;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Interactables;
using TheWizardCoder.Enums;
using TheWizardCoder.Data;
using TheWizardCoder.Utils;
using System.Threading.Tasks;

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
    private RaftSunkDisplay raftSunkDisplay;
    private ConsoleBoxText boxCountText;
    private ConsoleBoxText conditionText;
    private List<Vector2> spawnPositions;
    private List<TextBoxInteractable> challengeBoxes = new();
    private string[] boxTitles = new string[Enum.GetValues(typeof(ChallengeTextBoxType)).Length];
    private Action[] boxTypeFuncs = new Action[Enum.GetValues(typeof(ChallengeTextBoxType)).Length];
    private double passedTime = 0;
    private bool canMoveRaft;
    private bool cutsceneSkippable;
    private int currentChallenge = 0;
    private bool passedChallenge;

    public override async void OnReady()
    {
        base.OnReady();
        raft = GetNode<CharacterBody2D>("Raft");
        boxStack = GetNode<BoxStack>("%BoxStack");
        interactableScanner = GetNode<Area2D>("%InteractableScanner");
        challengeDisplay = GetNode<CanvasLayer>("ChallengeDisplay");
        raftSunkDisplay = GetNode<RaftSunkDisplay>("RaftSunkDisplay");
        boxCountText = GetNode<ConsoleBoxText>("%BoxCountText");
        conditionText = GetNode<ConsoleBoxText>("%ChallengeConditionText");

        spawnPositions = new List<Vector2>() 
        {
            GetNode<Marker2D>("TextBoxBase1").Position,
            GetNode<Marker2D>("TextBoxBase2").Position,
            GetNode<Marker2D>("TextBoxBase3").Position,
            GetNode<Marker2D>("TextBoxBase4").Position
        };

        InitBoxTypes();
        InitChallenges();

        if (!global.PlayerData.PassedWaterChallenges)
        {
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
        else
        {
            AnimationPlayer.Play("water_final_2");
        }
    }

    private void InitBoxTypes()
    {
        boxTypeFuncs[(int)ChallengeTextBoxType.AddBox] = () => { boxStack.AddBox(); };
        boxTypeFuncs[(int)ChallengeTextBoxType.RemoveBox] = () => { boxStack.RemoveBox(); };
        boxTypeFuncs[(int)ChallengeTextBoxType.ClearAllBoxes] = () => { boxStack.ClearBoxes(); };
        boxTypeFuncs[(int)ChallengeTextBoxType.AddTwoBoxes] = () => { boxStack.AddBox(); boxStack.AddBox(); };
        boxTypeFuncs[(int)ChallengeTextBoxType.AddFiveBoxes] = () => 
        {
            for (var i = 0; i < 5; i++)
            {
                boxStack.AddBox();
            }
        };
        boxTypeFuncs[(int)ChallengeTextBoxType.AddNoBoxes] = () => {};
        boxTypeFuncs[(int)ChallengeTextBoxType.RemoveTwoBoxes] = () => { boxStack.RemoveBox(); boxStack.RemoveBox(); };
        boxTypeFuncs[(int)ChallengeTextBoxType.AddSevenBoxes] = () => 
        {
            for (var i = 0; i < 7; i++)
            {
                boxStack.AddBox();
            }
        };

        boxTitles[(int)ChallengeTextBoxType.AddBox] = "AddBox();";
        boxTitles[(int)ChallengeTextBoxType.RemoveBox] = "RemoveBox();";
        boxTitles[(int)ChallengeTextBoxType.ClearAllBoxes] = "ClearBoxes();";
        boxTitles[(int)ChallengeTextBoxType.AddTwoBoxes] = "AddBoxes(2);";
        boxTitles[(int)ChallengeTextBoxType.AddFiveBoxes] = "AddBoxes(5);";
        boxTitles[(int)ChallengeTextBoxType.AddNoBoxes] = "AddBoxes(0);";
        boxTitles[(int)ChallengeTextBoxType.RemoveTwoBoxes] = "RemoveBoxes(2);";
        boxTitles[(int)ChallengeTextBoxType.AddSevenBoxes] = "AddBoxes(7);";
    }

    private void InitChallenges()
    {
        var data = (Godot.Collections.Dictionary<string, Variant>)DataLoader.GetJsonData("res://info/raft_water_challenges.json");

        foreach (var item in data)
        {
            var challengeData = (Godot.Collections.Dictionary<string, Variant>)item.Value;
            RaftWaterChallenge challenge = new(challengeData);
            challenges.Add(challenge);
        }
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
                //AnimationPlayer.Play("water_final");
                //OnChallengesCleared();
                //canMoveRaft = false;
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
        Interactable interactable = (Interactable)area;
        
        interactable.Action();
    }

    private void SpawnTextBox(int position, ChallengeTextBoxType boxType)
    {
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
                OnFailure();
                return;
            }

            if (challenges[currentChallenge-1].Condition(boxStack))
            {
                SpawnMany();
            }
            else
            {
                OnFailure();
                return;
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
        if (currentChallenge >= challenges.Count)
        {
            OnChallengesCleared();
            return;
        }

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

    private void OnFailure()
    {
        currentChallenge = 0;
        challengeDisplay.Hide();
        ClearTextBoxes();
        boxStack.ClearBoxes();
        raftSunkDisplay.ShowDisplay();
    }

    private async void OnChallengesCleared()
    {
        GD.Print("Challenges cleared!");
        ClearTextBoxes();
        challengeDisplay.Hide();
        passedChallenge = true;
        canMoveRaft = false;

        await TweenRaft(new Vector2(3932, 1144), 1f);
        await PlayCutscene("water_2");
        await TweenCameraToPlayer(1f);

        global.CanWalk = true;
        global.GameDisplayEnabled = true;
        global.CurrentRoom.Player.CameraEnabled = true;

        if (Player.Follower != null)
        {
            Player.Follower.EnableFollowing();
        }
    }

    private void OnDialogueEnded(string initialTitle, string message)
    {
        if (!passedChallenge)
        {
            StartSequence();
        }
        
        GD.Print(passedTime);
        GD.Print(global.CurrentRoom.Player.Position);
        GD.Print(global.CurrentRoom.Gertrude.Position);
        passedTime = 0;
    }

    private void SetCutsceneSkippable(bool skippable)
    {
        this.cutsceneSkippable = skippable;
    }

    private async Task TweenRaft(Vector2 position, float duration)
    {
        Tween tween = GetTree().CreateTween();
        tween.SetParallel();

        tween.TweenProperty(raft, "position", position, duration);
        tween.TweenProperty(global.CurrentRoom.Player, "position", position + new Vector2(-16, 0), duration);
        tween.TweenProperty(global.CurrentRoom.Gertrude, "position", position + new Vector2(16, 0), duration);

        tween.Play();
        await ToSignal(tween, Tween.SignalName.Finished);
    }
}
