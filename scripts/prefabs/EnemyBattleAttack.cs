using Godot;
using Godot.Collections;
using System;
using System.Threading;
public abstract partial class EnemyBattleAttack : Node2D
{
	[Export]
	public int Interval { get; set; } = 30;
	public CombatArea Area { get; set; }
}
