using Godot;
using System;

public partial class PickableButtonArea : Area2D
{
	[Signal]
	public delegate void ButtonAddedEventHandler(string buttonText);
	[Signal]
	public delegate void ButtonRemovedEventHandler();

	public string ButtonText { get; set; }
	public bool Taken { get; set; } = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnButtonEntered(Area2D area)
	{
		if (Visible && !Taken)
		{
			if (area.GetType() == typeof(PickableButton))
			{
				PickableButton button = (PickableButton)area;
				button.AreaIsDetected = true;
				button.Area = this;
				Taken = true;
				ButtonText = button.Text;
				//EmitSignal(SignalName.ButtonAdded, button.Text);
			}
		}
	}

	public void OnButtonExited(Area2D area)
	{
		if (area.GetType() == typeof(PickableButton))
		{
			PickableButton button = (PickableButton)area;
			button.AreaIsDetected = false;
			if (ButtonText == button.Text)
			{
				EmitSignal(SignalName.ButtonRemoved);
				Taken = false;
			}
		}
	}
}
