using Godot;
using Godot.Collections;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Subdisplays
{
    public partial class CharacterArmour : Display
    {
        private Color unequippedColor = new("dfdfdf");
        private Color unequippedColorHover = new("f2f2f25f");
        private Color equippedColor = new("00ab00");
        private Color equippedColorHover = new("00f000");

        private string primaryWeaponDesc = "Primary Weapon";
        private string primaryArmourDesc = "Primary Armour";
        private string secondaryArmourDesc = "Secondary Armour";

        [Export]
        public PackedScene TextButtonTemplate { get; set; }
        [Export]
        public GameDisplay GameDisplay { get; set; }
        [Export]
        public ControlsDisplay Controls { get; set; }

        private AnimatedSprite2D portrait;
        private Label name;
        private GridContainer container;
        private Label description;

        private Button primaryWeapon;
        private Button primaryArmour;
        private Button secondaryArmour;

        private Label healthLabel;
        private Label manaLabel;
        private Label agilityLabel;
        private Label attackLabel;
        private Label defenseLabel;

        private Dictionary<int, string> slotDescriptions;
        private Array<Button> slotButtons;
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
            primaryArmour = GetNode<Button>("%PrimaryArmour");
            secondaryArmour = GetNode<Button>("%SecondaryArmour");

            healthLabel = GetNode<Label>("%HealthEffect");
            manaLabel = GetNode<Label>("%ManaEffect");
            agilityLabel = GetNode<Label>("%AgilityEffect");
            attackLabel = GetNode<Label>("%AttackEffect");
            defenseLabel = GetNode<Label>("%DefenseEffect");

            slotDescriptions = new Dictionary<int, string>();
            slotDescriptions[(int)ArmourType.PrimaryWeapon] = primaryWeaponDesc;
            slotDescriptions[(int)ArmourType.PrimaryArmour] = primaryArmourDesc;
            slotDescriptions[(int)ArmourType.SecondaryArmour] = secondaryArmourDesc;

            slotButtons = new Array<Button>();
            slotButtons.Add(primaryWeapon);
            slotButtons.Add(primaryArmour);
            slotButtons.Add(secondaryArmour);
        }

        public override void _Input(InputEvent @event)
        {
            if (Visible)
            {
                if (Input.IsActionJustPressed("secondary"))
                {
                    if (GameDisplay.Level == 1)
                    {
                        int slotIndex = (int)currentArmourType;

                        character.UnequipArmour(slotIndex);
                        character.ApplyArmourEffects();
                        UpdateEffectLabels();

                        string indexName = currentArmourType.ToString();
                        Button indexButton = GetNode<Button>($"%{indexName}");
                        indexButton.Text = "_______";

                        description.Text = $"{slotDescriptions[slotIndex]}: None";

                        ClearContainer();
                        PopulateContainer();
                    }
                }
            }
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
            UpdateSlotButtons();

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

        public void FocusOnFirstSlot()
        {
            firstButton = null;
            primaryWeapon.GrabFocus();
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
                GameDisplay.Level = 2;
                firstButton.GrabFocus();

                Controls.ChangeXLabel("Go Back");
                Controls.ChangeZLabel("Equip");

                Controls.HideQLabel();
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

        private void UpdateSlotButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                int slotIndex = i;
                if (character.HasEquippedArmour(slotIndex))
                {
                    slotButtons[slotIndex].Text = character.EquippedArmours[slotIndex];
                }
                else
                {
                    slotButtons[slotIndex].Text = "_______";
                }
            }
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

        private void OnIndexFocusEntered(int index)
        {
            GameDisplay.Level = 1;

            Controls.ChangeXLabel("Go Back");
            Controls.ChangeZLabel("Select");

            Controls.ShowQLabel();
            Controls.ChangeQLabel("Unequip");

            currentArmourType = (ArmourType)index;
            string slotDescription = slotDescriptions[index];

            ClearContainer();
            PopulateContainer();

            if (character.EquippedArmours.ContainsKey(index))
            {
                if (string.IsNullOrEmpty(character.EquippedArmours[index]))
                {
                    description.Text = $"{slotDescription}: None";
                }
                else
                {
                    string armourName = character.EquippedArmours[index];
                    Armour armour = global.Armours[armourName];

                    description.Text = $"{slotDescription}: {armour.Name}";
                }
            }
            else
            {
                description.Text = $"{slotDescription}: None";
            }
        }
    }
}
