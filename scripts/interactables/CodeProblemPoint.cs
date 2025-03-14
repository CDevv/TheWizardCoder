using Godot;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Autoload;
using TheWizardCoder.Displays;

namespace TheWizardCoder.Interactables
{
    public partial class CodeProblemPoint : Interactable
    {
        [Signal]
        public delegate void ProblemSolvedEventHandler();

        [Export]
        public string UniqueIdentifier { get; set; }

        [Export(PropertyHint.MultilineText)]
        public string Code { get; set; }

        [Export]
        public Godot.Collections.Array<string> Items { get; set; }

        [Export]
        public Godot.Collections.Dictionary<string, Vector2> SolvableAreas { get; set; }

        [Export]
        public bool UseInventory { get; set; } = false;

        public override void _Ready()
        {
            base._Ready();
            global = GetNode<Global>("/root/Global");

            if (!string.IsNullOrEmpty(UniqueIdentifier))
            {
                if (global.PlayerData.Get(UniqueIdentifier).AsBool())
                {
                    Active = false;
                }
            }
        }

        public override void Action()
        {
            global.CurrentRoom.CodeProblemPanel.Point = this;
            global.CurrentRoom.CodeProblemPanel.ProblemId = UniqueIdentifier;
            global.CurrentRoom.CodeProblemPanel.ShowDisplay(Code, Items, SolvableAreas, UseInventory);

            global.CurrentRoom.CodeProblemPanel.ProblemSolved += OnCodeSolved;

            if (!global.CurrentRoom.CodeProblemPanel.IsConnected(CodeProblemPanel.SignalName.VisibilityChanged, Callable.From(OnPanelVisibilityChanged)))
            {
                global.CurrentRoom.CodeProblemPanel.VisibilityChanged += OnPanelVisibilityChanged;
            }
        }

        private void OnPanelVisibilityChanged()
        {
            if (!global.CurrentRoom.CodeProblemPanel.Visible)
            {
                global.CurrentRoom.CodeProblemPanel.ProblemSolved -= OnCodeSolved;
            }
        }

        public override void OnNotActive()
        {
            global.CanWalk = true;
            global.GameDisplayEnabled = true;
        }

        private void OnCodeSolved()
        {
            if (!string.IsNullOrEmpty(UniqueIdentifier))
            {
                global.PlayerData.Set(UniqueIdentifier, true);
            }

            Active = false;
            EmitSignal(SignalName.ProblemSolved);
        }
    }
}