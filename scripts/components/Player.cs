using Godot;
using System;
using System.Threading.Tasks;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using TheWizardCoder.Utils;

namespace TheWizardCoder.Components
{
    /// <summary>
    /// A component, representing the player's character in the world.
    /// </summary>
    public partial class Player : CharacterBody2D
    {
        [Export]
        public Resource SnippetsResource { get; set; }

        [Signal]
        public delegate void AnimationFinishedEventHandler();
        public Direction Direction { get; private set; }
        public const int DefaultSpeed = 2;

        public Actor Follower { get; set; }
        public bool HasFollower { get; set; } = false;
        public GroundEnemy Enemy { get; set; }
        public int PlayerSpeed { get; private set; } = DefaultSpeed;
        public float DistanceWalked { get; set; } = 0;
        public bool CameraEnabled { get; set; } = true;
        public new Vector2 Velocity { get; private set; }
        public bool IsSprinting { get; private set; } = false;

        private Global global;
        private Area2D interactableFinder;
        private Vector2 lastDirection;
        private AnimatedSprite2D animatedSprite;
        private Vector2 normalPosition = new(0, -35);
        private Vector2 equippedPosition = new(0, -22);
        private bool isItemEquipped = false;
        private string equippedItem;
        private bool itemIsInUse = false;

        public override void _Ready()
        {
            global = GetNode<Global>("/root/Global");
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

            KinematicCollision2D collision = MoveAndCollide(velocity);
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

                Godot.Collections.Array<Area2D> overlappingAreas = interactableFinder.GetOverlappingAreas();
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
                await Unequip();
            }

            if (Input.IsActionJustPressed("secondary"))
            {
                if (isItemEquipped && !itemIsInUse)
                {
                    if (equippedItem == "Fishing Rod" && !global.PlayerData.FishingRodSolved)
                    {
                        if (!global.CurrentRoom.CodeProblemPanel.Visible && !global.PlayerData.FishingRodSolved)
                        {
                            CodeProblem codeProblem = global.FishingProblemData;
                            global.CurrentRoom.CodeProblemPanel.ProblemId = codeProblem.UniqueIdentifier;

                            global.CurrentRoom.CodeProblemPanel.Reset();
                            global.CurrentRoom.CodeProblemPanel.ShowDisplay(codeProblem.Code,
                                codeProblem.Items,
                                codeProblem.SolvableAreas, false);
                        }
                    }
                }
            }

            if (Input.IsActionPressed("sprint"))
            {
                IsSprinting = true;
            }
            else
            {
                IsSprinting = false;
            }
        }

        /// <summary>
        /// Set an item in the Player's inventory as equipped.
        /// </summary>
        /// <remarks>
        /// <see cref="Player.EquipItem(string)"/> may be used in item_descriptions.json to define an item's behaviour when being used
        /// This 'Fishing Rod' item sets itself as equipped when the player 'uses' it using [Z] or [Enter] in the Inventory menu
        /// <code>
        /// "Fishing Rod":{
		/// "Description":"Catch fish!",
		/// "Type": "Key",
		/// "Effect": 0,
		/// "Price": 25,
		/// "AdditionalData": ["PlayerMethod", "EquipItem", "Fishing Rod"]
        /// }
        /// </code>
        /// </remarks>
        /// <param name="item">Name of the item</param>
        public void EquipItem(Item item)
        {
            global.GameDisplayEnabled = false;
            isItemEquipped = true;
            equippedItem = item.Name;
            global.CurrentRoom.GameDisplay.HideDisplay();

            item.OnEquipped.Call();
        }

        public async Task Unequip()
        {
            if (equippedItem == "Fishing Rod")
            {
                if (isItemEquipped)
                {
                    if (!itemIsInUse)
                    {
                        global.CanWalk = true;
                    }

                    isItemEquipped = false;

                    global.CurrentRoom.HideDisplay("FishingDisplay");

                    if (itemIsInUse)
                    {
                        itemIsInUse = false;

                        animatedSprite.PlayBackwards("fish_" + Direction.ToString().ToLower());

                        await ToSignal(animatedSprite, AnimatedSprite2D.SignalName.AnimationFinished);
                        animatedSprite.Position = normalPosition;

                        global.CanWalk = true;

                        PlayIdleAnimation(Direction);
                    }

                    global.GameDisplayEnabled = true;
                }
            }

            isItemEquipped = false;
        }

        private async void OnFishingTimeout()
        {
            if (itemIsInUse)
            {
                string itemName = "null";
                if (global.PlayerData.FishingRodSolved)
                {
                    itemName = "Fish";

                    float chance = GD.Randf();
                    if (chance <= 0.1f)
                    {
                        itemName = "Kris";
                    }
                }

                await Unequip();
                global.CurrentRoom.Dialogue.ShowDisplay(SnippetsResource, "fish", new()
                                        { itemName }, true);

                global.AddToInventory(itemName);
            }
        }

        /// <summary>
        /// Play an animation that is neither idle or movement with the provided <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the animation</param>
        public void PlaySideAnimation(string name)
        {
            animatedSprite.Play(name);
        }

        /// <summary>
        /// Play an idle animation that corresponds to the provided <paramref name="direction"/>
        /// </summary>
        /// <param name="direction">The direction of the animation</param>
        public void PlayIdleAnimation(Direction direction)
        {
            Direction = direction;

            animatedSprite.Stop();

            animatedSprite.Animation = "default";
            animatedSprite.Frame = (int)direction;

            interactableFinder.Rotation = direction.ToVector().Angle() - (Mathf.Pi / 2);
        }

        /// <summary>
        /// Play a movement animations that corresponds to the provided <paramref name="direction"/>
        /// </summary>
        /// <param name="direction">The direction of the animation</param>
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

        /// <summary>
        /// Freeze the player in place and disable the GameDisplay. Prevents the player from moving and opening the GameDisplay
        /// </summary>
        public void Freeze()
        {
            global.CanWalk = false;
            global.GameDisplayEnabled = false;
            PlayIdleAnimation(Direction);
        }

        /// <summary>
        /// Unfreeze the player, by making them able to move and enable the GameDisplay.
        /// </summary>
        public void Unfreeze()
        {
            global.CanWalk = true;
            global.GameDisplayEnabled = true;
        }

        /// <summary>
        /// Adds an Actor whose name is <paramref name="name"/> to the <c>Player</c>'s party and makes the <c>Actor</c>
        /// a follower.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="setPos"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when the Actor with name <paramref name="name"/> cannot be found</exception>
        public Actor AddAlly(string name, bool setPos)
        {
            Follower = global.CurrentRoom.GetNodeOrNull<Actor>(name);

            if (Follower == null)
            {
                throw new ArgumentException($"The Actor {name} does not exist.");
            }

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