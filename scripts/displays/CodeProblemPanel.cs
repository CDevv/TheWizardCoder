using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.UI;
using TheWizardCoder.Interactables;

namespace TheWizardCoder.Displays
{
	public partial class CodeProblemPanel : Display
	{
		[Signal]
		public delegate void ProblemSolvedEventHandler();
		[Signal]
		public delegate void ProblemItemsChangedEventHandler();

		[Export]
		public PackedScene PickableButtonScene { get; set; }
		[Export]
		public PackedScene PickableButtonAreaScene { get; set; }
		public string ProblemId { get; set; }
		public CodeProblemPoint Point { get; set; }

		private CodeEdit codeEdit;
		private ColorRect baseListRect;
		private AnimationPlayer animationPlayer;
		private int itemCount = 0;
		private int areasCount = 0;
		private List<PickableButton> buttonItems = new List<PickableButton>();
		private List<PickableButtonArea> areas = new List<PickableButtonArea>();
		private List<CodeProblemItem> items = new List<CodeProblemItem>();

		public List<CodeProblemItem> ProblemItems 
		{
			get { return items; }
		}

		public override void _Ready()
		{
			base._Ready();
			codeEdit = GetNode<CodeEdit>("%CodeEdit");
			baseListRect = GetNode<ColorRect>("%BaseListRect");
			animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		}

		public override async void _Process(double delta)
		{
			if (Input.IsActionPressed("ui_cancel"))
			{
				if (Visible)
				{
					global.CanWalk = true;
					global.GameDisplayEnabled = true;
					animationPlayer.PlayBackwards("show");
					await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
					Reset();
					Hide();
				}
			}
		}

		public override void ShowDisplay()
		{
			Show();
		}

		public void ShowDisplay(string code, Array<string> items, Godot.Collections.Dictionary<string, Vector2> areas, bool useInventory)
		{
			global.GameDisplayEnabled = false;

			codeEdit.Text = code;

			foreach (string item in items)
			{
				AddItem(item);
			}
			if (useInventory)
			{
				foreach (string item in global.PlayerData.Inventory)
				{
					AddItem(item);
				}
			}

			foreach (var item in areas)
			{
				AddArea(item.Key, item.Value);
			}
			Show();
			animationPlayer.Play("show");
		}

		private void AddItem(string item)
		{
			PickableButton button = PickableButtonScene.Instantiate<PickableButton>();

			Vector2 buttonPosition = new Vector2(baseListRect.GlobalPosition.X, baseListRect.GlobalPosition.Y + (itemCount * 32));
			button.GlobalPosition = buttonPosition;
			AddChild(button);
			buttonItems.Add(button);
			button.SetText(item);
			itemCount++;
		}
		
		private void AddArea(string correctAnswer, Vector2 position)
		{
			int currentIndex = areasCount;
			PickableButtonArea area = PickableButtonAreaScene.Instantiate<PickableButtonArea>();
			codeEdit.AddChild(area);
			area.Position = position;
			area.ButtonAdded += (string buttonText) => OnAreaButtonAdded(currentIndex, buttonText);
			area.ButtonRemoved += () => OnAreaButtonRemoved(currentIndex);
			areas.Add(area);

			CodeProblemItem item = new CodeProblemItem();
			item.CurrentAnswer = "";
			item.CorrectAnswer = correctAnswer;
			item.IsSolved = false;
			items.Add(item);

			areasCount++;
			GD.Print(areasCount);
		}

		private void OnAreaButtonAdded(int index, string buttonText)
		{
			if (items[index].CorrectAnswer == buttonText)
			{
				items[index].CurrentAnswer = buttonText;
				items[index].IsSolved = true;
				EmitSignal(SignalName.ProblemItemsChanged);
			}
			if (AreAllAreasSolved())
			{
				OnProblemSolved();
			}
		}

		private void OnAreaButtonRemoved(int index)
		{
			if (!Point.Active)
			{
				if (items.Count > index)
				{
					items[index].CurrentAnswer = "";
					items[index].IsSolved = false;
					EmitSignal(SignalName.ProblemItemsChanged);
				}
			}
		}

		public void Reset()
		{
			itemCount = 0;
			areasCount = 0;
			codeEdit.Text = "No code to see here :)";

			foreach (PickableButtonArea area in areas)
			{
				area.QueueFree();
			}
			areas.Clear();

			foreach (PickableButton item in buttonItems)
			{
				item.QueueFree();
			}
			buttonItems.Clear();

			items.Clear();
		}

		private bool AreAllAreasSolved()
		{
			int solvedCount = 0;
			foreach (var item in items)
			{
				if (item.IsSolved)
				{
					solvedCount++;
				}
			}
			return solvedCount == areasCount;
		}

		private async void OnProblemSolved()
		{
			GD.Print("SOLVED!!");
			global.CanWalk = true;
			global.GameDisplayEnabled = true;
			animationPlayer.PlayBackwards("show");
			await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			EmitSignal(SignalName.ProblemSolved);
			Reset();
			Hide();
		}
	}
}