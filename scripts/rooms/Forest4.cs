using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Forest4 : ForestRoom
    {
        public override void OnReady()
        {
            base.OnReady();
            global.RemoveFromInventory("Forest3.cs");
            global.AddToInventory("Forest4.cs", true);
        }
    }
}
