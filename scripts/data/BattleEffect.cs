using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class BattleEffect
    {
        public BattleEffectType Action { get; set; }
        public CharacterType TargetType { get; set; }
        public int Turns { get; set; }
        public int Effect { get; set; }

        public BattleEffect(string[] arr)
        {
            Action = Enum.Parse<BattleEffectType>(arr[0]);
            TargetType = Enum.Parse<CharacterType>(arr[1]);
            Turns = int.Parse(arr[2]);
            Effect = int.Parse(arr[3]);
        }
    }
}
