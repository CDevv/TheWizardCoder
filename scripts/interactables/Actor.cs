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

namespace TheWizardCoder.Interactables
{
	public partial class Actor : Interactable
	{
		[Signal]
		public delegate void FinishedWalkingEventHandler();

		[Export]
		public Resource DialogueResource { get; set; }
		[Export]
		public string DialogueTitle { get; set; }
		[Export]
		public Direction DefaultDirection { get; set; } = Direction.Down;

		private AnimatedSprite2D sprite;
		private CollisionShape2D collision;
		private bool followingPlayer;	
		private Queue<CharacterPathway> pathways = new();
		private int speed = 2;
		private bool walkingToPoint = false;
		private Vector2 targetPoint = Vector2.Zero;

		public AnimatedSprite2D Sprite { get { return sprite; } }
		public bool FollowingPlayer { get { return followingPlayer; } }

		public override void _Ready()
		{
			base._Ready();
			collision = GetNode<CollisionShape2D>("%Collision");
			sprite = GetNode<AnimatedSprite2D>("Sprite");

			sprite.Animation = "default";
			sprite.Frame = (int)DefaultDirection;
		}

        public override void _Process(double delta)
        {
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
                Position += normalizedDifference * speed;
            }
        }

        public override void Action()
		{
			Player player = global.CurrentRoom.Player;
			sprite.Frame = global.ReverseDirections[(int)player.Direction];
			global.CurrentRoom.Dialogue.ShowDisplay(DialogueResource, DialogueTitle);
			global.CurrentRoom.Dialogue.DialogueEnded += OnDialogueEnded;
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
			Active = false;
			followingPlayer = true;
			collision.Disabled = true;
		}

		public void DisableFollowing()
		{
			Active = true;
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

		public bool IsQueueEmpty()
		{
			return pathways.Count == 0;
		}

		public void FollowPlayer()
		{
			if (followingPlayer)
			{
				if (pathways.Count > 0)
				{
					CharacterPathway lastPathway = pathways.Peek();
					Vector2 lastPos = lastPathway.Position;
					Vector2 difference = lastPos - Position;

					GlobalPosition += difference;

					PlayAnimation(lastPathway.Direction);

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

		private void OnDialogueEnded(string initialTitle, string title)
		{
			sprite.Frame = (int)DefaultDirection;
			global.CurrentRoom.Dialogue.DialogueEnded -= OnDialogueEnded;
		}
	}
}