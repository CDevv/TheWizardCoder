using Godot;
using System;

public partial class EnemySprite : Sprite2D
{
	[Signal]
	public delegate void ButtonPressedEventHandler();

	private AnimatedSprite2D selectionArrow;
	private Button button;
	private Label nameLabel;
	public override void _Ready()
	{
		selectionArrow = GetNode<AnimatedSprite2D>("SelectionArrow");
		button = GetNode<Button>("Button");
		nameLabel = GetNode<Label>("Label");
	}

	public void ApplyData(CharacterData data)
	{
		nameLabel.Text = data.Name;
		Texture = (Texture2D)ResourceLoader.LoadThreadedGet($"res://assets/battle/enemies/{data.Name}.png");
		GD.Print($"res://assets/battle/enemies/{data.Name}.png");
	}

	public void GrabFocus()
	{
		selectionArrow.Show();
		selectionArrow.Play("default");
		nameLabel.Show();
		button.GrabFocus();
	}

	public void Unselect()
	{
		selectionArrow.Stop();
		selectionArrow.Hide();
		nameLabel.Hide();
	}

	private void OnButtonPressed()
	{
		EmitSignal(SignalName.ButtonPressed);
	}

	private void OnFocusExited()
	{
		Unselect();
	}
}
