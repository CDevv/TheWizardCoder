using Godot;
using System;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Vindi4 : BaseRoom
    {
        public override void OnReady()
        {
            base.OnReady();

            if (!global.PlayerData.VindiTreeSolved)
            {
                AnimationPlayer.Play("tree_init");
            }
            else
            {
                AnimationPlayer.Play("tree_solved");
            }
        }

        private void OnProblemSolved()
        {
            AnimationPlayer.Play("tree_solved_anim");
        }
    }
}
