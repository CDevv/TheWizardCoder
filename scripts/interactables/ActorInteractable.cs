using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Components;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Interactables
{
    public partial class ActorInteractable : Interactable
    {
        [Signal]
        public delegate void InteractionEndedEventHandler();

        public Resource DialogueResource { get; set; }
        public string DialogueTitle { get; set; }
        public Direction DefaultDirection { get; set; } = Direction.Down;
        public AnimatedSprite2D Sprite { get; set; }

        public override void _Ready()
        {
            base._Ready();
        }

        public override void Action()
        {
            Player player = global.CurrentRoom.Player;
            Sprite.Frame = global.ReverseDirections[(int)player.Direction];
            global.CurrentRoom.Dialogue.ShowDisplay(DialogueResource, DialogueTitle);
            global.CurrentRoom.Dialogue.DialogueEnded += OnDialogueEnded;
        }

        private void OnDialogueEnded(string initialTitle, string title)
        {
            Sprite.Frame = (int)DefaultDirection;
            global.CurrentRoom.Dialogue.DialogueEnded -= OnDialogueEnded;

            EmitSignal(SignalName.InteractionEnded);
        }
    }
}