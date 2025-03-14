using TheWizardCoder.Abstractions;
using TheWizardCoder.Interactables;

namespace TheWizardCoder.Rooms
{
    public partial class ForestNobeResidence : ForestRoom
    {
        private Warper cabinWarper;
        private DialoguePoint dialoguePoint;

        public override void OnReady()
        {
            base.OnReady();

            cabinWarper = GetNode<Warper>("NobeCabinWarper");
            dialoguePoint = GetNode<DialoguePoint>("NobeCabinDialogue");

            if (global.PlayerData.UnlockedNobeCabin)
            {
                UnlockCabin();
            }
        }

        private void UnlockCabin()
        {
            global.PlayerData.UnlockedNobeCabin = true;
            cabinWarper.Active = true;
            dialoguePoint.Active = false;
        }
    }
}
