using Godot;
using System;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class CharacterPathway
    {
        public Direction Direction { get; set; }
        public Vector2 Position { get; set; }
        public int Speed { get; set; }

        public CharacterPathway(Direction direction, Vector2 position, int speed)
        {
            Direction = direction;
            Position = position;
            Speed = speed;
        }
    }
}