using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Components;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;
using TheWizardCoder.Enums;
using TheWizardCoder.Interactables;
using TheWizardCoder.UI;
using TheWizardCoder.Utils;

namespace TheWizardCoder.Rooms
{
    public partial class RaftWater : BaseRoom
    {
        private const float RaftSpeed = 4;

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
        private float horizontalBoxPositionLimit;
        private List<TextBoxInteractable> challengeBoxes = new();
        private string[] boxTitles = new string[Enum.GetValues(typeof(ChallengeTextBoxType)).Length];
        private Action[] boxTypeFuncs = new Action[Enum.GetValues(typeof(ChallengeTextBoxType)).Length];
        private double passedTime = 0;
        private bool canMoveRaft;
        private bool cutsceneSkippable;
        private int currentChallenge = 0;
        private bool passedChallenge;
        private bool playSecondCutscene;
        private bool isSkippingSecondCutscene;

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

            challenges = new();
            challengeBoxes = new();

            spawnPositions = new List<Vector2>()
        {
            GetNode<Marker2D>("TextBoxBase1").Position,
            GetNode<Marker2D>("TextBoxBase2").Position,
            GetNode<Marker2D>("TextBoxBase3").Position,
            GetNode<Marker2D>("TextBoxBase4").Position
        };
            horizontalBoxPositionLimit = GetNode<Marker2D>("TextBoxLimit").Position.X;

            InitBoxTypes();
            InitChallenges();

            if (!global.PlayerData.PassedWaterChallenges)
            {
                if (Player.HasFollower)
                {
                    await PlayCutscene("water_1");
                    if (global.CurrentRoom.Player.Follower != null)
                    {
                        global.CurrentRoom.Player.Follower.DisableFollowing();
                    }
                }
                else
                {
                    await PlayCutscene("water_1_nolan");
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
                for (int i = 0; i < 5; i++)
                {
                    boxStack.AddBox();
                }
            };
            boxTypeFuncs[(int)ChallengeTextBoxType.AddNoBoxes] = () => { };
            boxTypeFuncs[(int)ChallengeTextBoxType.RemoveTwoBoxes] = () => { boxStack.RemoveBox(); boxStack.RemoveBox(); };
            boxTypeFuncs[(int)ChallengeTextBoxType.AddSevenBoxes] = () =>
            {
                for (int i = 0; i < 7; i++)
                {
                    boxStack.AddBox();
                }
            };
            boxTypeFuncs[(int)ChallengeTextBoxType.AddFourBoxes] = () =>
            {
                for (int i = 0; i < 4; i++)
                {
                    boxStack.AddBox();
                }
            };
            boxTypeFuncs[(int)ChallengeTextBoxType.AddThreeBoxes] = () =>
            {
                for (int i = 0; i < 3; i++)
                {
                    boxStack.AddBox();
                }
            };
            boxTypeFuncs[(int)ChallengeTextBoxType.AddNineBoxes] = () =>
            {
                for (int i = 0; i < 9; i++)
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
            boxTitles[(int)ChallengeTextBoxType.AddFourBoxes] = "AddBoxes(4);";
            boxTitles[(int)ChallengeTextBoxType.AddThreeBoxes] = "AddBoxes(3);";
            boxTitles[(int)ChallengeTextBoxType.AddNineBoxes] = "AddBoxes(9);";
        }

        private void InitChallenges()
        {
            Godot.Collections.Dictionary<string, Variant> data = (Godot.Collections.Dictionary<string, Variant>)DataLoader.GetJsonData("res://info/raft_water_challenges.json");

            foreach (KeyValuePair<string, Variant> item in data)
            {
                Godot.Collections.Dictionary<string, Variant> challengeData = (Godot.Collections.Dictionary<string, Variant>)item.Value;
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

                KinematicCollision2D collision = raft.MoveAndCollide(velocity);

                if (collision == null)
                {
                    global.CurrentRoom.Player.Position += velocity;

                    if (Player.HasFollower)
                    {
                        global.CurrentRoom.Gertrude.Position += velocity;
                    }
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

                    canMoveRaft = false;
                    cutsceneSkippable = false;
                }
            }
        }

        private void StartSequence()
        {
            AnimationPlayer.Stop();
            if (Player.HasFollower)
            {
                AnimationPlayer.Play("water_final");
            }
            else
            {
                AnimationPlayer.Play("water_final_nolan");
            }

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
            textBox.HorizontalPositionLimit = horizontalBoxPositionLimit;
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

                if (challenges[currentChallenge - 1].Condition(boxStack))
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
            foreach (TextBoxInteractable item in challengeBoxes)
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

            boxCountText.Text = $"Count = {boxStack.Count}";
            conditionText.Text = $"if ({challenges[currentChallenge].ConditionString})";

            ClearTextBoxes();
            for (int i = 0; i < 4; i++)
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

        private async void OnChallengesCleared(bool playSecondCutscene = true)
        {
            this.playSecondCutscene = playSecondCutscene;
            global.PlayerData.PassedWaterChallenges = true;

            ClearTextBoxes();
            challengeDisplay.Hide();
            global.CanWalk = false;
            passedChallenge = true;
            canMoveRaft = false;

            if (playSecondCutscene)
            {
                await TweenRaft(new Vector2(3932, 1144), 1f);

                if (Player.HasFollower)
                {
                    AnimationPlayer.Play("water_2", -1, 0.5f);
                }
                else
                {
                    AnimationPlayer.Play("water_2_nolan", -1, 0.5f);
                }
            }
            else
            {
                AnimationPlayer.Seek(14, true, true);
            }

            await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);

            await TweenCameraToPlayer(1f);

            global.CanWalk = true;
            global.GameDisplayEnabled = true;
            global.CurrentRoom.Player.CameraEnabled = true;

            if (Player.Follower != null)
            {
                Player.Follower.EnableFollowing();
                Player.Follower.AddPathwayPoint(Direction.Up, Player.Follower.Position, Player.PlayerSpeed);
            }

            if (passedChallenge)
            {
                global.PlayerData.Stats.AddLevelPoints(6);
            }
        }

        private void OnDialogueEnded(string initialTitle, string message)
        {
            if (!passedChallenge)
            {
                StartSequence();
            }
            else
            {
                if (isSkippingSecondCutscene)
                {
                    OnChallengesCleared(false);
                }
            }

            passedTime = 0;
        }

        private void OnDialogueLineSkipped()
        {
            if (passedChallenge)
            {
                isSkippingSecondCutscene = true;
            }
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

            if (Player.HasFollower)
            {
                tween.TweenProperty(global.CurrentRoom.Player, "position", position + new Vector2(-16, 0), duration);
                tween.TweenProperty(global.CurrentRoom.Gertrude, "position", position + new Vector2(16, 0), duration);
            }
            else
            {
                tween.TweenProperty(global.CurrentRoom.Player, "position", position, duration);
            }

            tween.Play();
            await ToSignal(tween, Tween.SignalName.Finished);
        }
    }

}