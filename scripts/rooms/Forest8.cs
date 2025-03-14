using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Forest8 : ForestRoom
    {
        public override void OnReady()
        {
            base.OnReady();
            global.RemoveFromInventory("Forest5.cs");
            global.AddToInventory("Forest6.cs", true);
        }
    }
}
