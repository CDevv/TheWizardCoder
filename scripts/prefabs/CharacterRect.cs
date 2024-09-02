using Godot;
using System;
using System.Threading.Tasks;

public partial class CharacterRect : NinePatchRect
{
	private Sprite2D sprite;
	private Label nameLabel;
	private Label healthLabel;
	private Label pointsLabel;
	private ColorRect background;
	private DamageIndicator damageIndicator;

    public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite");
		nameLabel = GetNode<Label>("NameLabel");
		background = GetNode<ColorRect>("BackgroundColor");
		healthLabel = GetNode<Label>("HealthLabel");
		pointsLabel = GetNode<Label>("PointsLabel");
		damageIndicator = GetNode<DamageIndicator>("DamageIndicator");
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
		healthLabel.Text = $"{value} HP";
	}

	public void SetPointsValue(int value)
	{
		pointsLabel.Text = $"{value} MP";
	}

	public void ApplyData(CharacterData data)
	{
		SetHealthValue(data.Health);
		SetPointsValue(data.Points);
	}

	public async Task TweenDamage()
	{
		damageIndicator.Position = Position;
		damageIndicator.PlayAnimation();

		Tween tween = CreateTween();
		tween.TweenProperty(background, "modulate", new Color(255, 255, 255, 0.5f), 0.3);
		tween.TweenProperty(background, "modulate", new Color(255, 255, 255, 0), 0.3);
		await ToSignal(tween, PropertyTweener.SignalName.Finished);
		tween.Stop();
	}
}
