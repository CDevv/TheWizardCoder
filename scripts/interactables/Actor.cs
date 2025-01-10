using Godot;
using System;
using DialogueManagerRuntime;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;
using TheWizardCoder.Components;
using TheWizardCoder.Data;
using System.Collections.Generic;
using System.Linq;
using TheWizardCoder.Utils;
using System.Threading.Tasks;
using TheWizardCoder.Autoload;

namespace TheWizardCoder.Interactables
{
	public partial class Actor : CharacterBody2D
	{
		private const int PixelsPerSecond = 120; //120 pixels per frame

		[Signal]
		public delegate void FinishedWalkingEventHandler();

		[Export]
		public Resource DialogueResource { get; set; }
		[Export]
		public string DialogueTitle { get; set; }
		[Export]
		public Direction DefaultDirection { get; set; } = Direction.Down;

		private Global global;
		private ActorInteractable interactable;
        private CollisionShape2D collision;
        private AnimatedSprite2D sprite;
		private bool followingPlayer;	
		private Queue<CharacterPathway> pathways = new();
		private bool walkingToPoint = false;
		private Vector2 targetPoint = Vector2.Zero;

		public AnimatedSprite2D Sprite { get { return sprite; } }
		public bool FollowingPlayer { get { return followingPlayer; } }
		public int Speed { get; set; } = 2;

        public override void _Ready()
		{
			global = GetNode<Global>("/root/Global");

			interactable = GetNode<ActorInteractable>("Interactable");
			collision = GetNode<CollisionShape2D>("BodyCollision");
			sprite = GetNode<AnimatedSprite2D>("Sprite");

			sprite.Animation = "default";
			sprite.Frame = (int)DefaultDirection;

			interactable.DialogueResource = DialogueResource;
			interactable.DialogueTitle = DialogueTitle;
			interactable.DefaultDirection = DefaultDirection;
			interactable.Sprite = sprite;

			
		}

        public override void _PhysicsProcess(double delta)
        {
			Speed = global.CurrentRoom.Player.PlayerSpeed;

            if (walkingToPoint)
            {
                if (Position.DistanceTo(targetPoint) < 1)
                {
					walkingToPoint = false;
					EmitSignal(SignalName.FinishedWalking);
					return;
                }

				Vector2 difference = targetPoint - Position;
                Vector2 normalizedDifference = difference.Normalized();
				Vector2 velocity = normalizedDifference * PixelsPerSecond * (float)delta;
                
				MoveAndCollide(velocity);
            }

            if (followingPlayer && global.CurrentRoom.Player.DistanceWalked >= 32)
            {
				FollowPlayer();
            }
        }

        public void MakeFollower()
		{
			EnableFollowing();

			CharacterPathway pathway = new(global.CurrentRoom.Player.Direction, GlobalPosition, global.CurrentRoom.Player.PlayerSpeed);
			pathways.Enqueue(pathway);

			global.CurrentRoom.Player.Follower = this;
		}

		public void EnableFollowing()
		{
			interactable.Active = false;
			followingPlayer = true;
			collision.Disabled = true;
		}

		public void DisableFollowing()
		{
			interactable.Active = true;
			followingPlayer = false;
			collision.Disabled = false;

			pathways = new();
		}

		public void AddPathwayPoint(Direction direction, Vector2 point, int speed)
		{
			CharacterPathway pathway = new(direction, point, speed);
			pathways.Enqueue(pathway);
		}

		public void AddPathwayPoint()
		{
			AddPathwayPoint(DefaultDirection, Position, 2);
		}

		public void FollowPlayer()
		{
            if (pathways.Count > 0 && global.CurrentRoom.Player.Velocity != Vector2.Zero)
            {
                CharacterPathway lastPathway = pathways.Peek();
                Vector2 lastPos = lastPathway.Position;
                Vector2 difference = lastPos - Position;
                Vector2 velocity = difference.Normalized() * lastPathway.Speed;

                MoveAndCollide(velocity);

                PlayAnimation(lastPathway.Direction);

                if (lastPos.DistanceTo(Position) < 1)
                {
                    pathways.Dequeue();
                }
            }
        }

		public CharacterPathway PeekPathway()
		{
			return pathways.Peek();
		}

		public async Task WalkToPoint(Vector2 point)
		{
			walkingToPoint = true;
			targetPoint = point;

            Vector2 difference = point - Position;

            if (Math.Abs(difference.X) <= 1) difference.X = 0;
            if (Math.Abs(difference.Y) <= 1) difference.Y = 0;

            Vector2 normalizedDifference = difference.Normalized();
            Direction targetDirection = normalizedDifference.ToDirection();

            PlayAnimation(targetDirection);
            await ToSignal(this, SignalName.FinishedWalking);
            PlayIdleAnimation(targetDirection);
        }

        public void PlayAnimation(string name)
		{
			sprite.Play(name);
		}

		public void PlayAnimation(Direction direction)
		{
			PlayAnimation(direction.ToString().ToLower());
		}

		public void PlayIdleAnimation(Direction direction)
		{
			sprite.Stop();
			sprite.Animation = "default";
			sprite.Frame = (int)direction;
		}
	}
}