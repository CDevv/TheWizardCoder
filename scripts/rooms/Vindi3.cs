using Godot;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Interactables;

namespace TheWizardCoder.Rooms
{
    public partial class Vindi3 : BaseRoom
    {
        private CodeProblemPoint problemPoint;

        public override void OnReady()
        {
            base.OnReady();
            problemPoint = GetNode<CodeProblemPoint>("CodeProblemPoint");
        }

        private void OpenFishingRodProblem()
        {
            if (!global.PlayerData.FishingRodSolved)
            {
                CodeProblemPanel.ProblemId = problemPoint.UniqueIdentifier;
                CodeProblemPanel.Point = problemPoint;

                CodeProblemPanel.Reset();
                CodeProblemPanel.ShowDisplay(problemPoint.Code,
                    problemPoint.Items,
                    problemPoint.SolvableAreas, false);
            }
        }

        private void OnProblemSolved()
        {
            GD.Print("fishing rod problem solved");

            global.PlayerData.FishingRodSolved = true;
            global.PlayerData.Stats.AddLevelPoints(7);
        }
    }
}
