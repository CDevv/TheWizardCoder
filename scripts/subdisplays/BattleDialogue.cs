using Godot;
using Godot.Collections;
using System;
using DialogueManagerRuntime;

public partial class BattleDialogue : NinePatchRect
{
	[Export]
	public PackedScene ItemButtonTemplate { get; set; }

	[Export]
	public Label ItemDescriptionLabel { get; set; }

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

		foreach (var item in global.PlayerData.Inventory)
		{
			Button button = ItemButtonTemplate.Instantiate<Button>();
			button.Set(Button.PropertyName.Text, item);
			button.FocusEntered += () => {
				ItemDescriptionLabel.Text = "Desc.";
			};
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
