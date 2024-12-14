using Godot;
using System;

namespace TheWizardCoder.UI
{
	public partial class CustomCheckbox : Control
	{
		[Signal]
		public delegate void ToggledEventHandler(bool toggledOn);

		public bool IsSelected { get; private set; } = false;

		private Sprite2D sprite;
		private Color unfocusedColor = new(255, 255, 255, 1);
		private Color focusedColor = new(255, 255, 255, 0.7f);
		private Texture2D unselectedSprite;
		private Texture2D selectedSprite;

		public override void _Ready()
		{
			unselectedSprite = ResourceLoader.Load<Texture2D>("res://assets/ui/checkbox-unchecked.png");
			selectedSprite = ResourceLoader.Load<Texture2D>("res://assets/ui/checkbox-checked.png");

			sprite = GetNode<Sprite2D>("Sprite");
		}

		public override void _Draw()
		{
			if (HasFocus())
			{
				sprite.Modulate = focusedColor;
			}
			else
			{
				sprite.Modulate = unfocusedColor;
			}
		}

		public override void _GuiInput(InputEvent @event)
		{
			if (Input.IsActionJustPressed("ui_accept"))
			{
				IsSelected = !IsSelected;

				if (IsSelected)
				{
					sprite.Texture = selectedSprite;
				}
				else
				{
					sprite.Texture = unselectedSprite;
				}

				EmitSignal(SignalName.Toggled, IsSelected);
			}
		}

		public void SetToggled(bool toggled)
		{
			IsSelected = toggled;

			if (IsSelected)
				{
					sprite.Texture = selectedSprite;
				}
				else
				{
					sprite.Texture = unselectedSprite;
				}
		}
	}
}
