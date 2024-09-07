using Godot;
using Godot.Collections;
using System;

public partial class BattleOptions : Display
{
	[Export]
	public PackedScene ButtonTemplate { get; set; }

	[Signal]
	public delegate void FightButtonTriggeredEventHandler();
	[Signal]
	public delegate void ItemsButtonTriggeredEventHandler(int itemIndex);
	[Signal]
	public delegate void MagicButtonTriggeredEventHandler(int index);
	[Signal]
	public delegate void DefenseButtonTriggeredEventHandler();

	private Global global;
	private GridContainer optionsContainer;
	private Button attackButton;
	private NinePatchRect descriptionContainer;
	private Label itemDescription;
	private Label costLabel;
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
		descriptionContainer = GetNode<NinePatchRect>("%DescriptionContainer");
		itemDescription = GetNode<Label>("%Description");
		costLabel = GetNode<Label>("%CostLabel");
		itemsContainer = GetNode<GridContainer>("%Items");
		magicContainer = GetNode<GridContainer>("%Magic");
		infoLabel = GetNode<Label>("%InfoLabel");
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

    public override void ShowDisplay()
    {
		Show();
        ShowOptions();
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
		costLabel.Hide();
		itemsContainer.Show();

		Button button = itemsContainer.GetChildOrNull<Button>(0);
		if (button == null)
		{
			descriptionContainer.Hide();
			ShowInfoLabel("You have no items");			
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

		Button button = magicContainer.GetChildOrNull<Button>(0);
		if (button == null)
		{
			descriptionContainer.Hide();
			ShowInfoLabel("You have no magic spells");	
		}
		else
		{
			descriptionContainer.Show();
			costLabel.Show();
			button.GrabFocus();
		}
	}

    public override void UpdateDisplay()
    {
        UpdateDisplay(global.PlayerData.Stats);
    }

    public void UpdateDisplay(CharacterData characterData)
	{
		ClearContainers();

		for (int i = 0; i < global.PlayerData.Inventory.Count; i++)
		{
			int currentIndex = i;

			string item = global.PlayerData.Inventory[i];
			Button buttonItem = ButtonTemplate.Instantiate<Button>();
			buttonItem.Text = item;
			buttonItem.FocusEntered += () => {
				itemDescription.Text = global.ItemDescriptions[item].Description;
			};
			buttonItem.Pressed += () => {
				EmitSignal(SignalName.ItemsButtonTriggered, currentIndex);
			};
			itemsContainer.AddChild(buttonItem);
		}

		for (int i = 0; i < characterData.MagicSpells.Count; i++)
		{
			int currentIndex = i;
			string magicSpellName = characterData.MagicSpells[i];
			MagicSpell magicSpell = global.MagicSpells[magicSpellName];

			Button buttonItem = ButtonTemplate.Instantiate<Button>();
			buttonItem.Text = magicSpellName;
			buttonItem.Pressed += () => {
				EmitSignal(SignalName.MagicButtonTriggered, currentIndex);
			};

			if (characterData.Points >= magicSpell.Cost)
			{
				buttonItem.FocusEntered += () => {
					itemDescription.Text = global.MagicSpells[magicSpellName].Description;
					costLabel.Text = $"{global.MagicSpells[magicSpellName].Cost} MP";
				};
			}
			else
			{
				buttonItem.Disabled = true;
			}

			magicContainer.AddChild(buttonItem);
		}
	}

	private void ClearContainers()
	{
		Array<Node> itemButtons = itemsContainer.GetChildren();
		foreach (var item in itemButtons)
		{
			item.QueueFree();
		}

		Array<Node> magicSpellsButtons = magicContainer.GetChildren();
		foreach (var item in magicSpellsButtons)
		{
			item.QueueFree();
		}
	}

	public void OnAttackButton()
	{
		EmitSignal(SignalName.FightButtonTriggered);
	}

	public void OnItemsButton()
	{
		ShowItems();
	}

	public void OnMagicButton()
	{
		ShowMagicSpells();
	}

	public void OnDefenseButton()
	{
		EmitSignal(SignalName.DefenseButtonTriggered);
	}
}
