using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Interactables
{
    public partial class DummyInteractable : Interactable
    {
        [Signal]
        public delegate void TriggeredEventHandler();

        public override void Action()
        {
            global.CanWalk = true;
            EmitSignal(SignalName.Triggered);
        }
    }
}