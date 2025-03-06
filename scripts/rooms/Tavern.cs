using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
	public partial class Tavern : BaseRoom
	{
		[Export]
		public Resource DialogueResource { get; set; }
		public override async void OnReady()
		{
			base.OnReady();
			if (!global.PlayerData.HasMetBerry)
			{
				await ShowDialogue(DialogueResource, "berry_1");
				await PlayCutscene("berry_1");
				global.PlayerData.HasMetBerry = true;
				TransitionToRoom("tavern_barrels_room", "EnterMarker", Enums.Direction.Up);
            }
		}
	}
}