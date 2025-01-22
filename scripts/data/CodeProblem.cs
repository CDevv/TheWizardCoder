using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWizardCoder.Data
{
    public class CodeProblem
    {
        public string UniqueIdentifier { get; set; }
        public string Code { get; set; }
        public Godot.Collections.Array<string> Items { get; set; }
        public Godot.Collections.Dictionary<string, Vector2> SolvableAreas { get; set; }

        public void ApplyDictionary(Godot.Collections.Dictionary<string, Variant> dict)
        {
            UniqueIdentifier = (string)dict["UniqueIdentifier"];
            Code = (string)dict["Code"];
            Items = new Godot.Collections.Array<string>((string[])dict["Items"]);

            SolvableAreas = new();
            Godot.Collections.Dictionary<string, Variant> solvableAreas = (Godot.Collections.Dictionary<string, Variant>)dict["SolvableAreas"];
            foreach (var item in solvableAreas)
            {
                int[] arr = (int[])item.Value;
                Vector2 vector = new Vector2(arr[0], arr[1]);

                SolvableAreas[item.Key] = vector;
            }
        }
    }
}
