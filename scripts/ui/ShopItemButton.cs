using Godot;
using System;
using TheWizardCoder.Enums;

namespace TheWizardCoder.UI
{
    public partial class ShopItemButton : Button
    {
        [Export]
        public ShopType ItemType { get; set; }
        [Export]
        public string ItemName { get; set; }
    }
}
