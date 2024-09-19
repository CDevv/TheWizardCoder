using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class GameDisplay : Display
{
	private int level = 0;
	private Button itemsButton;
	private AudioStreamPlayer audioPlayer;
	private Label gold;
	private MenuAction action;
	private int itemIndex;

	public override void _Ready()
	{
		base._Ready();
		itemsButton = GetNode<Button>("%ItemsButton");
		audioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");
		gold = GetNode<Label>("%GoldLabel");

		AddSubdisplay("Inventory", GetNode<InventoryDisplay>("%InventoryMenu"));
		AddSubdisplay("Options", GetNode<OptionsMenu>("%OptionsMenu"));
		AddSubdisplay("Controls", GetNode<ControlsMenu>("%ControlsMenu"));
		AddSubdisplay("PartyMembers", GetNode<PartyMembersList>("PartyMembersList"));
		AddSubdisplay("Status", GetNode<CharacterStatus>("CharacterStatus"));
		AddSubdisplay("Magic", GetNode<CharacterMagicSpells>("CharacterMagic"));

		HideAllSubdisplays();
		Hide();
		UpdateAllSubdisplays();
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
						HideAllSubdisplays();
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
	}

	public void OnControlsMenu()
	{
		level = 2;
		ChangeSubdisplay("Controls");
	}

	private void OnItemPressed(int index)
	{
		itemIndex = index;
		level = 1;
		action = MenuAction.Items;
		ChangeSubdisplay("PartyMembers");
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

	private void OnCharacterPressed()
	{
		switch (action)
		{
			case MenuAction.Items:
				level = 0;
				string itemName = global.PlayerData.Inventory[itemIndex];
				Item itemData = global.ItemDescriptions[itemName];

				global.PlayerData.Stats.AddHealth(itemData.Effect);
				global.PlayerData.RemoveFromInventory(itemIndex);

				UpdateAllSubdisplays();
				itemsButton.GrabFocus();

				break;
			
			case MenuAction.Stats:
				HideAllSubdisplays();
				((CharacterStatus)Subdisplays["Status"]).ShowDisplay(global.PlayerData.Stats);

				break;

			case MenuAction.Magic:
				HideAllSubdisplays();
				((CharacterMagicSpells)Subdisplays["Magic"]).ShowDisplay(global.PlayerData.Stats);

				break;
		}
	}
}
