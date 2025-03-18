using Godot;
using Godot.Collections;
using System.Collections.Generic;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Interactables;

namespace TheWizardCoder.Rooms
{
    public partial class VindiHotelFloor2Room2 : BaseRoom
    {
        private const int TileSetXOffset = 5;
        private const int TileSetYOffset = 7;

        private CodeProblemPoint problemPoint;
        private Vector2 cellBasePosition;
        private Sprite2D whiteCellBorder;
        private Sprite2D[,] cells;
        private Sprite2D selectCell;

        public override void OnReady()
        {
            base.OnReady();

            problemPoint = GetNode<CodeProblemPoint>("CodeProblemPoint");
            cellBasePosition = new Vector2(176, 240);
            cells = new Sprite2D[10, 5];
            whiteCellBorder = GetNode<Sprite2D>("WhiteCellBorder");
            selectCell = GetNode<Sprite2D>("SelectCell");

            InitializeCellBorders();

            if (global.PlayerData.SolvedHotelRoom)
            {
                AnimationPlayer.Play("hide_glitch");

                Array<Node> glitches = GetTree().GetNodesInGroup("glitches");
                foreach (Node node in glitches)
                {
                    ((Sprite2D)node).Hide();
                }
            }
        }

        private void InitializeCellBorders()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Sprite2D cell = (Sprite2D)whiteCellBorder.Duplicate();

                    AddChild(cell);
                    cell.Position = cellBasePosition + new Vector2(i * 32, j * 32);
                    cells[i, j] = cell;
                }
            }
        }

        private void ToggleCellVisiblity(bool visiblity)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    cells[i, j].Visible = visiblity;
                }
            }
        }

        private async void OnSubmitted(bool solved)
        {
            List<CodeProblemItem> items = new(CodeProblemPanel.LastProblemItems);

            if (items.Count == 2)
            {
                problemPoint.Active = false;
                ToggleCellVisiblity(true);

                int chosenColumns = 0;
                int chosenRows = 0;

                if (items[0].CurrentAnswer == "i < 10;")
                {
                    chosenColumns = 10;
                }
                else if (items[0].CurrentAnswer == "i < 1;")
                {
                    chosenColumns = 1;
                }

                if (items[1].CurrentAnswer == "j < 5;")
                {
                    chosenRows = 5;
                }
                else if (items[1].CurrentAnswer == "j < 2;")
                {
                    chosenRows = 2;
                }

                if (chosenColumns != 0 && chosenRows != 0)
                {
                    selectCell.Position = cellBasePosition;
                    selectCell.Show();

                    for (int row = 0; row < chosenRows; row++)
                    {
                        for (int column = 0; column < chosenColumns; column++)
                        {
                            Vector2 newPosition = cellBasePosition + new Vector2(column * 32, row * 32);

                            Tween tween = GetTree().CreateTween();
                            tween.TweenProperty(selectCell, "position", newPosition, 0.5);
                            await ToSignal(tween, Tween.SignalName.Finished);

                            string glitchName = $"Glitch[{TileSetXOffset + column},{TileSetYOffset + row}]";

                            Sprite2D glitch = GetNodeOrNull<Sprite2D>(glitchName);

                            if (glitch != null)
                            {
                                glitch.Hide();
                            }

                            SceneTreeTimer timer = GetTree().CreateTimer(0.5);
                            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
                        }
                    }

                    selectCell.Hide();
                    ToggleCellVisiblity(false);

                    if (!solved)
                    {
                        Array<Node> glitches = GetTree().GetNodesInGroup("glitches");
                        foreach (Node node in glitches)
                        {
                            ((Sprite2D)node).Show();
                        }

                        problemPoint.Active = true;
                    }
                    else
                    {
                        AnimationPlayer.Play("code_solved");
                        global.PlayerData.Stats.AddLevelPoints(8);
                    }
                }
            }
        }
    }
}
