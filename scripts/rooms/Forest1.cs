using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Forest1 : ForestRoom
    {
        public override void OnReady()
        {
            base.OnReady();
            global.AddToInventory("Forest1.cs", true);
        }
    }
}
