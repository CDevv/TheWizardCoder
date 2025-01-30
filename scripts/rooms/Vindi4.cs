using Godot;
using System;
using System.Collections.Generic;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;

namespace TheWizardCoder.Rooms
{
    public partial class Vindi4 : BaseRoom
    {
        private Sprite2D selectionBox;

        public override void OnReady()
        {
            base.OnReady();
            selectionBox = GetNode<Sprite2D>("SelectionBox");

            if (!global.PlayerData.VindiTreeSolved)
            {
                AnimationPlayer.Play("tree_init");
            }
            else
            {
                AnimationPlayer.Play("tree_solved");
            }
        }

        private void OnProblemSolved()
        {
            AnimationPlayer.Play("tree_solved_anim");
        }

        private async void OnSubmitted(bool solved)
        {
            Player.Freeze();

            List<CodeProblemItem> items = new(CodeProblemPanel.LastProblemItems);

            foreach (var item in items)
            {
                GD.Print(item.CurrentAnswer);
            }

            if (items.Count == 3)
            {
                Vector2 basePosition = selectionBox.Position;
                Vector2 baseVelocity = new Vector2(0, 32);
                int multiplier = 0;
                int num = 0;

                if (items[2].CurrentAnswer == "n--;")
                {
                    multiplier = 1;
                }
                else if (items[2].CurrentAnswer == "n++;")
                {
                    multiplier = -1;
                }

                if (items[0].CurrentAnswer == "7;")
                {
                    num = 7;
                }
                else if (items[0].CurrentAnswer == "4;")
                {
                    num = 4;
                }

                if (num != 0)
                {
                    selectionBox.Show();
                    int n = 1;

                    // The fake program
                    while (num != 0 && num != 10)
                    {
                        GD.Print(num);

                        Vector2 velocity = basePosition + ((n * multiplier) * baseVelocity);

                        Tween tween = GetTree().CreateTween();
                        tween.TweenProperty(selectionBox, "position", velocity, 1);
                        tween.Play();

                        await ToSignal(tween, Tween.SignalName.Finished);

                        string nodeName = $"%Trunk{num}";
                        Sprite2D trunk = GetNodeOrNull<Sprite2D>(nodeName);                      

                        if (trunk != null)
                        {
                            trunk.Hide();
                        }

                        num += -multiplier;
                        n++;

                        //Sleep
                        SceneTreeTimer timer = GetTree().CreateTimer(0.5);
                        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
                    }

                    selectionBox.Hide();
                    selectionBox.Position = basePosition;

                    Player.Unfreeze();

                    //If all the answers are correct
                    if (solved)
                    {
                        OnProblemSolved();
                    }
                    else
                    {
                        ResetTrunks();
                    }
                }
            }
        }

        private void ResetTrunks()
        {
            for (int i = 1; i <= 4; i++)
            {
                string nodeName = $"%Trunk{i}";
                Sprite2D trunk = GetNode<Sprite2D>(nodeName);
                trunk.Show();
            }
        }
    }
}
