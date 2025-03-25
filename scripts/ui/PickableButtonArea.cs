using Godot;

namespace TheWizardCoder.UI
{
    public partial class PickableButtonArea : Area2D
    {
        [Signal]
        public delegate void ButtonAddedEventHandler(string buttonText);
        [Signal]
        public delegate void ButtonRemovedEventHandler();

        [Export]
        public string ButtonText { get; set; }
        public bool Taken { get; set; } = false;
    }
}