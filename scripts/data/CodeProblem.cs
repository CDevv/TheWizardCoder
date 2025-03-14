using Godot;
using Godot.Collections;

namespace TheWizardCoder.Data
{
    public class CodeProblem
    {
        public string UniqueIdentifier { get; set; }
        public string Code { get; set; }
        public Array<string> Items { get; set; }
        public Dictionary<string, Vector2> SolvableAreas { get; set; }

        public CodeProblem(string uniqueIdentifier, string code, Array<string> items, Dictionary<string, Vector2> solvableAreas)
        {
            UniqueIdentifier = uniqueIdentifier;
            Code = code;
            Items = items;
            SolvableAreas = solvableAreas;
        }
    }
}
