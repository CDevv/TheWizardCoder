using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class FirstRoom : BaseRoom
    {
        [Export]
        public Resource DialogueResource { get; set; }

        public override async void OnReady()
        {
            base.OnReady();
            if (!global.PlayerData.HasPlayedIntro)
            {
                global.CanWalk = false;
                await PlayCutscene("player_intro");
                await ShowDialogue(DialogueResource, "intro_cutscene");
                global.PlayerData.HasPlayedIntro = true;
            }
            else
            {
                GetNode<AnimatedSprite2D>("BedAnimationTop").Set(AnimatedSprite2D.PropertyName.Visible, false);
                GetNode<AnimatedSprite2D>("BedAnimationBottom").Set(AnimatedSprite2D.PropertyName.Visible, false);
            }
        }
    }
}
