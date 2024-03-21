using Godot;
using System;

public partial class GameDisplay : CanvasLayer
{
	private bool visible = false;
	private Global global;
	private TextureProgressBar healthBar;
	private Button itemsButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		healthBar = GetNode<TextureProgressBar>("%HealthBar");
		itemsButton = GetNode<Button>("%ItemsButton");
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
			if (visible)
			{
				global.CanWalk = true;
				Hide();
				visible = false;
			}
			else
			{
				global.CanWalk = false;
				Show();
				healthBar.Set(TextureProgressBar.PropertyName.Value, global.Health);
				itemsButton.GrabFocus();
				visible = true;
			}
		}
    }
}
