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

            CodeProblemPanel.ProblemSolved += () =>
            {
                if (CodeProblemPanel.ProblemId == problemPoint.UniqueIdentifier)
                {
                    global.PlayerData.FishingRodSolved = true;
                }
            };
        }

        private void OpenFishingRodProblem()
        {
            CodeProblemPanel.ProblemId = problemPoint.UniqueIdentifier;
            CodeProblemPanel.Point = problemPoint;

            CodeProblemPanel.ShowDisplay(problemPoint.Code,
                problemPoint.Items,
                problemPoint.SolvableAreas, false);
        }
    }
}
