using Godot;
using TheWizardCoder.Autoload;
using TheWizardCoder.Enums;
using TheWizardCoder.UI;

namespace TheWizardCoder.Displays
{
    public partial class GameIntroDisplay : CanvasLayer
    {
        private const double DefaultWaitingTime = 2;
        private string[] partNames = new string[] { "intro_1", "intro_2", "intro_3", "intro_4" };

        [Export]
        public Resource DialogueResource { get; set; }

        private Global global;
        private TransitionRect transition;
        private Label label;
        private Label userInputLabel;
        private NinePatchRect demoItems;
        private NinePatchRect demoCode;
        private Sprite2D cursor;
        private bool waitingForInput = false;
        private string userInput;
        private int currentLine = 0;
        private AnimationPlayer animationPlayer;

        public override void _Ready()
        {
            global = GetNode<Global>("/root/Global");
            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            transition = GetNode<TransitionRect>("TransitionRect");
            label = GetNode<Label>("%IntroLabel");
            userInputLabel = GetNode<Label>("%UserInputLabel");

            demoItems = GetNode<NinePatchRect>("%ItemsDemoRect");
            demoCode = GetNode<NinePatchRect>("%CodeDemoRect");
            cursor = GetNode<Sprite2D>("%CursorDemo");

            transition.Show();
            transition.PlayAnimationBackwards();

            label.Text = global.GameIntroStrings["intro_1"];
            animationPlayer.Play("intro_1");
        }

        public override async void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed("ui_accept"))
            {
                currentLine++;
                if (currentLine < partNames.Length)
                {
                    animationPlayer.Stop();
                    animationPlayer.Play("hide");
                    await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);

                    if (currentLine == 2)
                    {
                        demoCode.Visible = true;
                        demoItems.Visible = true;
                        cursor.Visible = true;
                    }
                    else
                    {
                        demoCode.Visible = false;
                        demoItems.Visible = false;
                        cursor.Visible = false;
                    }

                    string title = partNames[currentLine];
                    label.Text = global.GameIntroStrings[title];

                    animationPlayer.PlayBackwards("hide");
                    await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);

                    animationPlayer.Play(title);

                    if (currentLine == 3)
                    {
                        waitingForInput = true;
                        return;
                    }
                }
            }

            if (@event is InputEventKey && @event.IsPressed())
            {
                InputEventKey inputKey = (InputEventKey)@event;
                if (waitingForInput)
                {
                    string keyString = inputKey.Keycode.ToString().ToLower();

                    if (inputKey.Keycode == Key.Enter)
                    {
                        waitingForInput = false;

                        global.SaveFiles.CreateSaveFile(global.ChosenSaveSlot, userInput);
                        global.ChangeRoom("first_room", "AfterCutsceneMarker", Direction.Down);
                        return;
                    }
                    else
                    {
                        char keyChar = (char)inputKey.Unicode;
                        if (char.IsLetter(keyChar))
                        {
                            userInput += inputKey.AsTextKeycode();
                            userInputLabel.Text = userInput;
                        }

                        if (inputKey.Keycode == Key.Backspace)
                        {
                            if (userInput.Length > 0)
                            {
                                userInput = userInput.Substring(0, userInput.Length - 1);
                                userInputLabel.Text = userInput;
                            }
                        }
                    }
                }
            }
        }
    }
}