using Godot;
using Godot.Collections;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Subdisplays
{
    public partial class InventoryDisplay : Display
    {
        [Signal]
        public delegate void ItemPressedEventHandler(int index);
        [Export]
        public ControlsDisplay Controls { get; set; }
        [Export]
        public PackedScene ItemButtonTemplate { get; set; }

        private GridContainer itemsContainer;
        private Button noItemsButton;
        private NinePatchRect descriptionRect;
        private Label itemDescription;

        public override void _Ready()
        {
            base._Ready();
            itemsContainer = GetNode<GridContainer>("%ItemsContainer");
            noItemsButton = GetNode<Button>("%NoItemsButton");
            descriptionRect = GetNode<NinePatchRect>("%DescriptionRect");
            itemDescription = GetNode<Label>("%ItemDescription");
        }

        public override void ShowDisplay()
        {
            Show();
            FocusFirst();
        }

        public override void UpdateDisplay()
        {
            Array<Node> oldNodes = itemsContainer.GetChildren();
            foreach (Node item in oldNodes)
            {
                item.QueueFree();
            }

            for (int i = 0; i < global.PlayerData.Inventory.Count; i++)
            {
                int currentIndex = i;
                string item = global.PlayerData.Inventory[i];

                Button button = ItemButtonTemplate.Instantiate<Button>();
                button.Set(Button.PropertyName.Text, item);
                button.Set("theme_override_font_sizes/font_size", 32);
                button.FocusEntered += () =>
                {
                    itemDescription.Text = global.ItemDescriptions[item].Description;
                };
                button.Pressed += () =>
                {
                    EmitSignal(SignalName.ItemPressed, currentIndex);
                };
                itemsContainer.AddChild(button);
            }
        }

        private void FocusFirst()
        {
            descriptionRect.Show();
            noItemsButton.Hide();

            Controls.ChangeXLabel("Go Back");

            Button item = itemsContainer.GetChildOrNull<Button>(0);
            if (item == null)
            {
                descriptionRect.Hide();
                noItemsButton.Show();
                noItemsButton.GrabFocus();

                Controls.ChangeZLabel("");
            }
            else
            {
                item.GrabFocus();

                Controls.ChangeZLabel("Use Item");
            }
        }
    }
}