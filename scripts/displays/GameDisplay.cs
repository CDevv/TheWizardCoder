using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using TheWizardCoder.Subdisplays;

namespace TheWizardCoder.Displays
{
    public partial class GameDisplay : Display
    {
        private int level = 0;
        private Button itemsButton;
        private AudioStreamPlayer audioPlayer;
        private Label gold;
        private ControlsDisplay controls;
        private MenuAction action;
        private int itemIndex;

        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public override void _Ready()
        {
            base._Ready();
            itemsButton = GetNode<Button>("%ItemsButton");
            audioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");
            gold = GetNode<Label>("%GoldLabel");
            controls = GetNode<ControlsDisplay>("ControlsDisplay");

            AddSubdisplay("Inventory", GetNode<InventoryDisplay>("%InventoryMenu"));
            AddSubdisplay("Options", GetNode<OptionsMenu>("%OptionsMenu"));
            AddSubdisplay("Controls", GetNode<ControlsMenu>("%ControlsMenu"));
            AddSubdisplay("PartyMembers", GetNode<PartyMembersList>("PartyMembersList"));
            AddSubdisplay("Status", GetNode<CharacterStatus>("CharacterStatus"));
            AddSubdisplay("Magic", GetNode<CharacterMagicSpells>("CharacterMagic"));
            AddSubdisplay("Armour", GetNode<CharacterArmour>("CharacterArmour"));

            controls.Hide();
            HideAllSubdisplays();
            Hide();
            UpdateAllSubdisplays();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (!global.GameDisplayEnabled || global.IsInCutscene || global.IsInShop)
            {
                return;
            }

            if (Input.IsActionJustPressed("ui_cancel"))
            {
                switch (level)
                {
                    case 0:
                        if (Visible)
                        {
                            global.CanWalk = true;
                            Hide();
                            HideAllSubdisplays();
                            controls.Hide();
                        }
                        else
                        {
                            global.CanWalk = false;
                            ShowDisplay();
                        }
                        break;
                    case 1:
                        level = 0;
                        HideAllSubdisplays();
                        Subdisplays["PartyMembers"].Show();
                        itemsButton.GrabFocus();

                        controls.ChangeXLabel("Close");
                        controls.ChangeZLabel("Confirm");
                        controls.HideQLabel();
                        break;
                    case 2:
                        if (action == MenuAction.Options)
                        {
                            OnOptionsMenu();
                        }
                        else if (action == MenuAction.Armour)
                        {
                            ((CharacterArmour)Subdisplays["Armour"]).FocusOnFirstSlot();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public override void ShowDisplay()
        {
            Show();
            controls.Show();
            HideAllSubdisplays();
            UpdateDisplay();
            Subdisplays["PartyMembers"].Show();
            itemsButton.GrabFocus();
        }

        public override void UpdateDisplay()
        {
            gold.Text = $"Gold: {global.PlayerData.Gold}";
            UpdateAllSubdisplays();
        }

        public void OnItemsMenu()
        {
            level = 1;
            ChangeSubdisplay("Inventory");
        }

        public void OnOptionsMenu()
        {
            level = 1;
            ChangeSubdisplay("Options");
            action = MenuAction.Options;
        }

        public void OnControlsMenu()
        {
            level = 2;
            ChangeSubdisplay("Controls");
        }

        private void OnStatusMenu()
        {
            level = 1;
            action = MenuAction.Stats;
            ChangeSubdisplay("PartyMembers");
        }

        private void OnMagicMenu()
        {
            level = 1;
            action = MenuAction.Magic;
            ChangeSubdisplay("PartyMembers");
        }

        private void OnArmourMenu()
        {
            level = 1;
            action = MenuAction.Armour;
            ChangeSubdisplay("PartyMembers");
        }

        private void OnItemPressed(int index)
        {
            string itemName = global.PlayerData.Inventory[index];
            Item item = global.ItemDescriptions[itemName];

            if (item.Type == ItemType.Heal || item.Type == ItemType.Mana)
            {
                itemIndex = index;
                level = 1;
                action = MenuAction.Items;
                ChangeSubdisplay("PartyMembers");
            }
            else if (item.Type == ItemType.Key)
            {
                if (item.AdditionalData.Length > 0)
                {
                    if (item.AdditionalData[0] == "Display")
                    {
                        global.CurrentRoom.Get(item.AdditionalData[1]).As<Node>().Call("ShowDisplay", item.AdditionalData[2]);
                    }
                    else if (item.AdditionalData[0] == "RoomMethod")
                    {
                        global.CurrentRoom.Call(item.AdditionalData[1], item.AdditionalData[2]);
                    }
                    else if (item.AdditionalData[0] == "PlayerMethod")
                    {
                        global.CurrentRoom.Player.Call(item.AdditionalData[1], item.AdditionalData[2]);
                        global.CanWalk = true;
                    }

                    level = 0;
                    HideDisplay();
                    HideAllSubdisplays();
                }
            }
        }



        private void OnCharacterPressed(bool isProtagonist, int allyIndex)
        {
            Character character;
            if (isProtagonist)
            {
                character = global.PlayerData.Stats;
            }
            else
            {
                character = global.PlayerData.Allies[allyIndex - 1];
            }

            switch (action)
            {
                case MenuAction.Items:
                    level = 0;
                    string itemName = global.PlayerData.Inventory[itemIndex];
                    Item itemData = global.ItemDescriptions[itemName];

                    if (itemData.Type == ItemType.Heal)
                    {
                        character.AddHealth(itemData.Effect);
                    }
                    else if (itemData.Type == ItemType.Mana)
                    {
                        character.AddMana(itemData.Effect);
                    }

                    global.PlayerData.RemoveFromInventory(itemIndex);

                    UpdateAllSubdisplays();
                    itemsButton.GrabFocus();

                    break;

                case MenuAction.Stats:
                    HideAllSubdisplays();
                    ((CharacterStatus)Subdisplays["Status"]).ShowDisplay(character);

                    controls.ChangeXLabel("Go Back");
                    controls.ChangeZLabel("");

                    break;

                case MenuAction.Magic:
                    HideAllSubdisplays();
                    ((CharacterMagicSpells)Subdisplays["Magic"]).ShowDisplay(character);

                    controls.ChangeXLabel("Go Back");
                    controls.ChangeZLabel("");

                    break;

                case MenuAction.Armour:
                    HideAllSubdisplays();
                    ((CharacterArmour)Subdisplays["Armour"]).ShowDisplay(character);

                    break;
            }
        }
    }
}