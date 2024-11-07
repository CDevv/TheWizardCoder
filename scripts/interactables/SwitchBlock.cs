using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Interactables
{
    public partial class SwitchBlock : Interactable
    {
        [Signal]
        public delegate void PushedEventHandler();

        public bool Value { get; private set; }

        public override void Action()
        {
            Value = !Value;
            global.CanWalk = true;

            EmitSignal(SignalName.Pushed);
        }
    }
}