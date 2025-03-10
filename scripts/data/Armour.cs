using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
	public class Armour
	{
		public string Name { get; private set; }
		public string Description { get; private set; }
        public ArmourType Type { get; set; }
        public int Effect { get; private set; }
		public BattleEffectType EffectType { get; private set; }

		public Armour(string name, string description, ArmourType type, int effect, BattleEffectType effectType)
		{
			Name = name;
			Description = description;
			Type = type;
			Effect = effect;
			EffectType = effectType;

			if (Effect < 0)
			{
				throw new ArgumentException("Effect cannot be negative.");
			}
		}
	}
}
