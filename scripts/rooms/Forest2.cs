using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Forest2 : ForestRoom
    {
        public override void OnReady()
        {
            base.OnReady();
            global.RemoveFromInventory("Forest1.cs");
            global.AddToInventory("Forest2.cs", true);
        }
    }
}
