using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;
using TheWizardCoder.Enums;
using TheWizardCoder.UI;

namespace TheWizardCoder.Subdisplays
{
    public partial class MainMenuSavedGames : Display
    {
        [Signal]
        public delegate void BackButtonTrigerredEventHandler();

        [Export]
        public PackedScene FirstRoom { get; set; }
        [Export]
        public DeleteFileConfirmation confirmation;
        [Export]
        public TransitionRect Transition;
        [Export]
        public MainMenuDisplay MainMenu { get; set; }

        private Button loadButton;
        private SaveFileOption[] saveButtons = new SaveFileOption[3];
        private SaveFileAction mode;

        public override void _Ready()
        {
            base._Ready();
            loadButton = GetNode<Button>("%LoadButton");
            saveButtons = new SaveFileOption[3] {
                GetNode<SaveFileOption>("Save1"),
                GetNode<SaveFileOption>("Save2"),
                GetNode<SaveFileOption>("Save3")
            };
        }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionPressed("ui_cancel"))
            {
                if (MainMenu.Level == 3)
                {
                    MainMenu.Level = 2;
                    loadButton.GrabFocus();
                    confirmation.HideDisplay();
                }
            }
        }

        public override void ShowDisplay()
        {
            Show();
            loadButton.GrabFocus();
        }

        public override void UpdateDisplay()
        {
            for (int i = 0; i < saveButtons.Length; i++)
            {
                SaveFileOption button = saveButtons[i];
                string fileName = saveButtons[i].Name.ToString().ToLower();
                SaveFileData saveData = global.SaveFiles.ReadSaveFile(fileName);
                button.ShowData(saveData);
            }
        }

        public void FocusFirst()
        {
            loadButton.GrabFocus();
        }

        private void OnLoadButton()
        {
            mode = SaveFileAction.Load;
            saveButtons[0].GrabFocus();
            MainMenu.Level = 3;
        }

        private void OnDeleteButton()
        {
            mode = SaveFileAction.Delete;
            saveButtons[0].GrabFocus();
            MainMenu.Level = 3;
        }

        private async void OnSaveButton(int saveNumber)
        {
            if (mode == SaveFileAction.Load)
            {
                SaveFileData data = global.SaveFiles.ReadSaveFile($"save{saveNumber}");
                if (data.IsSaveEmpty)
                {
                    Transition.Show();
                    Transition.PlayAnimation();
                    await ToSignal(Transition, TransitionRect.SignalName.AnimationFinished);

                    global.ChosenSaveSlot = $"save{saveNumber}";
                    global.GoToGameIntro();
                }
                else
                {
                    global.SaveFiles.LoadSaveFile($"save{saveNumber}");
                    global.ChangeRoom(global.PlayerData.SceneFileName, global.PlayerData.SceneDefaultMarker, Direction.Down);
                }
            }
            else
            {
                confirmation.ShowDisplay($"save{saveNumber}");
            }
        }

        public void OnBackButton()
        {
            EmitSignal(SignalName.BackButtonTrigerred);
        }

        private void OnFileDeleted()
        {
            UpdateDisplay();
            loadButton.GrabFocus();
        }
    }
}