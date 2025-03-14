using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Rooms
{
    public partial class Vindi8 : BaseRoom
    {
        public override void OnReady()
        {
            base.OnReady();

            if (global.PlayerData.DefeatedQuestEnemy1 &&
                global.PlayerData.DefeatedQuestEnemy2 &&
                global.PlayerData.DefeatedQuestEnemy3)
            {
                global.PlayerData.HasFinishedCraigQuest = true;
            }
        }
    }

}