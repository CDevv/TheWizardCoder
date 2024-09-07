using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class GameDisplay : Display
{
	private int level = 0;
	private bool visible = false;
	private TextureProgressBar healthBar;
	private Button itemsButton;
	private AudioStreamPlayer audioPlayer;
	private InventoryDisplay inventoryMenu;
	private OptionsMenu optionsMenu;
	private ControlsMenu controlsMenu;
	private PartyMembersList partyMembers;
	private MenuAction action;
	private int itemIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		healthBar = GetNode<TextureProgressBar>("%HealthBar");
		itemsButton = GetNode<Button>("%ItemsButton");
		audioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");
		optionsMenu = GetNode<OptionsMenu>("%OptionsMenu");
		controlsMenu = GetNode<ControlsMenu>("%ControlsMenu");
		inventoryMenu = GetNode<InventoryDisplay>("%InventoryMenu");
		partyMembers = GetNode<PartyMembersList>("PartyMembersList"); 

		inventoryMenu.Hide();
		Hide();
		optionsMenu.UpdateDisplay();
		controlsMenu.UpdateDisplay();
		partyMembers.UpdateDisplay();
	}

    public override void _UnhandledInput(InputEvent @event)
    {
		if (!global.GameDisplayEnabled || global.IsInCutscene)
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
						partyMembers.Hide();
						Visible = false;
					}
					else
					{
						global.CanWalk = false;
						Show();
						partyMembers.Show();
						itemsButton.GrabFocus();
						Visible = true;

						inventoryMenu.UpdateDisplay();
						partyMembers.UpdateDisplay();
					}
					break;
				case 1:
					level = 0;
					inventoryMenu.Hide();
					optionsMenu.Hide();
					partyMembers.Show();
					itemsButton.GrabFocus();
					break;
				case 2:
					OnOptionsMenu();
					break;
				default:
					break;
			}
		}
    }

    public override void ShowDisplay()
    {
        Show();
		partyMembers.Show();
		itemsButton.GrabFocus();
    }

    public override void UpdateDisplay()
    {
        partyMembers.UpdateDisplay();
		inventoryMenu.UpdateDisplay();
    }

    public void ToggleInventoryMenu()
	{
		if (inventoryMenu.Visible)
		{
			itemsButton.GrabFocus();
		}
		else
		{
			level = 1;
			inventoryMenu.ShowDisplay();
			partyMembers.Hide();
		}
	}

	public void ToggleOptionsMenu()
	{
		if (optionsMenu.Visible)
		{
			itemsButton.GrabFocus();
		}
		else
		{
			OnOptionsMenu();
		}
	}

	public void OnOptionsMenu()
	{
		level = 1;
		optionsMenu.ShowDisplay();
		controlsMenu.Hide();
		partyMembers.Hide();
	}

	public void OnControlsMenu()
	{
		level = 2;
		optionsMenu.Hide();
		controlsMenu.ShowDisplay();
	}

	private void OnItemPressed(int index)
	{
		itemIndex = index;
		level = 1;
		action = MenuAction.Items;
		inventoryMenu.Hide();
		partyMembers.ShowDisplay();
	}

	private void OnCharacterPressed()
	{
		level = 0;

		string itemName = global.PlayerData.Inventory[itemIndex];
		Item itemData = global.ItemDescriptions[itemName];

		global.PlayerData.Stats.AddHealth(itemData.Effect);
		global.PlayerData.RemoveFromInventory(itemIndex);

		inventoryMenu.UpdateDisplay();
		partyMembers.UpdateDisplay();
		itemsButton.GrabFocus();
	}
}
