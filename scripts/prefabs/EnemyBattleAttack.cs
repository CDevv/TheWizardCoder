using Godot;
using Godot.Collections;
using System;
using System.Threading;
public abstract partial class EnemyBattleAttack : Node2D
{
	[Signal]
	public delegate void PlayerGotDamagedEventHandler(int value);

	[Export]
	public int Interval { get; set; } = 30;
	public string CurrentAttackMethodName { get; set; }

	public CombatArea Area { get; set; }
	
	private Array<GodotObject> bullets = new();

	public void Initiate()
	{
		
	}

	public void Clear()
	{
		foreach (var bullet in bullets)
		{
			if (!bullet.IsQueuedForDeletion())
			{
				bullet.Dispose();
			}
		}
	}

	private void OnPlayerDamaged(int value)
	{
		EmitSignal(SignalName.PlayerGotDamaged, value);
	}
}
