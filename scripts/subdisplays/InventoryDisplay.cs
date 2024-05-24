using Godot;
using Godot.Collections;
using System;

public partial class InventoryDisplay : CanvasLayer
{
	[Export]
	public PackedScene ItemButtonTemplate { get; set; }
	private Global global;
	private GridContainer itemsContainer;
	private Button noItemsButton;
	private NinePatchRect descriptionRect;
	private Label itemDescription;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		itemsContainer = GetNode<GridContainer>("%ItemsContainer");
		noItemsButton = GetNode<Button>("%NoItemsButton");
		descriptionRect = GetNode<NinePatchRect>("%DescriptionRect");
		itemDescription = GetNode<Label>("%ItemDescription");
	}

	public void FocusFirst()
	{
		descriptionRect.Show();
		noItemsButton.Hide();

		Array<Node> items = itemsContainer.GetChildren();
		if (items.Count > 0)
		{
			((Button)items[0]).GrabFocus();
		}
		else
		{
			descriptionRect.Hide();
			noItemsButton.Show();
			noItemsButton.GrabFocus();
		}
	}

	public void UpdateDisplay()
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
				itemDescription.Text = global.ItemDescriptions[item].Description;
			};
			itemsContainer.AddChild(button);
		}
	}
}
