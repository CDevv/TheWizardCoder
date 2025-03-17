using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Subdisplays;
using TheWizardCoder.UI;

namespace TheWizardCoder.Displays
{
    public partial class MainMenuDisplay : Display
    {
        private TransitionRect transition;
        private Control main;
        private Control savedGames;
        private Control options;
        private Button playButton;
        private Button loadButton;
        private Button invisButton;
        private bool waitingInput = false;
        private string actionName;

        private string currentMenu = string.Empty;

        public int Level { get; set; } = 0;

        public override void _Ready()
        {
            base._Ready();

            transition = GetNode<TransitionRect>("TransitionRect");
            main = GetNode<Control>("Main");
            playButton = GetNode<Button>("%PlayButton");
            invisButton = GetNode<Button>("InvisButton");

            AddSubdisplay("SavedGames", GetNode<MainMenuSavedGames>("SavedGamesMenu"));
            AddSubdisplay("Options", GetNode<OptionsMenu>("OptionsMenu"));
            AddSubdisplay("Controls", GetNode<ControlsMenu>("ControlsMenu"));
            AddSubdisplay("Credits", GetNode<CreditsDisplay>("CreditsDisplay"));

            playButton.CallDeferred(Button.MethodName.GrabFocus);

            UpdateDisplay();
            ShowDisplay();
        }

        public override void _Input(InputEvent @event)
        {
            if (waitingInput)
            {
                if (@event is InputEventKey && @event.IsPressed())
                {
                    global.Settings.ChangeControl(actionName, @event);
                    waitingInput = false;
                }
            }

            if (Input.IsActionJustPressed("ui_cancel"))
            {
                if (Level == 1)
                {
                    ShowMainMenu();
                }
                else if (Level == 2)
                {
                    if (currentMenu == "controls")
                    {
                        currentMenu = string.Empty;
                        ShowOptions();
                    }
                    else
                    {
                        ShowSavedGamesMenu();
                    }
                }
            }
        }

        public override async void ShowDisplay()
        {
            Show();
            ShowMainMenu();
            transition.PlayAnimation();
            await ToSignal(transition, TransitionRect.SignalName.AnimationFinished);
        }

        public override void UpdateDisplay()
        {
            UpdateAllSubdisplays();
        }

        public void ShowMainMenu()
        {
            Level = 0;
            main.Show();
            HideAllSubdisplays();
            playButton.GrabFocus();
            playButton.CallDeferred(Button.MethodName.GrabFocus);
        }

        public void ShowSavedGamesMenu()
        {
            Level = 1;
            main.Hide();
            ChangeSubdisplay("SavedGames");
        }

        public void ShowOptions()
        {
            Level = 1;
            main.Hide();
            ChangeSubdisplay("Options");
        }

        public void ShowControls()
        {
            currentMenu = "controls";
            Level = 2;
            ChangeSubdisplay("Controls");
        }

        public void ShowCredits()
        {
            invisButton.GrabFocus();
            Level = 1;
            ChangeSubdisplay("Credits");
        }
    }
}