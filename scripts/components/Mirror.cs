using Godot;
using TheWizardCoder.Autoload;
using TheWizardCoder.Components;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Components
{
    public partial class Mirror : Node2D
    {
        private Global global;
        private AnimatedSprite2D playerSprite;
        private bool hasLastDirection = false;
        private Direction lastDirection;

        public override void _Ready()
        {
            global = GetNode<Global>("/root/Global");
            playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");

        }

        public override void _PhysicsProcess(double delta)
        {
            Player player = global.CurrentRoom.Player;

            float playerX = player.GlobalPosition.X;
            float playerY = player.GlobalPosition.Y;
            float mirrorY = GlobalPosition.Y;
            float distance = mirrorY - playerY;

            Vector2 newPosition = new(playerX, GlobalPosition.Y + (distance + 16));
            playerSprite.GlobalPosition = newPosition;

            Direction direction = player.Direction;

            if (player.Direction == Direction.Down)
            {
                direction = Direction.Up;
            }
            else if (player.Direction == Direction.Up)
            {
                direction = Direction.Down;
            }

            if (player.Velocity == Vector2.Zero)
            {
                playerSprite.Animation = "default";
                playerSprite.Frame = (int)direction;
            }
            else
            {
                playerSprite.Play(direction.ToString().ToLower());
            }
        }
    }

}