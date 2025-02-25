using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Forest8 : BaseRoom
    {
        public override void OnReady()
        {
            base.OnReady();
            global.RemoveFromInventory("Forest5.cs");
            global.AddToInventory("Forest6.cs", true);
        }
    }
}
