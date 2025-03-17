using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;
using TheWizardCoder.UI;

namespace TheWizardCoder.Subdisplays
{
    public partial class OptionsMenu : Display
    {
        [Signal]
        public delegate void OnControlsButtonPressedEventHandler();
        [Signal]
        public delegate void OnBackButtonPressedEventHandler();

        private OptionButton resolutionsButton;
        private CustomCheckbox fullscreenButton;
        private CustomCheckbox autoSprintButton;

        public override void _Ready()
        {
            base._Ready();
            resolutionsButton = GetNode<OptionButton>("%ResolutionOptions");
            fullscreenButton = GetNode<CustomCheckbox>("%FullscreenCheckBox");
            autoSprintButton = GetNode<CustomCheckbox>("%AutoSprintCheckBox");
        }

        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                if (Visible)
                {
                    EmitSignal(SignalName.OnBackButtonPressed);
                }
            }
        }

        public override void ShowDisplay()
        {
            Show();
            FocusFirst();
        }

        public override void UpdateDisplay()
        {
            resolutionsButton.Set(OptionButton.PropertyName.Selected, (int)global.Settings.WindowSize);
            fullscreenButton.SetToggled(global.Settings.Fullscreen);
            autoSprintButton.SetToggled(global.Settings.AutoSprint);
        }

        public void FocusFirst()
        {
            resolutionsButton.CallDeferred(Button.MethodName.GrabFocus);
        }

        public void OnWindowSizeChanged(int optionId)
        {
            WindowSize size = (WindowSize)optionId;
            global.Settings.ChangeWindowSize(size);
            global.Settings.SaveSettings();
        }

        public void OnFullscreenToggled(bool toggled)
        {
            global.Settings.ToggleFullscreen(toggled);
            global.Settings.SaveSettings();
        }

        private void OnAutoSprintToggled(bool toggled)
        {
            global.Settings.AutoSprint = toggled;
            global.Settings.SaveSettings();
        }

        public void OnBackButton()
        {
            EmitSignal(SignalName.OnBackButtonPressed);
        }

        public void OnControlsPage()
        {
            EmitSignal(SignalName.OnControlsButtonPressed);
        }
    }
}