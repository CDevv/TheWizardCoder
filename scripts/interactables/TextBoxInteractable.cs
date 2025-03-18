using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.UI;

namespace TheWizardCoder.Interactables
{
    public partial class TextBoxInteractable : Interactable
    {
        private const int BoxSpeed = 2;

        [Signal]
        public delegate void TouchedEventHandler();

        public string Text { get; set; }
        public float HorizontalPositionLimit { get; set; }

        private CollisionShape2D collisionShape;
        private ConsoleBoxText consoleBoxText;
        private VisibleOnScreenNotifier2D onScreenNotifier;

        public override void _Ready()
        {
            collisionShape = GetNode<CollisionShape2D>("CollisionShape");
            consoleBoxText = GetNode<ConsoleBoxText>("ConsoleBoxText");
            onScreenNotifier = GetNode<VisibleOnScreenNotifier2D>("OnScreenNotifier");

            consoleBoxText.Text = Text;
        }

        public override void _PhysicsProcess(double delta)
        {
            Vector2 velocity = new(-BoxSpeed, 0);
            Vector2 newPosition = Position + velocity;

            if (newPosition.X > HorizontalPositionLimit)
            {
                Position += velocity;
            }
        }

        public override void Action()
        {
            EmitSignal(SignalName.Touched);
            QueueFree();
        }

        public void SetText(string text)
        {
            Text = text;
            consoleBoxText.Text = text;
            OnBoxResized();
        }

        private void OnBoxResized()
        {
            if (consoleBoxText != null)
            {
                Vector2 shapeSize = new(consoleBoxText.Size.X, consoleBoxText.Size.Y);

                RectangleShape2D newShape = new();
                newShape.Size = shapeSize;
                collisionShape.Shape = newShape;

                collisionShape.Position = shapeSize;

                onScreenNotifier.Position = shapeSize;

                Rect2 rect = new();
                rect.Size = shapeSize;
                onScreenNotifier.Rect = rect;
            }
        }

        private void OnScreenExited()
        {
            QueueFree();
        }
    }
}