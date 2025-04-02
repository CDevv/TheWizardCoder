using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;

namespace TheWizardCoder.Subdisplays
{
    public abstract partial class BattleSideDisplay : Display
    {
        [Export]
        public BattleDisplay BattleDisplay { get; set; }
        [Export]
        public BattleOptions BattleOptions { get; set; }

        public BattleSide Characters { get; protected set; }
        public int CurrentCharacter { get; protected set; }

        public override void _Ready()
        {
            base._Ready();
            Characters = new BattleSide();
        }

        public override void ShowDisplay()
        {
            Show();
        }

        public abstract void AddCharacter(Character character);
        public abstract Task DisplayHealthChange(int index, int change);
        public abstract Task DisplayManaChange(int index, int change);
        public abstract Task DisplayBattleEffect(int index);
    }
}
