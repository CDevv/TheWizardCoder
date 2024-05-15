using Godot;
using System;

public partial class CharacterRect : NinePatchRect
{
	private Sprite2D sprite;
	private Label nameLabel;
	private TextureProgressBar healthBar;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite");
		nameLabel = GetNode<Label>("NameLabel");
		healthBar = GetNode<TextureProgressBar>("HealthBar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetSpriteTexture(Texture2D resource)
	{
		sprite.Texture = resource;
	}

	public void SetNameText(string text)
	{
		nameLabel.Text = text;
	}

	public void SetHealthValue(int value)
	{
		healthBar.Value = value;
	}

	public void TweenHealthValue(int value)
	{
		Tween tween = healthBar.CreateTween();
		tween.TweenProperty(healthBar, "value", value, 0.2);
	}
}
