using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class GameDisplay : CanvasLayer
{
	private int level = 0;
	private bool visible = false;
	private Global global;
	private TextureProgressBar healthBar;
	private Button itemsButton;
	private AudioStreamPlayer audioPlayer;
	private InventoryDisplay inventoryMenu;
	private OptionsMenu optionsMenu;
	private ControlsMenu controlsMenu;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		healthBar = GetNode<TextureProgressBar>("%HealthBar");
		itemsButton = GetNode<Button>("%ItemsButton");
		audioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");
		optionsMenu = GetNode<OptionsMenu>("%OptionsMenu");
		controlsMenu = GetNode<ControlsMenu>("%ControlsMenu");
		inventoryMenu = GetNode<InventoryDisplay>("%InventoryMenu");

		inventoryMenu.Hide();
		Hide();
		optionsMenu.UpdateDisplay();
		controlsMenu.UpdateDisplay();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

    public override void _UnhandledInput(InputEvent @event)
    {
		if (!global.GameDisplayEnabled)
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
						Visible = false;
					}
					else
					{
						global.CanWalk = false;
						Show();
						healthBar.Set(TextureProgressBar.PropertyName.Value, global.PlayerData.Health);
						itemsButton.GrabFocus();
						Visible = true;

						inventoryMenu.UpdateDisplay();
					}
					break;
				case 1:
					level = 0;
					inventoryMenu.Hide();
					optionsMenu.Hide();
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

	public void ToggleInventoryMenu()
	{
		if (inventoryMenu.Visible)
		{
			itemsButton.GrabFocus();
		}
		else
		{
			level = 1;
			inventoryMenu.Show();
			inventoryMenu.FocusFirst();
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
		optionsMenu.Show();
		controlsMenu.Hide();
		optionsMenu.FocusFirst();
	}

	public void OnControlsMenu()
	{
		level = 2;
		optionsMenu.Hide();
		controlsMenu.Show();
		controlsMenu.FocusFirst();
	}
}
