using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Forest12 : ForestRoom
    {
        public override void OnReady()
        {
            base.OnReady();
            global.RemoveFromInventory("Forest7.cs");

            if (!global.PlayerData.HasMetGertrude)
            {
                global.AddToInventory("Forest8.cs", true);
            }
            else
            {
                global.RemoveFromInventory("Forest8.cs");
            }
        }
    }
}
