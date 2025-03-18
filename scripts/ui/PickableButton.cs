using Godot;
using Godot.Collections;
using System;
using TheWizardCoder.Autoload;

namespace TheWizardCoder.UI
{
    public partial class PickableButton : Area2D
    {
        [Signal]
        public delegate void TweenFinishedEventHandler();

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

        public void TweenDisappearance()
        {
            Tween tween = CreateTween();
            tween.TweenProperty(this, "modulate", new Color(20, 34, 20, 0), 1);
            tween.Finished += () =>
            {
                EmitSignal(SignalName.TweenFinished);
            };
        }

        private void OnButtonDown()
        {
            offset = GetViewport().GetMousePosition() - button.GlobalPosition;
            isDragging = true;
        }

        private void OnButtonUp()
        {
            if (Area != null)
            {
                Area.ButtonText = string.Empty;
                Area.Taken = false;

                Area.EmitSignal(PickableButtonArea.SignalName.ButtonRemoved);
            }

            isDragging = false;
            GetDetectedAreas();

            if (AreaIsDetected)
            {
                Tween tween = GetTree().CreateTween();
                tween.TweenProperty(this, "global_position", Area.GlobalPosition, 0.5);
                tween.Finished += () =>
                {
                    Area.Taken = true;
                    Area.ButtonText = text;
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

        private void GetDetectedAreas()
        {
            Array<Area2D> areas = GetOverlappingAreas();

            if (areas.Count > 0)
            {
                float min = int.MaxValue;

                foreach (Area2D area in areas)
                {
                    PickableButtonArea buttonArea = (PickableButtonArea)area;
                    float distance = Position.DistanceTo(buttonArea.Position);

                    if (!buttonArea.Taken && distance < min)
                    {
                        min = distance;

                        Area = buttonArea;
                        AreaIsDetected = true;
                    }
                }
            }
            else
            {
                AreaIsDetected = false;
            }
        }
    }
}