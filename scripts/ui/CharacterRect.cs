using Godot;
using System;
using System.Threading.Tasks;
using TheWizardCoder.Data;

namespace TheWizardCoder.UI
{
	public partial class CharacterRect : NinePatchRect
	{
		[Signal]
		public delegate void PressedEventHandler();

		private Color currentCharacterColor = new Color(255, 255, 255, 0.2f);
		private Color invisibleColor = new Color(255, 255, 255, 0);
		private Color focusColor = new Color(255, 255, 255, 0.5f);
		private AnimatedSprite2D sprite;
		private Label nameLabel;
		private Label healthLabel;
		private Label pointsLabel;
		private ColorRect background;
		private AnimatedSprite2D selectArrow;
		private bool focused = false;

		public override void _Ready()
		{
			FocusMode = FocusModeEnum.All;
			sprite = GetNode<AnimatedSprite2D>("Sprite");
			nameLabel = GetNode<Label>("NameLabel");
			background = GetNode<ColorRect>("BackgroundColor");
			healthLabel = GetNode<Label>("HealthLabel");
			pointsLabel = GetNode<Label>("PointsLabel");
			selectArrow = GetNode<AnimatedSprite2D>("SelectArrow");
		}

		private void OnGuiInput(InputEvent inputEvent)
		{
			if (Input.IsActionJustPressed("ui_accept") && focused)
			{
				EmitSignal(SignalName.Pressed);
			}
		}

		private void SetSpriteTexture(string characterName)
		{
			sprite.Play(characterName);
		}

		private void SetNameText(string text)
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
			SetNameText(data.Name);
			SetHealthValue(data.Health);
			SetPointsValue(data.Points);
			SetSpriteTexture(data.Name);
		}

		public void ShowAsCurrentCharacter()
		{
			background.Modulate = currentCharacterColor;
		}

		public void HideBackground()
		{
			background.Modulate = invisibleColor;
		}

		private void OnFocusEntered()
		{
			focused = true;
			background.Modulate = focusColor;
			selectArrow.Show();
			selectArrow.Play("default");
		}

		private void OnFocusExited()
		{
			focused = false;
			background.Modulate = invisibleColor;
			selectArrow.Hide();
			selectArrow.Stop();
		}

		public async Task TweenDamage(Color color)
		{
			Tween tween = CreateTween();
			tween.TweenProperty(background, "modulate", new Color(color.R, color.G, color.B, 0.5f), 0.3);
			tween.TweenProperty(background, "modulate", new Color(color.R, color.G, color.B, 0), 0.3);
			SceneTreeTimer timer = GetTree().CreateTimer(2);
			await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
			tween.Stop();
		}
	}
}