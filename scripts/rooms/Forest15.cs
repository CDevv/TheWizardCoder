using Godot;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;
using TheWizardCoder.Interactables;

namespace TheWizardCoder.Rooms
{
    public partial class Forest15 : ForestRoom
    {
        [Export]
        public Resource DialogueResource { get; set; }

        private const int ButtonsCount = 5;

        private bool[] bools = { false, false, false, false, false };
        private SwitchBlock[] buttons = new SwitchBlock[ButtonsCount];
        private CodeMessagePoint codeMessage;

        private TileMap tileMap;

        public override void OnReady()
        {
            base.OnReady();

            tileMap = GetNode<TileMap>("TileMap");

            codeMessage = GetNode<CodeMessagePoint>("CodeMessagePoint");
            for (int i = 0; i < ButtonsCount; i++)
            {
                int currentIndex = i;
                SwitchBlock interactable = GetNode<SwitchBlock>($"Button{i + 1}");
                buttons[i] = interactable;
                buttons[i].Pushed += () => OnButtonPushed(currentIndex);
            }
        }

        private void OnButtonPushed(int i)
        {
            bools[i] = !bools[i];
            codeMessage.CodeText = BuildCodeString();
        }

        private string BuildCodeString()
        {
            StringBuilder builder = new();
            builder.AppendLine("int c = 5;");
            builder.AppendLine("bool[] arr =");
            builder.Append("{ ");
            builder.Append(string.Join(", ", bools.Select(x => x.ToString()).Select(x => x.ToLower())));
            builder.AppendLine(" };");
            builder.AppendLine("for (int i = 0; i < c; i++)");
            builder.AppendLine("	if (IsEven(i) == arr[i])");
            builder.AppendLine("		RemoveTrees();");

            return builder.ToString();
        }

        private async void CheckCode()
        {
            GD.Print("execute");
            int count = 0;
            for (int i = 0; i < ButtonsCount; i++)
            {
                if ((i % 2 == 0) == buttons[i].Value)
                {
                    count++;
                }
            }

            if (count == ButtonsCount)
            {
                await OnCodeSolved();
            }
        }

        private async Task OnCodeSolved()
        {
            GD.Print("Passed!");
            ClearTrees();

            await PuzzleSolvedCutscene();

            global.CanWalk = true;

            global.PlayerData.Stats.AddLevelPoints(6);
        }

        private void ClearTrees()
        {
            for (int i = 8; i <= 17; i++)
            {
                tileMap.SetCell(0, new Vector2I(56, i));
                tileMap.SetCell(0, new Vector2I(57, i));
            }
        }

        private async void PuzzleIntroCutscene()
        {
            if (global.CurrentRoom.Player.Follower != null)
            {
                global.CurrentRoom.Player.Follower.DisableFollowing();

                await PlayCutscene("puzzle_intro_1");
                await ShowDialogue(DialogueResource, "puzzle_intro_1");
                await PlayCutscene("puzzle_intro_2");
                await ShowDialogue(DialogueResource, "puzzle_intro_2");
            }
        }

        private async Task PuzzleSolvedCutscene()
        {
            Player.Freeze();

            await TweenCamera(new Vector2(1840, 408), 0.9f);

            if (Player.Follower != null)
            {
                await PlayCutscene("puzzle_solved");
                await ShowDialogue(DialogueResource, "puzzle_solved");
                global.CanWalk = false;
            }

            await TweenCameraToPlayer(0.9f);

            if (Player.Follower != null)
            {
                global.CurrentRoom.Gertrude.PlayIdleAnimation(Direction.Left);
                SceneTreeTimer timer = GetTree().CreateTimer(1);
                await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

                global.CurrentRoom.Gertrude.PlayAnimation("left");

                Tween tween = GetTree().CreateTween();
                tween.TweenProperty(global.CurrentRoom.Gertrude, "position", new Vector2(global.CurrentRoom.Player.Position.X, 408), 1.5f);
                tween.Play();
                await ToSignal(tween, Tween.SignalName.Finished);

                global.CurrentRoom.Gertrude.PlayIdleAnimation(Direction.Up);

                global.CurrentRoom.Gertrude.EnableFollowing();
                global.CurrentRoom.Gertrude.AddPathwayPoint();
            }

            global.CurrentRoom.Player.DistanceWalked = 0;

            Player.Unfreeze();
        }
    }
}
