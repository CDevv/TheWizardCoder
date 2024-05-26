using Godot;
using System;
using System.Threading;

public partial class TutorialBattle : EnemyBattleAttack
{
	[Export]
	public PackedScene EnemyBullet { get; set; }

    public void Intro()
    {
        Vector2 bulletPos = Area.GetRandomPoint();
        bulletPos = new Vector2(bulletPos.X, 2);
        Area.CallDeferred(CombatArea.MethodName.SpawnBullet, EnemyBullet, bulletPos, (int)Direction.Down);
    }

    public void Test()
    {
        Area.CallDeferred(CombatArea.MethodName.SpawnBullet, EnemyBullet, (int)Direction.Left);
    }
}
