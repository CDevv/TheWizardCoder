using Godot;
using Godot.Collections;
using System;
using TheWizardCoder.Autoload;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using System.Linq;

namespace TheWizardCoder.Subdisplays
{
	public partial class BattleOptions : Display
	{
		const int ItemsPerPage = 4;

		[Export]
		public PackedScene ButtonTemplate { get; set; }

		[Signal]
		public delegate void FightButtonTriggeredEventHandler();
		[Signal]
		public delegate void ItemsButtonTriggeredEventHandler(int itemIndex, string itemName);
		[Signal]
		public delegate void MagicButtonTriggeredEventHandler(int index);
		[Signal]
		public delegate void DefenseButtonTriggeredEventHandler();

		private GridContainer optionsContainer;
		private Button attackButton;
		private NinePatchRect descriptionContainer;
		private Label itemDescription;
		private Label costLabel;
		private GridContainer itemsContainer;
		private GridContainer magicContainer;
		private Label infoLabel;
		private NinePatchRect pagingRect;
		private Label pageLabel;
		private Button prevButton;
		private Button nextButton;

		private bool isTyping = false;
		private double waitingTime = 0.005;
		private int currentPage = 0;
		private int lastPage = 0;

		public override void _Ready()
		{
			base._Ready();
			optionsContainer = GetNode<GridContainer>("%MainOptions");
			attackButton = GetNode<Button>("%AttackButton");
			descriptionContainer = GetNode<NinePatchRect>("%DescriptionContainer");
			itemDescription = GetNode<Label>("%Description");
			costLabel = GetNode<Label>("%CostLabel");
			itemsContainer = GetNode<GridContainer>("%Items");
			magicContainer = GetNode<GridContainer>("%Magic");
			infoLabel = GetNode<Label>("%InfoLabel");

			pagingRect = GetNode<NinePatchRect>("%PagingRect");
			pageLabel = GetNode<Label>("%PageLabel");

			prevButton = GetNode<Button>("PrevPageButton");
			nextButton = GetNode<Button>("NextPageButton");
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

		private void CalculateLastPage()
		{
			lastPage = global.PlayerData.Inventory.Count / ItemsPerPage;

			if (global.PlayerData.Inventory.Count % ItemsPerPage > 0)
			{
				lastPage++;
			}
		}

		public override void ShowDisplay()
		{
			CalculateLastPage();

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
			infoLabel.Show();
		}

		public void ShowOptions()
		{
			optionsContainer.Show();
			descriptionContainer.Hide();
			itemsContainer.Hide();
			magicContainer.Hide();
			infoLabel.Hide();

			pagingRect.Hide();

			attackButton.GrabFocus();
		}

		private void ShowItems()
		{
			currentPage = 0;
			optionsContainer.Hide();
			magicContainer.Hide();
			costLabel.Hide();
			itemsContainer.Show();

			CalculateLastPage();
			ClearInventoryContainer();
			Button firstItem = PopulateInventory();

			if (firstItem == null)
			{
				descriptionContainer.Hide();
				ShowInfoLabel("You have no items");			
			}
			else
			{
				firstItem.GrabFocus();
				descriptionContainer.Show();
			}

			if (global.PlayerData.Inventory.Count > ItemsPerPage)
			{
				pagingRect.Show();
				pageLabel.Text = $"{currentPage + 1}/{lastPage}";
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

			PopulateInventory();
			PopulateMagicSpells(characterData);
		}

		private Button PopulateInventory()
		{
			if (global.PlayerData.Inventory.Count == 0) return null;

			string[] availableItems = global.PlayerData.Inventory.Where((itemName) =>
			{
				Item item = global.ItemDescriptions[itemName];
				return (!(item.Type == Enums.ItemType.Key));
			}).ToArray();

			Button firstButton = null;
			for (int i = currentPage * ItemsPerPage; i < (currentPage + 1) * ItemsPerPage; i++)
			{
				int currentIndex = i;
				if (currentIndex >= global.PlayerData.Inventory.Count)
				{
					break;
				}

				string item = global.PlayerData.Inventory[i];
				Button buttonItem = ButtonTemplate.Instantiate<Button>();
				buttonItem.Text = item;
				buttonItem.FocusEntered += () => {
					itemDescription.Text = global.ItemDescriptions[item].Description;
				};
				buttonItem.Pressed += () => {
					EmitSignal(SignalName.ItemsButtonTriggered, currentIndex, item);
				};

				if (global.PlayerData.Inventory.Count > ItemsPerPage)
				{
					if (i % 2 == 0)
					{
						buttonItem.FocusNeighborLeft = prevButton.GetPath();
					}
					else
					{
						buttonItem.FocusNeighborRight = nextButton.GetPath();
					}
				}

				itemsContainer.AddChild(buttonItem);

				if (firstButton == null)
				{
					firstButton = buttonItem;
				}
			}

			return firstButton;
		}

		private void PopulateMagicSpells(CharacterData characterData)
		{
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
			ClearInventoryContainer();

			Array<Node> magicSpellsButtons = magicContainer.GetChildren();
			foreach (var item in magicSpellsButtons)
			{
				item.QueueFree();
			}
		}

		private void ClearInventoryContainer()
		{
			Array<Node> itemButtons = itemsContainer.GetChildren();
			foreach (var item in itemButtons)
			{
				item.QueueFree();
			}
		}

		public void OnAttackButton()
		{
			if (!global.CurrentRoom.Dialogue.Visible)
			{
				EmitSignal(SignalName.FightButtonTriggered);
			}
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
			if (!global.CurrentRoom.Dialogue.Visible)
			{
				EmitSignal(SignalName.DefenseButtonTriggered);
			}
		}

		private void OnPrevButton()
		{
			if (currentPage > 0)
			{
				currentPage--;
				ClearContainers();
				Button firstItem = PopulateInventory();

				firstItem.CallDeferred(Button.MethodName.GrabFocus);

				pageLabel.Text = $"{currentPage + 1}/{lastPage}";
			}
			else
			{
				Button button = itemsContainer.GetChildOrNull<Button>(0);
				button.CallDeferred(Button.MethodName.GrabFocus);
			}
		}

		private void OnNextButton()
		{
			if (currentPage + 1 < lastPage)
			{
				currentPage++;
				ClearContainers();
				Button firstItem = PopulateInventory();

				firstItem.CallDeferred(Button.MethodName.GrabFocus);

				pageLabel.Text = $"{currentPage + 1}/{lastPage}";
			}
			else
			{
				Button button = itemsContainer.GetChildOrNull<Button>(0);
				button.CallDeferred(Button.MethodName.GrabFocus);
			}
		}
	}
}