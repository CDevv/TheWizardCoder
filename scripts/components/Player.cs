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
        [Export]
        public Resource SnippetsResource { get; set; }

        [Signal]
		public delegate void AnimationFinishedEventHandler();
		public Direction Direction { get; private set; }
		public const int DefaultSpeed = 2;
		private Global global;
		private AnimationPlayer animationPlayer;
		private AnimationTree animationTree;
		private Area2D interactableFinder;
		private Vector2 lastDirection;
		private AnimatedSprite2D animatedSprite;
		private Vector2 normalPosition = new(0, -35);
		private Vector2 equippedPosition = new(0, -22);

		public Actor Follower { get; set; }
		public bool HasFollower { get; set; } = false;
		public GroundEnemy Enemy { get; set; }

		public int PlayerSpeed { get; private set; } = DefaultSpeed;
		public float DistanceWalked { get; set; } = 0;
		public bool CameraEnabled { get; set; } = true;
        public new Vector2 Velocity { get; private set; }
		public bool IsSprinting { get; private set; } = false;

		private bool isItemEquipped = false;
		private string equippedItem;

		private bool itemIsInUse = false;

        public override void _Ready()	
		{
			global = GetNode<Global>("/root/Global");
			animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			animationTree = GetNode<AnimationTree>("AnimationTree");
			interactableFinder = GetNode<Area2D>("InteractableFinder");

			animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

			DistanceWalked = 0;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (!global.CanWalk)
			{
				return;
			}

			Vector2 directionVector = Input.GetVector("left", "right", "up", "down").Normalized();
			Vector2 velocity = directionVector * DefaultSpeed;

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

            if (directionVector != Vector2.Zero)
            {
                Direction = directionVector.ToDirection();
            }

            if (velocity == Vector2.Zero)
			{
				PlayIdleAnimation(Direction);
			}
			else
			{
				PlayMoveAnimation(directionVector);
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
                        if (lastDirection != directionVector)
                        {
                            Follower.AddPathwayPoint(Direction, GlobalPosition, PlayerSpeed);
                        }
                    }
                }
			}

			lastDirection = directionVector;
		}

		public override async void _UnhandledInput(InputEvent @event)
		{
			if (Input.IsActionJustPressed("ui_accept"))
			{
				if (global.IsInCutscene)
				{
					return;
				}

				var overlappingAreas = interactableFinder.GetOverlappingAreas();
				if (overlappingAreas.Count > 0)
				{
                    if (global.CanWalk)
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
                else
                {
                    if (isItemEquipped)
                    {
						TileData tileData = global.CurrentRoom.GetTileAtPosition(1, Position + (Direction.ToVector() * 24));
                        if (equippedItem == "Fishing Rod" && tileData.GetCustomData("isWater").AsBool())
                        {
							if (!itemIsInUse)
							{
                                itemIsInUse = true;

                                global.CanWalk = false;
                                global.GameDisplayEnabled = false;

								
                                animatedSprite.Position = equippedPosition;
                                animatedSprite.Play("fish_" + Direction.ToString().ToLower());

                                SceneTreeTimer timer = GetTree().CreateTimer(5);
								timer.Timeout += OnFishingTimeout;
                            }
                        }
                    }
                }
            }

            if (Input.IsActionJustPressed("ui_cancel"))
            {
				Unequip();
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

		public void EquipItem(string item)
		{
			isItemEquipped = true;
			equippedItem = item;

            if (item == "Fishing Rod")
            {
                global.CurrentRoom.ShowDisplay("FishingDisplay");
            }
        }

        public async Task Unequip()
		{
            if (equippedItem == "Fishing Rod")
            {
                if (isItemEquipped && itemIsInUse)
                {
                    animatedSprite.PlayBackwards("fish_" + Direction.ToString().ToLower());

                    await ToSignal(animatedSprite, AnimatedSprite2D.SignalName.AnimationFinished);
                    animatedSprite.Position = normalPosition;

                    global.CanWalk = true;
                    global.GameDisplayEnabled = true;
                    isItemEquipped = false;
                    itemIsInUse = false;

                    global.CurrentRoom.HideDisplay("FishingDisplay");
                    PlayIdleAnimation(Direction);
                }
            }

            isItemEquipped = false;
		}

		private async void OnFishingTimeout()
		{
            if (itemIsInUse)
            {
				string itemName = "null";

                await Unequip();
                global.CurrentRoom.Dialogue.ShowDisplay(SnippetsResource, "fish", new()
                                        { itemName }, true);

				global.AddToInventory(itemName);
            }
        }

        public void PlaySideAnimation(string name)
        {
            animatedSprite.Play(name);
        }

        public void PlayIdleAnimation(Direction direction)
        {
			Direction = direction;

			animatedSprite.Stop();

            animatedSprite.Animation = "default";
            animatedSprite.Frame = (int)direction;

            interactableFinder.Rotation = direction.ToVector().Angle() - (Mathf.Pi / 2);
        }

        public void PlayMoveAnimation(Direction direction)
        {
			Direction = direction;

            animatedSprite.Play(direction.ToString().ToLower(), PlayerSpeed / 2);

            interactableFinder.Rotation = direction.ToVector().Angle() - (Mathf.Pi / 2);
        }

        public void PlayMoveAnimation(Vector2 direction)
        {
            if (!Vector2Helper.IsInOneDirection(direction))
            {
                return;
            }

            PlayMoveAnimation(direction.ToDirection());
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
			//animationTree.Set("parameters/Idle/blend_position", vectorDirection);
			//animationTree.Set("parameters/Move/blend_position", vectorDirection);
		}

		public void ChangeDirection(Direction direction)
		{
			//Vector2 resultVector = global.GetDirectionVector(direction);
			//Vector2 resultVector = direction.ToVector();

			//animationTree.Set("parameters/Idle/blend_position", resultVector);
			//animationTree.Set("parameters/Move/blend_position", resultVector);
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