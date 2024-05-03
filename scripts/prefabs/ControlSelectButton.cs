using Godot;
using System;

public partial class ControlSelectButton : Button
{
	[Export]
	public string ActionName { get; set; }
	private InputEventKey selectedControl;
	private bool waitingInput = false;

	private Global global;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");

		InputEventKey eventKey = (InputEventKey)global.Settings.Controls[ActionName];
		Text = eventKey.AsTextKeycode();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if (waitingInput)
		{
			if (@event is InputEventKey && @event.IsPressed())
			{
				Text = ((InputEventKey)@event).AsTextKeycode();
				global.Settings.ChangeControl(ActionName, @event);
				global.Settings.SaveSettings();
				waitingInput = false;
			}
		}
    }

	public void OnPressed()
	{
		Text = "Waiting for input..";
		waitingInput = true;
	}
}
