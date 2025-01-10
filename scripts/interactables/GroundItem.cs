using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Interactables
{
    public partial class GroundItem : Interactable
    {
        [Export]
        public string ItemName { get; set; }

        private CollisionShape2D collision;
        private Sprite2D sprite;

        public override void _Ready()
        {
            base._Ready();
            collision = GetNode<CollisionShape2D>("%Collision");
            sprite = GetNode<Sprite2D>("Sprite");
        }

        public override void Action()
        {
            Active = false;
            collision.Disabled = true;
            sprite.Hide();
            global.AddToInventory(ItemName);

            global.CanWalk = true;
        }
    }
}
