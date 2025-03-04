using Godot;
using Godot.Collections;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;

namespace TheWizardCoder.Subdisplays
{
    public partial class CharacterArmour : Display
    {
        private Color unequippedColor = new("dfdfdf");
        private Color unequippedColorHover = new("f2f2f25f");
        private Color equippedColor = new("00ab00");
        private Color equippedColorHover = new("00f000");

        [Export]
        public PackedScene TextButtonTemplate { get; set; }

        private AnimatedSprite2D portrait;
        private Label name;
        private GridContainer container;
        private Label description;

        private Label healthLabel;
        private Label manaLabel;
        private Label agilityLabel;
        private Label attackLabel;
        private Label defenseLabel;

        private Character character;
        private Button firstButton;

        public override void _Ready()
        {
            base._Ready();

            portrait = GetNode<AnimatedSprite2D>("%Portrait");
            name = GetNode<Label>("%NameLabel");
            container = GetNode<GridContainer>("%ArmoursGridContainer");
            description = GetNode<Label>("%Description");

            healthLabel = GetNode<Label>("%HealthEffect");
            manaLabel = GetNode<Label>("%ManaEffect");
            agilityLabel = GetNode<Label>("%AgilityEffect");
            attackLabel = GetNode<Label>("%AttackEffect");
            defenseLabel = GetNode<Label>("%DefenseEffect");
        }

        public override void ShowDisplay()
        {
            Show();
        }

        public void ShowDisplay(Character character)
        {
            this.character = character;

            portrait.Animation = character.Name;
            name.Text = character.Name;

            description.Text = "No Armours.";

            ClearContainer();
            PopulateContainer();
            UpdateEffectLabels();

            Show();
            FocusOnFirstItem();
        }

        public override void UpdateDisplay()
        {
            if (character != null)
            {
                ClearContainer();
                PopulateContainer();
                UpdateEffectLabels();
            }
        }

        private void ClearContainer()
        {
            Array<Node> nodes = container.GetChildren();

            foreach (var item in nodes)
            {
                item.QueueFree();
            }
        }

        private void PopulateContainer()
        {
            for (int i = 0; i < character.Armours.Count; i++)
            {
                Button currentArmour = AddItem(character.Armours[i]);
                if (i == 0)
                {
                    firstButton = currentArmour;
                }
            }
        }

        private Button AddItem(string name)
        {
            Armour armour = global.Armours[name];

            Button button = TextButtonTemplate.Instantiate<Button>();
            button.Text = name;
            button.Set("theme_override_font_sizes/font_size", 32);

            button.FocusEntered += () =>
            {
                description.Text = armour.Description;
            };

            button.Pressed += () => OnArmourPressed(button, armour.Name);

            if (character.HasEquippedArmour(name))
            {
                button.Set("theme_override_colors/font_color", equippedColor);
                button.Set("theme_override_colors/font_focus_color", equippedColorHover);
            }

            container.AddChild(button);

            return button;
        }

        private void FocusOnFirstItem()
        {
            if (firstButton != null)
            {
                firstButton.GrabFocus();
            }
            else
            {
                description.Text = "No Armour.";
            }
        }

        private void UpdateEffectLabels()
        {
            healthLabel.Text = $"+{character.ArmourEffects.Health}";
            manaLabel.Text = $"+{character.ArmourEffects.Mana}";
            agilityLabel.Text = $"+{character.ArmourEffects.Agility}";
            attackLabel.Text = $"+{character.ArmourEffects.Attack}";
            defenseLabel.Text = $"+{character.ArmourEffects.Defense}";
        }

        private void OnArmourPressed(Button button, string name)
        {
            if (character.HasEquippedArmour(name))
            {
                character.UnequipArmour(name);

                button.Set("theme_override_colors/font_color", unequippedColor);
                button.Set("theme_override_colors/font_focus_color", unequippedColorHover);
            }
            else
            {
                character.EquipArmour(name);

                button.Set("theme_override_colors/font_color", equippedColor);
                button.Set("theme_override_colors/font_focus_color", equippedColorHover);
            }

            character.ApplyArmourEffects();
            UpdateEffectLabels();
        }
    }
}
