using Godot;
using Godot.Collections;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Subdisplays
{
    public partial class CharacterArmour : Display
    {
        private Color unequippedColor = new("dfdfdf");
        private Color unequippedColorHover = new("f2f2f25f");
        private Color equippedColor = new("00ab00");
        private Color equippedColorHover = new("00f000");

        private string primaryWeaponDesc = "Primary Weapon: ";
        private string primaryArmourDesc = "Primary Armour: ";
        private string secondaryArmourDesc = "Secondary Armour: ";

        [Export]
        public PackedScene TextButtonTemplate { get; set; }

        private AnimatedSprite2D portrait;
        private Label name;
        private GridContainer container;
        private Label description;
        private Button primaryWeapon;

        private Label healthLabel;
        private Label manaLabel;
        private Label agilityLabel;
        private Label attackLabel;
        private Label defenseLabel;

        private Dictionary<int, string> slotDescriptions;
        private ArmourType currentArmourType;
        private Character character;
        private Button firstButton;

        public override void _Ready()
        {
            base._Ready();

            portrait = GetNode<AnimatedSprite2D>("%Portrait");
            name = GetNode<Label>("%NameLabel");
            container = GetNode<GridContainer>("%ItemsContainer");
            description = GetNode<Label>("%Description");
            primaryWeapon = GetNode<Button>("%PrimaryWeapon");

            healthLabel = GetNode<Label>("%HealthEffect");
            manaLabel = GetNode<Label>("%ManaEffect");
            agilityLabel = GetNode<Label>("%AgilityEffect");
            attackLabel = GetNode<Label>("%AttackEffect");
            defenseLabel = GetNode<Label>("%DefenseEffect");

            slotDescriptions = new Dictionary<int, string>();
            slotDescriptions[(int)ArmourType.PrimaryWeapon] = primaryWeaponDesc;
            slotDescriptions[(int)ArmourType.PrimaryArmour] = primaryArmourDesc;
            slotDescriptions[(int)ArmourType.SecondaryArmour] = secondaryArmourDesc;
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

            description.Text = "No Armour.";

            currentArmourType = ArmourType.PrimaryWeapon;

            UpdateEffectLabels();

            Show();
            
            primaryWeapon.GrabFocus();
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
            firstButton = null;
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
                string armourName = character.Armours[i];
                Armour armour = global.Armours[armourName];

                if (armour.Type == currentArmourType)
                {
                    Button currentArmour = AddItem(armourName);
                    if (firstButton == null)
                    {
                        firstButton = currentArmour;
                    }

                    if (character.EquippedArmours.ContainsKey((int)currentArmourType))
                    {
                        if (character.EquippedArmours[(int)currentArmourType] == armourName)
                        {
                            currentArmour.Set("theme_override_colors/font_color", equippedColor);
                            currentArmour.Set("theme_override_colors/font_focus_color", equippedColorHover);
                        }
                    }
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

        private void OnIndexFocusEntered(int index)
        {
            currentArmourType = (ArmourType)index;
            string slotDescription = slotDescriptions[index];

            ClearContainer();
            PopulateContainer();

            if (character.EquippedArmours.ContainsKey(index))
            {
                if (string.IsNullOrEmpty(character.EquippedArmours[index]))
                {
                    description.Text = $"{slotDescription}None";
                }
                else
                {
                    string armourName = character.EquippedArmours[index];
                    Armour armour = global.Armours[armourName];

                    description.Text = $"{slotDescription}{armour.Name}";
                }
            }
            else
            {
                description.Text = $"{slotDescription}None";
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
            int index = (int)currentArmourType;

            if (character.HasEquippedArmour(index))
            {
                character.UnequipArmour(index);
            }
            
            character.EquipArmour(index, name);

            string indexName = currentArmourType.ToString();
            Button indexButton = GetNode<Button>($"%{indexName}");

            indexButton.Text = name;
            indexButton.GrabFocus();

            character.ApplyArmourEffects();
            UpdateEffectLabels();
        }
    }
}
