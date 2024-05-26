using Godot;
using Godot.Collections;
using System;
using DialogueManagerRuntime;

public partial class BattleDialogue : NinePatchRect
{
	[Signal]
	public delegate void ItemTriggeredEventHandler(string itemName);
	[Signal]
	public delegate void MagicSpellTriggeredEventHandler(string spellName);

	[Export]
	public PackedScene ItemButtonTemplate { get; set; }

	[Export]
	public BattleOptions BattleOptions { get; set; }

	private Global global;
	private RichTextLabel dialogueLabel;
	private GridContainer itemsContainer;
	private Control fightContainer;
	private Button noItemsButton;
	private TextureProgressBar fightPowerBar;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		dialogueLabel = GetNode<RichTextLabel>("%DialogueLabel");
		itemsContainer = GetNode<GridContainer>("%ItemsContainer");
		fightContainer = GetNode<Control>("FightOptionContainer");
		noItemsButton = GetNode<Button>("%NoItemsButton");
		fightPowerBar = GetNode<TextureProgressBar>("%FightPowerBar");
	}

	public void ShowDialogueLabel()
	{
		noItemsButton.Hide();
		dialogueLabel.Show();
		itemsContainer.Hide();
		fightContainer.Hide();
	}

	public void ShowFightContainer()
	{
		noItemsButton.Hide();
		dialogueLabel.Hide();
		itemsContainer.Hide();
		fightContainer.Show();
	}

	public void ShowItems()
	{
		noItemsButton.Text = "No items..";
		UpdateInventoryContainer();
		ShowGridContainer();
	}

	public void ShowMagic()
	{
		noItemsButton.Text = "No magic spells..";
		UpdateMagicContainer();
		ShowGridContainer();
	}

	private void ShowGridContainer()
	{
		dialogueLabel.Hide();
		itemsContainer.Show();

		Array<Node> items = itemsContainer.GetChildren();
		if (items.Count > 0)
		{
			((Button)items[0]).GrabFocus();
		}
		else
		{
			noItemsButton.Show();
			noItemsButton.GrabFocus();
		}
	}

	public void SetText(string text)
	{
		dialogueLabel.Text = text;
	}

	public void SetDialogueLine(DialogueLine line)
	{
		dialogueLabel.Set("dialogue_line", line);
		dialogueLabel.Call("type_out");
	}

	public void SetFightPowerBarValue(double value)
	{
		fightPowerBar.Value = value;
	}

	public void UpdateInventoryContainer()
	{
		Array<Node> oldNodes = itemsContainer.GetChildren();
		foreach (var item in oldNodes)
		{
			item.QueueFree();
		}

		for (int i = 0; i < global.PlayerData.Inventory.Count; i++)
		{
			string item = global.PlayerData.Inventory[i];

			Button button = ItemButtonTemplate.Instantiate<Button>();
			button.Set("metadata/itemId", i);
			button.Set(Button.PropertyName.Text, item);		
			button.FocusEntered += () => {
				BattleOptions.SetItemDescription(global.ItemDescriptions[item].Description);
			};
			button.Pressed += () => OnItemPressed(button, item, i);
			itemsContainer.AddChild(button);
		}
	}

	public void UpdateMagicContainer()
	{
		Array<Node> oldNodes = itemsContainer.GetChildren();
		foreach (var item in oldNodes)
		{
			item.QueueFree();
		}

		for (int i = 0; i < global.PlayerData.MagicSpells.Count; i++)
		{
			string item = global.PlayerData.MagicSpells[i];

			Button button = ItemButtonTemplate.Instantiate<Button>();
			button.Set(Button.PropertyName.Text, item);
			button.FocusEntered += () => {
				BattleOptions.SetItemDescription(global.MagicSpells[item].Description);
			};
			button.Pressed += () => OnMagicSpellPressed(button, item, i);
			itemsContainer.AddChild(button);
		}
	}

	private void OnItemPressed(Button button, string itemName, int index)
	{
		EmitSignal(SignalName.ItemTriggered, itemName);
		global.PlayerData.RemoveFromInventory(index);
		button.QueueFree();
		ShowItems();
	}

	private void OnMagicSpellPressed(Button button, string spellName, int index)
	{
		EmitSignal(SignalName.MagicSpellTriggered, spellName);
		ShowDialogueLabel();
		BattleOptions.ShowOptions();
		BattleOptions.FocusFirst();
	}
}
