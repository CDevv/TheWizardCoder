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

		public override void OnReady()
		{
			base.OnReady();
			timothy = GetNode<Actor>("Timothy");
			gregory = GetNode<Actor>("Gregory");

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
			await timothy.WalkToPoint(new Vector2(timothy.Position.X, Player.Position.Y));
			await timothy.WalkToPoint(new Vector2(Player.Position.X + 32, Player.Position.Y));
        }
	}
}
