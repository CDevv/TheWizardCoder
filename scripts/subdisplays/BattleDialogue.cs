using Godot;
using Godot.Collections;
using System;
using DialogueManagerRuntime;

public partial class BattleDialogue : NinePatchRect
{
	[Export]
	public PackedScene ItemButtonTemplate { get; set; }

	private Global global;
	private RichTextLabel dialogueLabel;
	private GridContainer itemsContainer;
	private Button noItemsButton;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		dialogueLabel = GetNode<RichTextLabel>("%DialogueLabel");
		itemsContainer = GetNode<GridContainer>("%ItemsContainer");
		noItemsButton = GetNode<Button>("%NoItemsButton");
	}

	public void ShowDialogueLabel()
	{
		noItemsButton.Hide();
		dialogueLabel.Show();
		itemsContainer.Hide();
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

	public void ShowGridContainer()
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

	public void UpdateInventoryContainer()
	{
		Array<Node> oldNodes = itemsContainer.GetChildren();
		foreach (var item in oldNodes)
		{
			item.QueueFree();
		}

		foreach (var item in global.PlayerData.Inventory)
		{
			Button button = ItemButtonTemplate.Instantiate<Button>();
			button.Set(Button.PropertyName.Text, item);
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

		foreach (var item in global.PlayerData.Inventory)
		{
			Button button = ItemButtonTemplate.Instantiate<Button>();
			button.Set(Button.PropertyName.Text, item);
			itemsContainer.AddChild(button);
		}
	}
}
