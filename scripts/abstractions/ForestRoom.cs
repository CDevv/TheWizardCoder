namespace TheWizardCoder.Abstractions
{
    public partial class ForestRoom : BaseRoom
    {
        public override void OnReady()
        {
            base.OnReady();
            RemoveForestNotes();
        }

        private void RemoveForestNotes()
        {
            for (int i = 1; i <= 8; i++)
            {
                string itemName = $"Forest{i}.cs";
                global.RemoveFromInventory(itemName);
            }
        }
    }
}
