using Godot;
using TheWizardCoder.Abstractions;

namespace TheWizardCoder.Interactables
{
    public partial class CodeMessagePoint : Interactable
    {
        [Export(PropertyHint.MultilineText)]
        public string CodeText { get; set; }

        public override void _Ready()
        {
            base._Ready();
        }

        public override void Action()
        {

            global.CurrentRoom.CodeMessage.ShowDisplay(CodeText);
        }
    }
}
