using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;
using TheWizardCoder.UI;

namespace TheWizardCoder.Interactables
{
    public partial class Warper : Interactable
    {
        [Export]
        public string TargetRoomName { get; set; }
        [Export]
        public string TargetLocation { get; set; }
        [Export]
        public Direction PlayerDirection { get; set; } = Direction.Down;

        public override void _Ready()
        {
            base._Ready();
        }

        public override async void Action()
        {
            global.CanWalk = false;

            global.CurrentRoom.TransitionRect.PlayAnimation();
            await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);

            global.CurrentRoom.TransitionToRoom(TargetRoomName, TargetLocation, PlayerDirection);
        }

        public override void OnNotActive()
        {
            global.CanWalk = true;
        }
    }
}
