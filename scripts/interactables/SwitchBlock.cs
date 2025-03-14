using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Interactables
{
    public partial class SwitchBlock : Interactable
    {
        private readonly Color redColor = new(255, 0, 0);
        private readonly Color greenColor = new(0, 255, 0);

        [Signal]
        public delegate void PushedEventHandler();

        private bool value;
        public bool Value
        {
            get { return value; }
            set
            {
                this.value = value;
                if (colorRect != null)
                {
                    if (value)
                    {
                        colorRect.Color = greenColor;
                    }
                    else
                    {
                        colorRect.Color = redColor;
                    }
                }
            }
        }

        private ColorRect colorRect;

        public override void _Ready()
        {
            base._Ready();

            colorRect = GetNode<ColorRect>("ColorRect");
        }

        public override void Action()
        {
            Value = !Value;
            global.CanWalk = true;

            EmitSignal(SignalName.Pushed);
        }
    }
}