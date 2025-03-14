using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Forest16 : ForestRoom
    {
        [Export]
        public Resource DialogueResource { get; set; }

        private async void FirstCutscene()
        {
            if (Player.Follower != null)
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
        }

        private async void RaftCutscene()
        {
            global.CanWalk = false;

            if (global.CurrentRoom.Player.Follower != null)
            {
                global.CurrentRoom.Player.Follower.DisableFollowing();

                global.CurrentRoom.Player.PlayIdleAnimation(global.CurrentRoom.Player.Direction);

                await PlayCutscene("forest_final_2");
                await ShowDialogue(DialogueResource, "forest_final_2");
                await PlayCutscene("forest_final_3");
            }
            else
            {
                await PlayCutscene("forest_final_2_nolan");
                await PlayCutscene("forest_final_3_nolan");
            }

            global.ChangeRoom("raft_water");
        }
    }

}