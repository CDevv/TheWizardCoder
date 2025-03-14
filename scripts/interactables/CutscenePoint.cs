using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Interactables
{
    public partial class CutscenePoint : Interactable
    {
        [Export]
        public string PlaythroughPropertyName { get; set; }
        [Export]
        public string RoomMethodName { get; set; }

        public override void _Ready()
        {
            base._Ready();

            if (!string.IsNullOrEmpty(PlaythroughPropertyName))
            {
                if (global.PlayerData.Get(PlaythroughPropertyName).AsBool())
                {
                    Active = false;
                }
            }
        }

        public override void Action()
        {
            global.CurrentRoom.CallDeferred(RoomMethodName);
            Active = false;

            if (!string.IsNullOrEmpty(PlaythroughPropertyName))
            {
                global.PlayerData.Set(PlaythroughPropertyName, true);
            }
        }
    }
}