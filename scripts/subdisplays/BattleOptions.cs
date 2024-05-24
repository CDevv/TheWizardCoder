using Godot;
using System;

public partial class BattleOptions : NinePatchRect
{
	[Signal]
	public delegate void FightButtonTriggeredEventHandler();
	[Signal]
	public delegate void ItemsButtonTriggeredEventHandler();
	[Signal]
	public delegate void MagicButtonTriggeredEventHandler();
	[Signal]
	public delegate void DefenseButtonTriggeredEventHandler();

	private VBoxContainer optionsContainer;
	private Button fightButton;
	private Label itemDescription;

	public override void _Ready()
	{
		optionsContainer = GetNode<VBoxContainer>("%OptionsContainer");
		fightButton = GetNode<Button>("%FightButton");
		itemDescription = GetNode<Label>("%ItemDescription");
	}

	public void FocusFirst()
	{
		fightButton.GrabFocus();
	}

	public void ShowOptions()
	{
		optionsContainer.Show();
		itemDescription.Hide();
	}

	public void ShowItemDescription()
	{
		optionsContainer.Hide();
		itemDescription.Show();
	}

	public void SetItemDescription(string text)
	{
		itemDescription.Text = text;
	}

	public void OnFightButton()
	{
		EmitSignal(SignalName.FightButtonTriggered);
	}

	public void OnItemsButton()
	{
		EmitSignal(SignalName.ItemsButtonTriggered);
	}

	public void OnMagicButton()
	{
		EmitSignal(SignalName.MagicButtonTriggered);
	}

	public void OnDefenseButton()
	{
		EmitSignal(SignalName.DefenseButtonTriggered);
	}
}
