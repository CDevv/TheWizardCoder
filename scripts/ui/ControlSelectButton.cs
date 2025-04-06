using Godot;
using System.Linq;
using TheWizardCoder.Autoload;

namespace TheWizardCoder.UI
{
    public partial class ControlSelectButton : Button
    {
        [Export]
        public string ActionName { get; set; }
        private InputEventKey selectedControl;
        private bool waitingInput = false;
        private Global global;
        private Key[] disallowedKeys;

        public override void _Ready()
        {
            global = GetNode<Global>("/root/Global");

            disallowedKeys = new Key[]
            {
                Key.Z,
                Key.X,
                Key.Q
            };

            InputEventKey eventKey = (InputEventKey)global.Settings.Controls[ActionName];
            Text = eventKey.AsTextKeycode();
        }

        public override void _Input(InputEvent @event)
        {
            if (waitingInput)
            {
                if (@event is InputEventKey && @event.IsPressed())
                {
                    InputEventKey eventKey = (InputEventKey)@event;
                    if (IsKeyAllowed(eventKey.PhysicalKeycode))
                    {
                        Text = eventKey.AsTextKeycode();
                        global.Settings.ChangeControl(ActionName, @event);
                        global.Settings.SaveSettings();                    
                    }
                    else
                    {
                        InputEventKey currentKey = (InputEventKey)global.Settings.Controls[ActionName];
                        Text = currentKey.AsTextKeycode();
                    }

                    waitingInput = false;
                }
            }
        }

        public void OnPressed()
        {
            Text = "Waiting for input..";
            waitingInput = true;
        }

        private bool IsKeyAllowed(Key key)
        {
            if (disallowedKeys.Contains(key))
            {
                return false;
            }
            return true;
        }
    }
}