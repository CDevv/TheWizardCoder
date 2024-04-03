using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class GameDisplay : CanvasLayer
{
	[Export]
	public PackedScene ItemButtonTemplate;

	private int level = 0;
	private bool visible = false;
	private Global global;
	private TextureProgressBar healthBar;
	private Button itemsButton;
	private AudioStreamPlayer audioPlayer;
	private NinePatchRect inventoryMenu;
	private GridContainer itemsContainer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		healthBar = GetNode<TextureProgressBar>("%HealthBar");
		itemsButton = GetNode<Button>("%ItemsButton");
		audioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");
		inventoryMenu = GetNode<NinePatchRect>("InventoryRect");
		itemsContainer = GetNode<GridContainer>("%ItemsContainer");

		inventoryMenu.Hide();
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
		{
			switch (level)
			{
				case 0:
					if (Visible)
					{
						global.CanWalk = true;
						Hide();
						Visible = false;
					}
					else
					{
						global.CanWalk = false;
						Show();
						healthBar.Set(TextureProgressBar.PropertyName.Value, global.Health);
						itemsButton.GrabFocus();
						Visible = true;

						UpdateInventoryMenu();
					}
					break;
				case 1:
					ToggleInventoryMenu();
					break;
				default:
					break;
			}
		}
		if (Input.IsKeyPressed(Key.Q))
		{
			var savedGamesDisplay = global.CurrentRoom.SavedGamesDisplay;
			savedGamesDisplay.Show();
			savedGamesDisplay.FocusOnFirstSave();
		}
    }

	public void ToggleInventoryMenu()
	{
		if (inventoryMenu.Visible)
		{
			level = 0;
			inventoryMenu.Hide();
			itemsButton.GrabFocus();
		}
		else
		{
			level = 1;
			inventoryMenu.Show();

			Array<Node> items = itemsContainer.GetChildren();
			if (items.Count > 0)
			{
				((Button)items[0]).GrabFocus();
			}
			else
			{
				inventoryMenu.GrabFocus();
			}
		}
	}

	public void ToggleOptionsMenu()
	{
		//TODO
	}

	public void UpdateInventoryMenu()
	{
		Array<Node> oldNodes = itemsContainer.GetChildren();
		foreach (var item in oldNodes)
		{
			item.QueueFree();
		}

		foreach (var item in global.Inventory)
		{
			Button button = ItemButtonTemplate.Instantiate<Button>();
			button.Set(Button.PropertyName.Text, item);
			itemsContainer.AddChild(button);
		}
	}
}
