using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Forest10 : ForestRoom
    {
        public override void OnReady()
        {
            base.OnReady();
            global.RemoveFromInventory("Forest6.cs");
            global.AddToInventory("Forest7.cs");
        }
    }
}
