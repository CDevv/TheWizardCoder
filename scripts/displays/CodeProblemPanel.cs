using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class CodeProblemPanel : CanvasLayer
{
	[Signal]
	public delegate void ProblemSolvedEventHandler();

	[Export]
	public PackedScene PickableButtonScene { get; set; }
	[Export]
	public PackedScene PickableButtonAreaScene { get; set; }
	public string ProblemId { get; set; }
	public CodeProblemPoint Point { get; set; }

	private Global global;
	private CodeEdit codeEdit;
	private ColorRect baseListRect;
	private AnimationPlayer animationPlayer;
	private int itemCount = 0;
	private int areasCount = 0;
	private List<PickableButton> items = new List<PickableButton>();
	private List<PickableButtonArea> areas = new List<PickableButtonArea>();
	private List<string> correctAnswers = new List<string>();
	private List<bool> solvedAreas = new List<bool>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		codeEdit = GetNode<CodeEdit>("%CodeEdit");
		baseListRect = GetNode<ColorRect>("%BaseListRect");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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

	public void ShowDisplay(string code, Array<string> items, Godot.Collections.Dictionary<string, Vector2> areas)
	{
		global.GameDisplayEnabled = false;

		codeEdit.Text = code;
		foreach (string item in items)
		{
			AddItem(item);
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
		button.Text = item;
		Vector2 buttonPosition = new Vector2(baseListRect.GlobalPosition.X, baseListRect.GlobalPosition.Y + (itemCount * 32));
		button.GlobalPosition = buttonPosition;
		AddChild(button);
		items.Add(button);
		itemCount++;
	}
	
	private void AddArea(string correctAnswer, Vector2 position)
	{
		int currentIndex = areasCount;
		PickableButtonArea area = PickableButtonAreaScene.Instantiate<PickableButtonArea>();
		codeEdit.AddChild(area);
		area.Position = position;
		area.ButtonAdded += (string buttonText) => {
			if (correctAnswers[currentIndex] == buttonText)
			{
				solvedAreas[currentIndex] = true;
			}
			if (AreAllAreasSolved())
			{
				OnProblemSolved();
			}
		};
		area.ButtonRemoved += () => {
			solvedAreas[currentIndex] = false;
		};
		areas.Add(area);
		correctAnswers.Add(correctAnswer);
		solvedAreas.Add(false);
		areasCount++;
		GD.Print(areasCount);
	}

	public void Reset()
	{
		itemCount = 0;
		areasCount = 0;
		codeEdit.Text = "No code to see here :)";

		foreach (PickableButton item in items)
		{
			item.QueueFree();
		}
		items.Clear();

		foreach (PickableButtonArea area in areas)
		{
			area.QueueFree();
		}
		areas.Clear();
		correctAnswers.Clear();
	}

	private bool AreAllAreasSolved()
	{
		int solvedCount = 0;
		foreach (var item in solvedAreas)
		{
			if (item)
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
		Point.Solved = true;
		Point.EmitSignal(CodeProblemPoint.SignalName.ProblemSolved);
		EmitSignal(SignalName.ProblemSolved);
		Reset();
		Hide();
	}
}
