using Godot;
using System;

public partial class ControlSelectButton : Button
{
	[Export]
	public string ActionName { get; set; }
	private InputEventKey selectedControl;
	private bool waitingInput = false;
	private Global global;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");

		InputEventKey eventKey = (InputEventKey)global.Settings.Controls[ActionName];
		Text = eventKey.AsTextKeycode();
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
