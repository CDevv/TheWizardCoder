using Godot;
using System;
using System.Threading.Tasks;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Enums;
using TheWizardCoder.Interactables;

namespace TheWizardCoder.Rooms
{
	public partial class Vindi1 : BaseRoom
	{
        [Export]
        public Resource DialogueResource { get; set; }

        private CharacterDialoguePoint timothy;
		private CharacterDialoguePoint gregory;

		public override void OnReady()
		{
			base.OnReady();
			timothy = GetNode<CharacterDialoguePoint>("Timothy");
			gregory = GetNode<CharacterDialoguePoint>("Gregory");

			if (!global.PlayerData.HasMetTimothy)
			{
				AnimationPlayer.Play("setup");
			}
		}

		private async void IntroCutscene()
		{
			Player.Freeze();
			await ShowDialogue(DialogueResource, "vindi_intro_1");
			await PlayCutscene("vindi_intro_2");
            await ShowDialogue(DialogueResource, "vindi_intro_2");
            await WalkToPlayer();
            await ShowDialogue(DialogueResource, "vindi_intro_3");
            global.CanWalk = true;
        }

		private async Task WalkToPlayer()
		{
			Direction targetDirection = Direction.Down;
            if (Player.Position.Y > timothy.Position.Y)
            {
				targetDirection = Direction.Down;
            }
            else if (Player.Position.Y < timothy.Position.Y)
            {
				targetDirection = Direction.Up;
            }
            else
            {
				return;
            }

			await MakeTimothyMoveY(targetDirection);
			await MakeTimothyMoveLeft();
        }

		private async Task MakeTimothyMoveY(Direction targetDirection)
		{
            timothy.PlayAnimation(targetDirection.ToString().ToLower());

            Vector2 targetPosition = new Vector2(timothy.Position.X, Player.Position.Y);

            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(timothy, "position", targetPosition, 2);
            tween.Play();

            await ToSignal(tween, Tween.SignalName.Finished);

            timothy.PlayIdleAnimation(targetDirection);
        }

		private async Task MakeTimothyMoveLeft()
		{
			timothy.PlayAnimation("left");

			Vector2 targetPosition = new Vector2(Player.Position.X + 32, Player.Position.Y);

			Tween tween = GetTree().CreateTween();
            tween.TweenProperty(timothy, "position", targetPosition, 2);
            tween.Play();

            await ToSignal(tween, Tween.SignalName.Finished);

			timothy.PlayIdleAnimation(Direction.Left);
        }
	}
}
