using DialogueManagerRuntime;
using Godot;
using System;
using System.Threading.Tasks;
using TheWizardCoder.Autoload;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;
using TheWizardCoder.Data;
using TheWizardCoder.Interactables;
using TheWizardCoder.UI;
using TheWizardCoder.Utils;

namespace TheWizardCoder.Components
{
	public partial class Player : CharacterBody2D
	{
		[Signal]
		public delegate void AnimationFinishedEventHandler();
		public Direction Direction { get; private set; }
		public const int DefaultSpeed = 2;
		private Global global;
		private AnimationPlayer animationPlayer;
		private AnimationTree animationTree;
		private Area2D interactableFinder;
		private Vector2 lastDirection;

		public Actor Follower { get; set; }
		public bool HasFollower { get; set; } = false;
		public GroundEnemy Enemy { get; set; }

		public int PlayerSpeed { get; private set; } = DefaultSpeed;
		public float DistanceWalked { get; set; } = 0;
		public bool CameraEnabled { get; set; } = true;
        public new Vector2 Velocity { get; set; }
		public bool IsSprinting { get; private set; } = false;

        public override void _Ready()	
		{
			global = GetNode<Global>("/root/Global");
			animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			animationTree = GetNode<AnimationTree>("AnimationTree");
			interactableFinder = GetNode<Area2D>("InteractableFinder");

			DistanceWalked = 0;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (!global.CanWalk)
			{
				return;
			}
			animationTree.Set("parameters/conditions/extrastate", false);

			Vector2 direction = Input.GetVector("left", "right", "up", "down").Normalized();
			Vector2 velocity = direction * DefaultSpeed;

			if (IsSprinting || global.Settings.AutoSprint)
			{
				velocity *= 2;
				PlayerSpeed = DefaultSpeed * 2;
			}
			else
			{
				PlayerSpeed = DefaultSpeed;
			}

			Velocity = velocity;

			if (velocity == Vector2.Zero)
			{
				animationTree.Set("parameters/Transition/transition_request", "idle");
			}
			else
			{
				animationTree.Set("parameters/Transition/transition_request", "move");
			}

			if (direction != Vector2.Zero)
			{
				ChangeDirection(direction);
				Direction = direction.ToDirection();
			}

			if (global.CurrentRoom.Camera != null && CameraEnabled)
			{
				global.CurrentRoom.Camera.Set(Camera2D.PropertyName.Position, Position + velocity);
			}

			if (global.PlayerIsOnStairs && velocity.X != 0)
			{
				float addedY = (float)(100 * delta);
				if (velocity.X > 0)
				{
					if (global.StairsGoUp)
					{
						velocity.Y += global.StairsInverted ? -addedY : addedY;
					}
				}
				else
				{
					velocity.Y -= global.StairsInverted ? -addedY : addedY;
				}
			}

			var collision = MoveAndCollide(velocity); 
			DistanceWalked += Position.DistanceTo(Position + velocity);

			if (Follower != null && collision == null)
			{
				if (velocity != Vector2.Zero)
				{
                    if (Follower.FollowingPlayer)
                    {
                        if (lastDirection != direction)
                        {
                            Follower.AddPathwayPoint(Direction, GlobalPosition, PlayerSpeed);
                        }
                    }
                }
			}

			lastDirection = direction;
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (Input.IsActionJustPressed("ui_accept"))
			{
				if (global.IsInCutscene)
				{
					return;
				}

				var overlappingAreas = interactableFinder.GetOverlappingAreas();
				if (overlappingAreas.Count > 0 && global.CanWalk)
				{
					global.CanWalk = false;

					for (int i = 0; i < overlappingAreas.Count; i++)
					{
						Interactable interactable = (Interactable)overlappingAreas[i];
						
						if (interactable.Active)
						{
							PlayIdleAnimation(Direction);
							interactable.Action();
							break;
						}
						else
						{
							interactable.OnNotActive();
						}
					}
				}
			}

			if (Input.IsActionPressed("sprint"))
			{
				IsSprinting = true;
				animationTree.Set("parameters/TimeScale/scale", 2);
			}
			else 
			{
				IsSprinting = false;
				animationTree.Set("parameters/TimeScale/scale", 1);
			}
		}

		public void Freeze()
		{
			global.CanWalk = false;
			global.GameDisplayEnabled = false;
			PlayIdleAnimation(Direction);
		}

		public void Unfreeze()
		{
			global.CanWalk = true;
			global.GameDisplayEnabled = true;
		}

		public async void TransitionToRoom(Warper warper)
		{
			global.CurrentRoom.TransitionRect.PlayAnimation();
			await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
			if (string.IsNullOrEmpty(warper.TargetLocation))
			{
				global.ChangeRoom(warper.TargetRoomName);
			}
			else
			{
				global.ChangeRoom(warper.TargetRoomName, warper.TargetLocation, warper.PlayerDirection);
			}
		}

		public async void TransitionToRoomName(string roomName, string markerName, Direction direction)
		{
			global.CurrentRoom.TransitionRect.PlayAnimation();
			await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
			if (string.IsNullOrEmpty(markerName))
			{
				global.ChangeRoom(roomName);
			}
			else
			{
				global.ChangeRoom(roomName, markerName, direction);
			}
		}

		public void ChangeDirection(Vector2 vectorDirection)
		{
			animationTree.Set("parameters/Idle/blend_position", vectorDirection);
			animationTree.Set("parameters/Move/blend_position", vectorDirection);
		}

		public void ChangeDirection(Direction direction)
		{
			//Vector2 resultVector = global.GetDirectionVector(direction);
			Vector2 resultVector = direction.ToVector();

			animationTree.Set("parameters/Idle/blend_position", resultVector);
			animationTree.Set("parameters/Move/blend_position", resultVector);
		}

		public void PlaySideAnimation(string name)
		{
			animationTree.Set("parameters/Transition/transition_request", "extra");
			animationTree.Set("parameters/Extra/transition_request", name);

			Task finishedAnimation = new Task(async () => {
				await ToSignal(animationTree, AnimationTree.SignalName.AnimationFinished);
				EmitSignal(SignalName.AnimationFinished);
			});
		}

		public void PlayIdleAnimation(Direction direction)
		{
			animationTree.Set("parameters/Transition/transition_request", "idle");
			ChangeDirection(direction);
		}

		public void PlayMoveAnimation(Direction direction)
		{
			animationTree.Set("parameters/Transition/transition_request", "move");
			ChangeDirection(direction);
		}

		public void ShowDialogueBallon(Resource resource, string title)
		{
			DialogueManager.ShowDialogueBalloon(resource, title);
		}

		public Actor AddAlly(string name, bool setPos)
		{
			Follower = global.CurrentRoom.GetNode<Actor>(name);	
			if (!global.PlayerData.Allies.Exists(x => x.Name == name))
			{
				global.PlayerData.Allies.Add(global.Characters[name]);
			}

			if (setPos)
			{
				Follower.GlobalPosition = GlobalPosition;
			}
			Follower.MakeFollower();
			Follower.Show();

			HasFollower = true;

			return Follower;
		}
	}
}