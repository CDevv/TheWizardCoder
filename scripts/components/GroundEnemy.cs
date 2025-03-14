using Godot;
using TheWizardCoder.Autoload;
using TheWizardCoder.Enums;
using TheWizardCoder.Interactables;
using TheWizardCoder.Utils;

namespace TheWizardCoder.Components
{
    public partial class GroundEnemy : Area2D
    {
        private const int PixelsPerSecond = 120; //120 pixels per frame
        private const float Speed = 0.25f;

        [Export]
        public string EnemyName { get; set; } = "Glitch";
        [Export]
        public Texture2D BackgroundImage { get; set; } = ResourceLoader.Load<Texture2D>("res://assets/battle/backgrounds/battle-bg.png");
        [Export]
        public string PlaythroughProperty { get; set; } = string.Empty;

        private Global global;
        private BattlePoint battlePoint;
        private AnimatedSprite2D sprite;
        private bool following;
        private Direction direction;

        public override void _Ready()
        {
            global = GetNode<Global>("/root/Global");
            battlePoint = GetNode<BattlePoint>("BattlePoint");
            sprite = GetNode<AnimatedSprite2D>("Sprite");

            battlePoint.EnemyName = EnemyName;
            battlePoint.BackgroundImage = BackgroundImage;

            SpriteFrames enemyTexture = ResourceLoader.Load<SpriteFrames>($"res://assets/enemies/spriteframes/{EnemyName}.tres");
            sprite.SpriteFrames = enemyTexture;

            if (!string.IsNullOrEmpty(PlaythroughProperty) && global.PlayerData.Get(PlaythroughProperty).AsBool())
            {
                QueueFree();
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (following)
            {
                Vector2 difference = (global.CurrentRoom.Player.Position - Position);
                Vector2 normalizedDifference = difference.Normalized();
                Vector2 velocity = normalizedDifference * PixelsPerSecond * (float)delta;
                Position += velocity;

                if (velocity.X > 0)
                {
                    sprite.FlipH = true;
                }
                else
                {
                    sprite.FlipH = false;
                }

                if (Vector2Helper.IsInOneDirection(velocity))
                {
                    direction = velocity.ToDirection();
                    PlayMoveAnimation(velocity);
                }
            }
            else
            {
                PlayIdleAnimation(direction);
            }
        }

        private void PlayIdleAnimation(Direction direction)
        {
            sprite.Animation = "default";
            sprite.Frame = (int)direction;
        }

        private void PlayMoveAnimation(Vector2 velocity)
        {
            sprite.Play(velocity.ToDirection().ToString().ToLower());
        }

        private void OnBodyEntered(Node2D body)
        {
            if (global.CurrentRoom.Player.Enemy == null && Visible)
            {
                if (body.GetType() == typeof(Player))
                {
                    global.CurrentRoom.BattleDisplay.BattleEnded += BattleEnded;
                    global.CurrentRoom.Player.Enemy = this;
                    following = true;

                    sprite.Play();
                }
            }
        }

        private void OnBodyExited(Node2D body)
        {
            if (body.GetType() == typeof(Player))
            {
                global.CurrentRoom.Player.Enemy = null;
                following = false;

                sprite.Stop();
                sprite.Frame = 0;
            }
        }

        private void BattleEnded()
        {
            battlePoint.Active = false;
            Visible = false;
            following = false;

            sprite.Stop();

            if (!string.IsNullOrEmpty(PlaythroughProperty))
            {
                global.PlayerData.Set(PlaythroughProperty, true);
            }
        }

        private void OnInteracted()
        {
            following = false;
        }
    }
}
