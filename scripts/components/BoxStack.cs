using Godot;
using System.Collections.Generic;
using TheWizardCoder.Utils;

namespace TheWizardCoder.Components
{
    public partial class BoxStack : Node2D
    {
        private const int BoxGapY = 24;

        [Export]
        public PackedScene BoxScene { get; set; }

        private Stack<Sprite2D> boxes = new Stack<Sprite2D>();
        public int Count { get { return boxes.Count; } }

        public void AddBox()
        {
            Sprite2D box = BoxScene.Instantiate<Sprite2D>();
            box.Modulate = ColorHelper.GetRandom();

            if (boxes.Count > 0)
            {
                Vector2 newBoxPosition = boxes.Peek().Position + new Vector2(0, -BoxGapY);
                box.Position = newBoxPosition;
            }
            else
            {
                box.Position = Position - new Vector2(0, -16);
            }

            AddChild(box);
            boxes.Push(box);
        }

        public void RemoveBox()
        {
            boxes.Pop().QueueFree();
        }

        public void ClearBoxes()
        {
            foreach (var box in boxes)
            {
                box.QueueFree();
            }
            boxes.Clear();
        }
    }

}