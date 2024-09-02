using Godot;

public partial class CharacterData : GodotObject
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Points { get; set; }
    public int MaxPoints { get; set; }
    public int AttackPoints { get; set; }
}