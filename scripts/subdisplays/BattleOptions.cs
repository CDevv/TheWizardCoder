using Godot;
using System;

public partial class BattleOptions : NinePatchRect
{
	[Export]
	public PackedScene ButtonTemplate { get; set; }

	[Signal]
	public delegate void FightButtonTriggeredEventHandler();
	[Signal]
	public delegate void ItemsButtonTriggeredEventHandler();
	[Signal]
	public delegate void MagicButtonTriggeredEventHandler();
	[Signal]
	public delegate void DefenseButtonTriggeredEventHandler();

	private Global global;
	private GridContainer optionsContainer;
	private Button attackButton;
	private NinePatchRect descriptionContainer;
	private Label itemDescription;
	private GridContainer itemsContainer;
	private GridContainer magicContainer;
	private Label infoLabel;

	private bool isTyping = false;
	private double waitingTime = 0.005;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		optionsContainer = GetNode<GridContainer>("%MainOptions");
		attackButton = GetNode<Button>("%AttackButton");
		descriptionContainer = GetNode<NinePatchRect>("DescriptionContainer");
		itemDescription = GetNode<Label>("%Description");
		itemsContainer = GetNode<GridContainer>("%Items");
		magicContainer = GetNode<GridContainer>("%Magic");
		infoLabel = GetNode<Label>("InfoLabel");
	}

	public override void _Process(double delta)
	{
		if (isTyping)
		{
			if (waitingTime > 0)
			{
				waitingTime -= delta;
			}
			else
			{
				infoLabel.VisibleCharacters++;
				waitingTime = 0.05;
			}

			if (infoLabel.VisibleRatio == 1)
			{
				isTyping = false;
				infoLabel.VisibleCharacters = 0;
			}
		}
	}

	public void ShowInfoLabel(string text)
	{
		optionsContainer.Hide();
		descriptionContainer.Hide();
		itemsContainer.Hide();
		magicContainer.Hide();
		infoLabel.Text = text;
		//infoLabel.VisibleCharacters = 0;
		infoLabel.Show();
		//isTyping = true;
	}

	public void ShowOptions()
	{
		optionsContainer.Show();
		descriptionContainer.Hide();
		itemsContainer.Hide();
		magicContainer.Hide();
		infoLabel.Hide();

		attackButton.GrabFocus();
	}

	private void ShowItems()
	{
		optionsContainer.Hide();
		magicContainer.Hide();
		itemsContainer.Show();
		descriptionContainer.Show();

		Button button = itemsContainer.GetChildOrNull<Button>(1);
		if (button == null)
		{
			descriptionContainer.Hide();			
		}
		else
		{
			button.GrabFocus();
			descriptionContainer.Show();
		}
	}

	private void ShowMagicSpells()
	{
		optionsContainer.Hide();
		itemsContainer.Hide();
		magicContainer.Show();
		descriptionContainer.Show();

		Button button = magicContainer.GetChild<Button>(0);
		button.GrabFocus();
	}

	public void UpdateDisplay()
	{
		foreach (string item in global.PlayerData.Inventory)
		{
			Button buttonItem = ButtonTemplate.Instantiate<Button>();
			buttonItem.Text = item;
			buttonItem.FocusEntered += () => {
				itemDescription.Text = global.ItemDescriptions[item].Description;
			};
			itemsContainer.AddChild(buttonItem);
		}

		foreach (string item in global.PlayerData.MagicSpells)
		{
			Button buttonItem = ButtonTemplate.Instantiate<Button>();
			buttonItem.Text = item;
			magicContainer.AddChild(buttonItem);
		}
	}

	public void OnAttackButton()
	{
		EmitSignal(SignalName.FightButtonTriggered);
	}

	public void OnItemsButton()
	{
		ShowItems();
		EmitSignal(SignalName.ItemsButtonTriggered);
	}

	public void OnMagicButton()
	{
		ShowMagicSpells();
		EmitSignal(SignalName.MagicButtonTriggered);
	}

	public void OnDefenseButton()
	{
		EmitSignal(SignalName.DefenseButtonTriggered);
	}
}
