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

        private Actor timothy;
		private Actor gregory;
		private Marker2D timothyHouseMarker;

		public override void OnReady()
		{
			base.OnReady();
			timothy = GetNode<Actor>("Timothy");
			gregory = GetNode<Actor>("Gregory");
			timothyHouseMarker = GetNode<Marker2D>("TimothyMarker");

			if (!global.PlayerData.HasMetTimothy)
			{
				AnimationPlayer.Play("setup");
			}
            else
            {
				AnimationPlayer.Play("hide");
            }
        }

		private async void IntroCutscene()
		{
			Player.Freeze();
			await ShowDialogue(DialogueResource, "vindi_intro_1");
			await PlayCutscene("vindi_intro_2");
            await ShowDialogue(DialogueResource, "vindi_intro_2");
			global.GameDisplayEnabled = false;
            await WalkToPlayer();
            await ShowDialogue(DialogueResource, "vindi_intro_3");
			await WalkToMarker();
			Player.Unfreeze();
        }

		private async Task WalkToPlayer()
		{
			await timothy.WalkToPoint(new Vector2(timothy.Position.X, Player.Position.Y));
			await timothy.WalkToPoint(new Vector2(Player.Position.X + 40, Player.Position.Y));
        }

		private async Task WalkToMarker()
		{
			gregory.PlayIdleAnimation(Direction.Down);

			Vector2 point2 = GetNode<Marker2D>("WarperPoint").Position;
			await timothy.WalkToPoint(new Vector2(timothyHouseMarker.Position.X, timothy.Position.Y));
			await timothy.WalkToPoint(new Vector2(timothy.Position.X, point2.Y));
			timothy.Hide();
		}
	}
}
