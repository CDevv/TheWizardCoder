using Godot;
using System;
using TheWizardCoder.Enums;
using TheWizardCoder.Components;
using TheWizardCoder.Data;
using System.Collections.Generic;
using System.Linq;
using TheWizardCoder.Utils;
using System.Threading.Tasks;
using TheWizardCoder.Autoload;
using TheWizardCoder.Interactables;

namespace TheWizardCoder.Components
{
	/// <summary>
	/// A class representing an Actor, which may just be a Non-Playable Character (NPC) that has a dialogue or follows the player.
	/// </summary>
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
		private Direction direction;

		public AnimatedSprite2D Sprite { get { return sprite; } }

		/// <summary>
		/// Returns whether or not this <c>Actor</c> is currently following the player.
		/// </summary>
		public bool FollowingPlayer { get { return followingPlayer; } }
		public int Speed { get; set; } = 2;
		public CharacterPathway LastPathway { get; set; }

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
			if (global.CurrentRoom.Player.IsSprinting || global.Settings.AutoSprint)
			{
				Speed = Player.DefaultSpeed * 2;
			}
			else
			{
				Speed = Player.DefaultSpeed;
			}

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
				FollowPlayer(delta);
			}
		}

		public void MakeFollower()
		{
			EnableFollowing();

			CharacterPathway pathway = new(global.CurrentRoom.Player.Direction, GlobalPosition, global.CurrentRoom.Player.PlayerSpeed);
			pathways.Enqueue(pathway);

			global.CurrentRoom.Player.Follower = this;
		}

		/// <summary>
		/// Enables following the player of an <c>Actor</c>
		/// </summary>
		public void EnableFollowing()
		{
			interactable.Active = false;
			followingPlayer = true;
			collision.Disabled = true;
		}

		/// <summary>
		/// Disables following the player. It may be enabled again with <c>EnableFollowing()</c>, thus pausing the following toggle is possible.
		/// </summary>
		public void DisableFollowing()
		{
			interactable.Active = true;
			followingPlayer = false;
			collision.Disabled = false;

			pathways = new();
		}

		/// <summary>
		/// Adds a pathway point to the pathways queue of an <c>Actor</c>. When the <c>Actor</c>
		/// reaches this point, it will change to the provided <paramref name="direction"/> and <paramref name="speed"/>.
		/// </summary>
		/// <param name="direction">Direction that the Actor will face.</param>
		/// <param name="point">The position of the point</param>
		/// <param name="speed">Speed that the Actor will change to</param>
		public void AddPathwayPoint(Direction direction, Vector2 point, int speed)
		{
			CharacterPathway pathway = new(direction, point, speed);
			pathways.Enqueue(pathway);
		}

		/// <summary>
		/// Adds a pathway point to the pathways queue with the current direction and position and a default speed.
		/// </summary>
		public void AddPathwayPoint()
		{
			AddPathwayPoint(DefaultDirection, Position, 2);
		}

		/// <summary>
		/// Method that advances the movement of an <c>Actor</c> towards the <c>Player</c>. Should be used
		/// within a <c>_Process</c> or <c>_PhysicsProcess</c> method.
		/// <example>
		/// For example:
		/// <code>
		/// public void _PhysicsProcess(double delta)
		/// {
		///		//...
		///		FollowPlayer(delta);
		/// }
		/// </code>
		/// </example>
		/// </summary>
		/// <param name="delta">Deltatime to be provided by the <c>_PhysicsProcess</c> method.</param>
		public void FollowPlayer(double delta)
		{
			if (global.CurrentRoom.Player.Velocity != Vector2.Zero)
			{
				if (pathways.Count > 0)
				{
					CharacterPathway peekedPathway = pathways.Peek();
					Vector2 lastPos = peekedPathway.Position;
					Vector2 difference = lastPos - Position;
					Vector2 velocity = difference.Normalized() * Speed;

					MoveAndCollide(velocity);

					if (LastPathway == null)
					{
						PlayAnimation(peekedPathway.Direction);
					}
					else
					{
						PlayAnimation(LastPathway.Direction);
					}

					if (lastPos.DistanceTo(Position) < 2)
					{
						LastPathway = pathways.Dequeue();
					}
				}
				else
				{
					Vector2 lastPos = global.CurrentRoom.Player.Position;
					Vector2 difference = lastPos - Position;
					Vector2 velocity = difference.Normalized() * Speed;

					if (lastPos.DistanceTo(Position) >= 32)
					{
						MoveAndCollide(velocity);
					}

					PlayAnimation(LastPathway.Direction);
				}
			}
			else
			{
				if (pathways.Count > 0)
				{
					PlayIdleAnimation(pathways.Peek().Direction);
				}
				else
				{
					PlayIdleAnimation(LastPathway.Direction);
				}
			}
		}

		/// <summary>
		/// Peek the pathways queue of this <c>Actor</c>
		/// </summary>
		/// <returns>A <c>CharacterPathway</c> object that describes the last pathway on top of the queue.</returns>
		public CharacterPathway PeekPathway()
		{
			return pathways.Peek();
		}

		/// <summary>
		/// Makes the <c>Actor</c> walk to a certain position. Can be awaited in case the <c>Actor</c>
		/// needs to to walk to several points one by one.
		/// <example>
		/// Example with walking to different positions one by one:
		/// <code>
		/// private async Task WalkToPoints()
		/// {
		///		await WalkToPoint(new Vector2(50, 20));
		///		await WalkToPoint(new Vector2(50, 70));
		/// }
		/// </code>
		/// </example>
		/// </summary>
		/// <param name="point">Position to walk to</param>
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

		/// <summary>
		/// Play an animation.
		/// </summary>
		/// <param name="name">Name of the animation.</param>
		public void PlayAnimation(string name)
		{
			sprite.Play(name);
		}

		/// <summary>
		/// Play a walking animation, corresponding to the provided <paramref name="direction"/>
		/// </summary>
		/// <param name="direction"></param>
		public void PlayAnimation(Direction direction)
		{
			this.direction = direction;

			PlayAnimation(direction.ToString().ToLower());
		}

		/// <summary>
		/// Play an idle animation, corresponding to the provided <paramref name="direction"/>
		/// </summary>
		/// <param name="direction"></param>
		public void PlayIdleAnimation(Direction direction)
		{
			sprite.Stop();
			sprite.Animation = "default";
			sprite.Frame = (int)direction;
		}

		/// <summary>
		/// Play an idle animation for the current direction of the <c>Actor</c>
		/// </summary>
		public void PlayIdleAnimation()
		{
			PlayIdleAnimation(direction);
		}
	}
}