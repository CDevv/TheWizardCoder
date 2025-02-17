using Godot;
using System;
using System.Collections.Generic;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;

public partial class Vindi7 : BaseRoom
{
    private Vector2I[] houseTilePositions;
    private Vector2I houseAtlas;

    public override void OnReady()
    {
        base.OnReady();

        houseTilePositions = new Vector2I[6];
        houseTilePositions[0] = new Vector2I(9, 25);
        houseTilePositions[1] = new Vector2I(24, 14);
        houseTilePositions[2] = new Vector2I(33, 16);
        houseTilePositions[3] = new Vector2I(33, 26);
        houseTilePositions[4] = new Vector2I(43, 16);
        houseTilePositions[5] = new Vector2I(43, 26);

        houseAtlas = new Vector2I(6, 0);

        if (global.PlayerData.VindiHousesPuzzle)
        {
            FixHouses();
            AnimationPlayer.Play("hide_glitch");
        }
    }

    public void OnSubmitted(bool solved)
	{
        List<CodeProblemItem> problemItems = new(CodeProblemPanel.LastProblemItems);
        int housesCount = 6;

        ResetHouses();

        if (problemItems.Count == 2)
        {
            int i = -1;

            if (problemItems[0].CurrentAnswer == "1;")
            {
                i = 1;
            }
            else if (problemItems[0].CurrentAnswer == "0;")
            {
                i = 0;
            }

            while (i != housesCount)
            {
                if (problemItems[1].CurrentAnswer == "CorruptHouse(i);")
                {
                    GetNode<Sprite2D>($"HouseGlitch{i}").Show();
                }
                else if (problemItems[1].CurrentAnswer == "FixHouse(i);")
                {
                    GetNode<Sprite2D>($"HouseGlitch{i}").Hide();
                }
                else if (problemItems[1].CurrentAnswer == "RemoveHouse(i);")
                {
                    GetNode<Sprite2D>($"HouseGlitch{i}").Hide();
                    TileMap.SetCell(0, houseTilePositions[i], -1, null, 0);
                }
                i++;
            }

            if (solved)
            {
                AnimationPlayer.Play("puzzle_solved");
            }
            else
            {
                //ResetHouses();
            }
        }
    }

    private void ResetHouses()
    {
        for (int i = 0; i < 6; i++)
        {
            GetNode<Sprite2D>($"HouseGlitch{i}").Show();
            TileMap.SetCell(0, houseTilePositions[i], 0, houseAtlas);
        }
    }

    private void FixHouses()
    {
        for (int i = 0; i < 6; i++)
        {
            GetNode<Sprite2D>($"HouseGlitch{i}").Hide();
            TileMap.SetCell(0, houseTilePositions[i], 0, houseAtlas);
        }
    }
}
