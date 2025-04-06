using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using TheWizardCoder.Subdisplays;
using TheWizardCoder.UI;

namespace TheWizardCoder.Displays
{
    public partial class SavedGamesDisplay : Display
    {
        private bool selectingSave = false;
        private string selectedSaveFileName;
        private SaveFileOption save1button;
        private SaveFileOption save2button;
        private SaveFileOption save3button;
        private Button saveButton;
        private ControlsDisplay controls;
        private SaveFileAction action;

        public override void _Ready()
        {
            base._Ready();
            save1button = GetNode<SaveFileOption>("Save1");
            save2button = GetNode<SaveFileOption>("Save2");
            save3button = GetNode<SaveFileOption>("Save3");
            saveButton = GetNode<Button>("%SaveButton");
            controls = GetNode<ControlsDisplay>("ControlsDisplay");

            controls.HideDisplay();
        }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                if (Visible)
                {
                    if (selectingSave)
                    {
                        saveButton.GrabFocus();
                        selectingSave = false;
                    }
                    else
                    {
                        global.SetDeferred(Global.PropertyName.GameDisplayEnabled, true);
                        HideDisplay();
                    }
                }
            }
        }

        public override void ShowDisplay()
        {
            global.GameDisplayEnabled = false;
            Show();
            controls.Show();
            saveButton.GrabFocus();
        }

        public override void UpdateDisplay()
        {
            SaveFileOption[] buttons = { save1button, save2button, save3button };
            for (int i = 0; i < buttons.Length; i++)
            {
                SaveFileOption button = buttons[i];
                string fileName = buttons[i].Name.ToString().ToLower();
                SaveFileData save1data = global.SaveFiles.ReadSaveFile(fileName);
                button.ShowData(save1data);
            }
        }

        public override void HideDisplay()
        {
            Hide();
            controls.HideDisplay();
            global.CanWalk = true;
        }

        public void OnSaveButton()
        {
            selectingSave = true;
            action = SaveFileAction.Save;
            save1button.GrabFocus();
        }

        public void OnLoadButton()
        {
            selectingSave = true;
            action = SaveFileAction.Load;
            save1button.GrabFocus();
        }

        public void OnCloseButton()
        {
            Hide();
            global.CanWalk = true;
            global.GameDisplayEnabled = true;
        }

        public void OnSaveOption(string saveName)
        {
            if (action == SaveFileAction.Save)
            {
                global.SaveFiles.UpdateSaveFile(saveName);
            }
            else if (action == SaveFileAction.Load)
            {
                global.SaveFiles.LoadSaveFile(saveName);
                global.ChangeRoom(global.PlayerData.SceneFileName, global.PlayerData.SceneDefaultMarker, Direction.Down);
            }

            OnCloseButton();
        }
    }
}