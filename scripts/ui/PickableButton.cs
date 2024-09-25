using Godot;
using System;
using TheWizardCoder.Autoload;

namespace TheWizardCoder.UI
{
	public partial class PickableButton : Area2D
	{
		private string text = String.Empty;

		public string Text
		{
			get { return text; }
		}

		public bool AreaIsDetected { get; set; } = false;
		public PickableButtonArea Area { get; set; }
		
		private Vector2 originalPosition;
		private bool isDragging = false;
		private Vector2 offset;
		private Global global;
		private Button button;

		public override void _Ready()
		{
			global = GetNode<Global>("/root/Global");
			button = GetNode<Button>("Button");
			originalPosition = button.GlobalPosition;
		}

		public override void _Input(InputEvent input)
		{
			if (input.GetType() == typeof(InputEventMouseMotion))
			{
				if (isDragging)
				{
					GlobalPosition = GetViewport().GetMousePosition() - offset;
				}
			}
		}

		public void SetText(string text)
		{
			this.text = text;
			button.Text = text;
		}

		private void OnButtonDown()
		{
			offset = GetViewport().GetMousePosition() - button.GlobalPosition;
			isDragging = true;
		}

		private void OnButtonUp()
		{
			isDragging = false;
			if (AreaIsDetected)
			{
				Tween tween = GetTree().CreateTween();
				tween.TweenProperty(this, "global_position", Area.GlobalPosition, 0.5);
				tween.Finished += () => {
					Area.EmitSignal(PickableButtonArea.SignalName.ButtonAdded, Text);
				};
				tween.Play();
			}
			else
			{
				Tween tween = GetTree().CreateTween();
				tween.TweenProperty(this, "global_position", originalPosition, 0.5);
				tween.Play();
			}
		}
	}
}