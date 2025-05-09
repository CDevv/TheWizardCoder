using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;
using TheWizardCoder.Interactables;

namespace TheWizardCoder.Rooms
{
    public partial class Village4 : BaseRoom
    {
        [Export]
        public Resource DialogueResource { get; set; }

        private Warper watchtowerWarper;
        private CodeProblemPoint codeProblem;
        private int tutorialProgress = 1;
        private Button invisButton;

        public override async void OnReady()
        {
            base.OnReady();
            watchtowerWarper = GetNode<Warper>("WatchtowerWarper");
            codeProblem = GetNode<CodeProblemPoint>("CodeProblemPoint");
            invisButton = GetNode<Button>("%InvisButton");

            if (global.PlayerData.HasSolvedWatchtowerGlitch)
            {
                AnimationPlayer.Play("final_pos");
                watchtowerWarper.Active = true;

                if (!global.PlayerData.LintonDummyCutscene && global.PlayerData.HasMetLinton)
                {
                    global.PlayerData.LintonDummyCutscene = true;
                    await PlayCutscene("linton_4");
                    await ShowDialogue(DialogueResource, "linton_3");
                    BattleDisplay.IsTutorial = true;
                    invisButton.GrabFocus();
                    BattleDisplay.ShowDisplay(new() { "Dummy" });


                }
                else if (global.PlayerData.LintonDummyCutscene)
                {
                    AnimationPlayer.Play("linton_7");
                }
            }
        }

        private async void OnCodeSolved()
        {
            await PlayCutscene("code_solved");
            watchtowerWarper.Active = true;

            global.PlayerData.RemoveFromInventory("'s'");
            global.PlayerData.RemoveFromInventory("'t'");
            global.PlayerData.RemoveFromInventory("'a'");
            global.PlayerData.RemoveFromInventory("'r'");

            global.PlayerData.Stats.AddLevelPoints(5);
        }

        private async void OnBattleFinished()
        {
            await PlayCutscene("linton_6");
            await ShowDialogue(DialogueResource, "linton_4");
            await PlayCutscene("linton_7");
        }

        private void OnDialogueEnded(string initialTitle, string lastTitle)
        {
            if (initialTitle.Contains("tutorial_"))
            {
                BattleDisplay.BattleOptions.ShowOptions();
            }
        }

        private void OnTurnFinished()
        {
            if (tutorialProgress < 3)
            {
                if (global.CurrentRoom.BattleDisplay.IsBattleEnded)
                {
                    return;
                }

                switch (tutorialProgress)
                {
                    case 1:
                        ProgressCheckpointOne();
                        break;
                    case 2:
                        ProgressCheckpointTwo();
                        break;
                    case 3:
                        ProgressCheckpointThree();
                        break;
                }
            }
        }

        private async void NextTutorialStep()
        {
            invisButton.GrabFocus();
            await ShowDialogue(DialogueResource, $"tutorial_battle_{tutorialProgress}");
            BattleDisplay.BattleOptions.ShowOptions();

            global.GameDisplayEnabled = false;
            tutorialProgress++;
        }

        private async void ProgressCheckpointOne()
        {
            switch (global.CurrentRoom.BattleDisplay.Allies.Characters.BattleStates[0].Action)
            {
                case CharacterAction.Attack:
                    NextTutorialStep();
                    break;
                case CharacterAction.Magic:
                    await ShowDialogue(DialogueResource, $"tutorial_unnecessary_magic");
                    global.GameDisplayEnabled = false;
                    break;
                default:
                    await ShowDialogue(DialogueResource, $"tutorial_bad_decision");
                    global.GameDisplayEnabled = false;
                    break;
            }
        }

        private async void ProgressCheckpointTwo()
        {
            switch (global.CurrentRoom.BattleDisplay.Allies.Characters.BattleStates[0].Action)
            {
                case CharacterAction.Defend:
                    NextTutorialStep();
                    break;
                case CharacterAction.Magic:
                    await ShowDialogue(DialogueResource, $"tutorial_unnecessary_magic");
                    global.GameDisplayEnabled = false;
                    break;
                default:
                    await ShowDialogue(DialogueResource, $"tutorial_bad_decision");
                    global.GameDisplayEnabled = false;
                    break;
            }
        }

        private async void ProgressCheckpointThree()
        {
            switch (global.CurrentRoom.BattleDisplay.Allies.Characters.BattleStates[0].Action)
            {
                case CharacterAction.Attack:
                    NextTutorialStep();
                    break;
                case CharacterAction.Magic:
                    await ShowDialogue(DialogueResource, $"tutorial_unnecessary_magic");
                    global.GameDisplayEnabled = false;
                    break;
                default:
                    await ShowDialogue(DialogueResource, $"tutorial_bad_decision");
                    global.GameDisplayEnabled = false;
                    break;
            }
        }
    }
}