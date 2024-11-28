using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Interactables
{
	public partial class TextBoxInteractable : Interactable
	{
		private const int BoxSpeed = 2;

		[Signal]
		public delegate void TouchedEventHandler();

		private CollisionShape2D collisionShape;
		private ConsoleBoxText consoleBoxText;
		private VisibleOnScreenNotifier2D onScreenNotifier;

		public override void _Ready()
		{
			collisionShape = GetNode<CollisionShape2D>("CollisionShape");
			consoleBoxText = GetNode<ConsoleBoxText>("ConsoleBoxText");
			onScreenNotifier = GetNode<VisibleOnScreenNotifier2D>("OnScreenNotifier");
		}

		public override void _PhysicsProcess(double delta)
		{
			Vector2 velocity = new Vector2(-BoxSpeed, 0);
			Position += velocity;
		}

		public override void Action()
		{
			EmitSignal(SignalName.Touched);
			QueueFree();
		}

		private void OnBoxResized()
		{
			Vector2 shapeSize = new Vector2(consoleBoxText.Size.X, consoleBoxText.Size.Y);

			RectangleShape2D newShape = new();
			newShape.Size = shapeSize;
			collisionShape.Shape = newShape;

			collisionShape.Position = shapeSize;

			onScreenNotifier.Position = shapeSize;

			Rect2 rect = new();
			rect.Size = shapeSize;
			onScreenNotifier.Rect = rect;
		}

		private void OnScreenExited()
		{
			QueueFree();
		}
	}
}