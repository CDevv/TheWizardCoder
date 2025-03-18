using Godot;
using Godot.Collections;
using System.Collections.Generic;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Interactables;

namespace TheWizardCoder.Rooms
{
    public partial class Vindi7 : BaseRoom
    {
        private Vector2I[] houseTilePositions;
        private Vector2I houseAtlas;
        private CollisionShape2D questWall;
        private Warper theodoreHouseWarper;
        private DialoguePoint theodoreHouseDialogue;
        private Array<Sprite2D> barriers;

        public override void OnReady()
        {
            base.OnReady();

            questWall = GetNode<CollisionShape2D>("%QuestWall");
            theodoreHouseWarper = GetNode<Warper>("TheodoreWarper");
            theodoreHouseDialogue = GetNode<DialoguePoint>("TheodoreHouseDialogue");

            houseTilePositions = new Vector2I[6];
            houseTilePositions[0] = new Vector2I(9, 25);
            houseTilePositions[1] = new Vector2I(24, 14);
            houseTilePositions[2] = new Vector2I(33, 16);
            houseTilePositions[3] = new Vector2I(33, 26);
            houseTilePositions[4] = new Vector2I(43, 16);
            houseTilePositions[5] = new Vector2I(43, 26);

            houseAtlas = new Vector2I(6, 0);

            barriers = new Array<Sprite2D>();
            Array<Node> barriersNodes = GetTree().GetNodesInGroup("barrier");
            foreach (Node item in barriersNodes)
            {
                Sprite2D barrierSprite = (Sprite2D)item;
                barriers.Add(barrierSprite);
            }

            if (global.PlayerData.VindiHousesPuzzle)
            {
                FixHouses();
                AnimationPlayer.Play("hide_glitch");
                theodoreHouseWarper.Active = true;
                theodoreHouseDialogue.Active = false;

                RemoveQuestWallCollision();
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
                    theodoreHouseWarper.Active = true;
                    theodoreHouseDialogue.Active = false;
                    RemoveQuestWallCollision();

                    global.PlayerData.Stats.AddLevelPoints(8);
                }
            }
        }

        private void HideBarriers()
        {
            foreach (Sprite2D item in barriers)
            {
                item.Hide();
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

        private void RemoveQuestWallCollision()
        {
            if (global.PlayerData.FishingRodSolved &&
                global.PlayerData.VindiTreeSolved &&
                global.PlayerData.HasFinishedCraigQuest &&
                global.PlayerData.VindiHousesPuzzle)
            {
                global.PlayerData.CompletedAllVindiQuests = true;
            }

            if (global.PlayerData.CompletedAllVindiQuests)
            {
                questWall.Disabled = true;
                HideBarriers();
            }
        }
    }
}