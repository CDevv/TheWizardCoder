using System;

public class CharacterBattleState
{
    public int Target { get; set; }
    public CharacterAction Action { get; set; }
    public int ActionModifier { get; set; }

    public CharacterBattleState()
    {
        Target = 0;
        Action = CharacterAction.Attack;
        ActionModifier = 0;
    }

    public void Reset()
    {
        Target = 0;
        Action = CharacterAction.Attack;
        ActionModifier = 0;
    }
}