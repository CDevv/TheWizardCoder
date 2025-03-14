using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Interactables
{
    public partial class ShopPoint : Interactable
    {
        [Export]
        public string ShopName { get; set; }

        public override void _Ready()
        {
            base._Ready();
        }

        public override void Action()
        {
            global.OpenShop(ShopName);
        }
    }
}
