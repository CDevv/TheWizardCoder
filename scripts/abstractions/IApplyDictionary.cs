using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWizardCoder.Abstractions
{
    public interface IApplyDictionary
    {
        public void ApplyDictionary(Godot.Collections.Dictionary<string, Variant> dict);
    }
}
