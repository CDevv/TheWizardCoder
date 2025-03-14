using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Forest6 : ForestRoom
    {
        public override void OnReady()
        {
            base.OnReady();
            global.RemoveFromInventory("Forest4.cs");
            global.AddToInventory("Forest5.cs", true);
        }
    }
}
