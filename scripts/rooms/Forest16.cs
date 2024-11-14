using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class Forest16 : BaseRoom
{
	[Export]
	public Resource DialogueResource { get; set; }

	private async void FirstCutscene()
	{
		global.CurrentRoom.Player.CameraEnabled = false;
		global.CurrentRoom.Player.PlayIdleAnimation(global.CurrentRoom.Player.Direction);
		global.CanWalk = false;

		await TweenCamera(new Vector2(416, 880), 2);
		Camera.Position = new Vector2(416, 880);

		SceneTreeTimer timer = GetTree().CreateTimer(1);
		await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

		await TweenCameraToPlayer(2);
		await ShowDialogue(DialogueResource, "forest_final_1");

		global.CurrentRoom.Player.CameraEnabled = true;
		global.CanWalk = true;
	}

	private async void RaftCutscene()
	{
		if (global.CurrentRoom.Player.Follower != null)
		{
			global.CurrentRoom.Player.Follower.DisableFollowing();
		}
		
		global.CanWalk = false;
		global.CurrentRoom.Player.PlayIdleAnimation(global.CurrentRoom.Player.Direction);

		await PlayCutscene("forest_final_2");
		await ShowDialogue(DialogueResource, "forest_final_2");
		await PlayCutscene("forest_final_3");
	}
}
