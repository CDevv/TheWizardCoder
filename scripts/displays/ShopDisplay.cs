using Godot;
using Godot.Collections;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using TheWizardCoder.UI;

public partial class ShopDisplay : Display
{
	[Export]
	private PackedScene TextButtonPackedScene { get; set; }

	private TextureRect background;
	private NinePatchRect optionsRect;
	private NinePatchRect descriptionRect;
	private GridContainer itemsContainer;
	private Label gold;
	private Label description;
	private Label priceLabel;
	private Button buyButton;
	private Label shopHint;
	private int level = 0;

	public Shop Shop { get; private set; }

	public override void _Ready()
	{
		base._Ready();
		//global = GetNode<Global>("/root/Global");

		background = GetNode<TextureRect>("ShopBackground");
		optionsRect = GetNode<NinePatchRect>("%OptionsRect");
		descriptionRect = GetNode<NinePatchRect>("%DescriptionRect");
		itemsContainer = GetNode<GridContainer>("%ItemsContainer");
		gold = GetNode<Label>("%Gold");
		description = GetNode<Label>("%Description");
		priceLabel = GetNode<Label>("%Price");
		buyButton = GetNode<Button>("%Buy");
		shopHint = GetNode<Label>("%ShopHint");
	}

    public override void _Process(double delta)
    {
		if (Visible)
		{
			if (Input.IsActionJustPressed("ui_cancel"))
			{
				ShowDisplay();
				switch (level)
				{
					case 0:
						HideDisplay();
						break;
					case 1:
						ShowDisplay();
						level = 0;
						break;
				}
			}
		}
    }

    public override void ShowDisplay()
    {
        ShowDisplay(Shop.Name, false);
    }

	public async void ShowDisplay(string shopId, bool playTransition = true)
	{
		global.GameDisplayEnabled = false;
		global.CanWalk = false;

		if (playTransition)
		{
			global.CurrentRoom.TransitionRect.PlayAnimation();
			await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
		}

		Show();
		Shop = global.Shops[shopId];
		background.Texture = ResourceLoader.Load<Texture2D>($"res://assets/shops/{Shop.Name}.png");

		UpdateDisplay();
		itemsContainer.Hide();
		descriptionRect.Hide();

		shopHint.Show();
		buyButton.GrabFocus();

		if (playTransition)
		{
			global.CurrentRoom.TransitionRect.PlayAnimationBackwards();
			await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
		}
	}

	public override void UpdateDisplay()
	{
		gold.Text = $"Gold: {global.PlayerData.Gold}";

		UpdateShopItems();
		UpdateInventoryItems();
	}

    public override async void HideDisplay()
    {
		global.CurrentRoom.TransitionRect.PlayAnimation();
		await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);

        Hide();
		global.CanWalk = true;
		global.GameDisplayEnabled = true;

		global.CurrentRoom.TransitionRect.PlayAnimationBackwards();
		await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
    }

    private Button AddItem(int index, string itemName, ShopAction shopAction)
	{
		Button newItem = TextButtonPackedScene.Instantiate<Button>();
		newItem.Text = itemName;

		newItem.Pressed += () => OnItemButton(index, itemName, shopAction);
		newItem.FocusEntered += () => {
			ShowDescription(itemName);
		};

		itemsContainer.AddChild(newItem);
		return newItem;
	}

	private void ClearItems()
	{
		Array<Node> array = itemsContainer.GetChildren();

		foreach (Node item in array)
		{
			item.QueueFree();
		}
	}

	private void ShowDescription(string itemName)
	{
		Item item = global.ItemDescriptions[itemName];

		descriptionRect.Show();
		description.Text = item.Description;
		priceLabel.Text = $"Price: {item.Price}";
	}

	private Button UpdateShopItems()
	{
		Button firstItem = null;
		shopHint.Hide();
		ClearItems();

		for (int i = 0; i < Shop.Items.Count; i++)
		{
			int currentIndex = i;
			string item = Shop.Items[i];
			Button button = AddItem(currentIndex, item, ShopAction.Buy);
			if (i == 0)
			{
				firstItem = button;
			}
		}

		return firstItem;
	}

	private Button UpdateInventoryItems()
	{
		Button firstItem = null;
		shopHint.Hide();
		ClearItems();

		for (int i = 0; i < global.PlayerData.Inventory.Count; i++)
		{
			int currentIndex = i;
			string item = global.PlayerData.Inventory[i];
			Button button = AddItem(currentIndex, item, ShopAction.Sell);
			if (i == 0)
			{
				firstItem = button;
			}
		}

		return firstItem;
	}

	private void OnBuyButton()
	{
		level = 1;

		shopHint.Hide();	
		itemsContainer.Show();

		Button firstItem = UpdateShopItems();;
		firstItem.GrabFocus();
	}

	private void OnSellButton()
	{
		level = 1;

		shopHint.Hide();		
		itemsContainer.Show();

		Button firstItem = UpdateInventoryItems();
		if (firstItem != null)
		{
			firstItem.GrabFocus();
		}
		else
		{
			ShowDisplay(Shop.Name);
		}
	}

	private void OnItemButton(int index, string itemName, ShopAction shopAction)
	{
		Item item = global.ItemDescriptions[itemName];

		switch (shopAction)
		{
			case ShopAction.Buy:
				if (item.Price <= global.PlayerData.Gold && item.Sellable)
				{
					global.PlayerData.Gold -= item.Price;
					global.AddToInventory(itemName);
					UpdateDisplay();
					ShowDisplay();
					level = 0;
				}
				break;
			case ShopAction.Sell:
				global.PlayerData.Gold += item.Price;
				global.PlayerData.RemoveFromInventory(index);
				UpdateDisplay();
				ShowDisplay();
				level = 0;
				break;
		}
	}
}
