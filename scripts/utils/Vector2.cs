using Godot;
using System;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Utils
{
    public static class Vector2Helper
    {
        public static Vector2 ToVector(this Direction direction)
        {
            Vector2 resultVector = Vector2.Down;
			switch (direction)
			{
				case Direction.Up:
					resultVector = Vector2.Up;
					break;
				case Direction.Down:
					resultVector = Vector2.Down;
					break;
				case Direction.Left:
					resultVector = Vector2.Left;
					break;
				case Direction.Right:
					resultVector = Vector2.Right;
					break;
			}

			return resultVector;
        }

        public static Direction ToDirection(this Vector2 vector)
        {
            float radianAngle = vector.Angle();
			float angle = Mathf.RadToDeg(radianAngle);
			Direction direction = Direction.Down;
			
			switch (angle)
			{
				case 0:
					direction = Direction.Right;
					break;
				case 90:
					direction = Direction.Down;
					break;
				case 180:
					direction = Direction.Left;
					break;
				case -90:
					direction = Direction.Up;
					break;
				default:
					direction = Direction.Down;
					break;
			}
		
			return direction;
        }
    }
}