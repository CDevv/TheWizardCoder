using Godot;
using System;
using System.Threading;

public partial class TutorialBattle : EnemyBattleAttack
{
	[Export]
	public PackedScene EnemyBullet { get; set; }

    public void Intro()
    {
        Area.CallDeferred(CombatArea.MethodName.SpawnBullet, EnemyBullet);
		Thread.Sleep(5 * 1000);
    }
}
