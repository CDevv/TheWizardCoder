using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Forest3 : ForestRoom
    {
        public override void OnReady()
        {
            base.OnReady();
            global.RemoveFromInventory("Forest2.cs");
            global.AddToInventory("Forest3.cs", true);
        }
    }
}
