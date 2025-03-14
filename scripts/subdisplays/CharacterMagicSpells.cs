using Godot;
using Godot.Collections;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;

namespace TheWizardCoder.Subdisplays
{
    public partial class CharacterMagicSpells : Display
    {
        [Export]
        public PackedScene TextButtonTemplate { get; set; }

        private Character character;
        private Button firstItem;
        private AnimatedSprite2D portrait;
        private Label name;
        private GridContainer magicContainer;
        private NinePatchRect descriptionRect;
        private Label magicDescription;
        private Label characterPoints;
        private Label pointsCost;

        public override void _Ready()
        {
            base._Ready();

            portrait = GetNode<AnimatedSprite2D>("%Portrait");
            name = GetNode<Label>("%NameLabel");
            characterPoints = GetNode<Label>("%PointsLabel");
            magicContainer = GetNode<GridContainer>("%SpellsGridContainer");
            descriptionRect = GetNode<NinePatchRect>("%DescriptionRect");
            magicDescription = GetNode<Label>("%MagicDescription");
            pointsCost = GetNode<Label>("%MagicCost");
        }

        public void ShowDisplay(Character character)
        {
            this.character = character;
            firstItem = null;

            portrait.Animation = character.Name;
            name.Text = character.Name;
            characterPoints.Text = $"MP: {character.Points}";

            magicDescription.Text = "No magic spells.";
            ClearContainer();
            PopulateContainer();

            Show();
            FocusOnFirstItem();
        }

        public override void ShowDisplay()
        {
            Show();
        }

        private void PopulateContainer()
        {
            for (int i = 0; i < character.MagicSpells.Count; i++)
            {
                Button magicSpellButton = AddItem(character.MagicSpells[i]);
                if (i == 0)
                {
                    firstItem = magicSpellButton;
                }
            }
        }

        private void FocusOnFirstItem()
        {
            if (firstItem != null)
            {
                firstItem.GrabFocus();
            }
            else
            {
                magicDescription.Text = "No magic spells.";
            }
        }

        private void ClearContainer()
        {
            Array<Node> nodes = magicContainer.GetChildren();
            foreach (Node node in nodes)
            {
                node.QueueFree();
            }
        }

        private Button AddItem(string name)
        {
            MagicSpell magicSpell = global.MagicSpells[name];

            Button button = TextButtonTemplate.Instantiate<Button>();
            button.Text = name;
            button.Set("theme_override_font_sizes/font_size", 32);
            button.FocusEntered += () =>
            {
                magicDescription.Text = magicSpell.Description;
                pointsCost.Text = $"MP Cost: {magicSpell.Cost}";
            };

            magicContainer.AddChild(button);

            return button;
        }
    }
}