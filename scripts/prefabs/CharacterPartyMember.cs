using Godot;
using System;
using System.Text;

public partial class CharacterPartyMember : NinePatchRect
{
	[Signal]
	public delegate void PressedEventHandler();

	private Global global;
	private AnimatedSprite2D sprite;
	private Label name;
	private Label mainStats;
	private Label secondaryStats;
	private AnimatedSprite2D selectArrow;
	private ColorRect background;
	private bool focused = false;

	public override void _Ready()
	{
		FocusMode = FocusModeEnum.All;
		global = GetNode<Global>("/root/Global");
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		name = GetNode<Label>("NameLabel");
		mainStats = GetNode<Label>("MainStats");
		secondaryStats = GetNode<Label>("SecondaryStats");
		selectArrow = GetNode<AnimatedSprite2D>("SelectArrow");
		background = GetNode<ColorRect>("Background");
	}

	public override void _Process(double delta)
	{
	}

	public void ApplyData(CharacterData data)
	{
		name.Text = data.Name;
		
		StringBuilder mainStatsBuilder = new();
		mainStatsBuilder.AppendLine($"HP: {data.Health}");
		mainStatsBuilder.AppendLine($"MP: {data.Points}");
		mainStats.Text = mainStatsBuilder.ToString();

		StringBuilder secondaryStatsBuilder = new();
		secondaryStatsBuilder.AppendLine($"ATK: {data.AttackPoints}");
		secondaryStatsBuilder.AppendLine($"DEF: {data.DefensePoints}");
		secondaryStats.Text = secondaryStatsBuilder.ToString();
	}

	private void OnFocusEntered()
	{
		focused = true;
		background.Modulate = new Color(255, 255, 255, 0.3f);
		selectArrow.Show();
		selectArrow.Play("default");
	}

	private void OnFocusExited()
	{
		focused = false;
		background.Modulate = new Color(255, 255, 255, 0);
		selectArrow.Stop();
		selectArrow.Hide();
	}

	private void OnGuiInput(InputEvent inputEvent)
	{
		if (Input.IsActionJustPressed("ui_accept") && focused)
		{
			EmitSignal(SignalName.Pressed);
		}
	}
}
