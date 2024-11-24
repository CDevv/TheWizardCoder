using Godot;

namespace TheWizardCoder.Utils
{
    public static class ColorHelper
    {
        public static Color GetRandom()
        {
            return new Color(GD.Randf(), GD.Randf(), GD.Randf());
        }
    }
}