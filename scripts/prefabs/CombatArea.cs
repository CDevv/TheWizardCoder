using Godot;
using Godot.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class CombatArea : Area2D
{
	[Signal]
	public delegate void TimeoutEventHandler();
	[Signal]
	public delegate void PlayerGotDamagedEventHandler(int value);

	[Export]
	public PackedScene EnemyBulletPackedScene { get; set; }
	public PackedScene EnemyAttackPackedScene { get; set; }
	public PackedScene PlayerBallPackedScene { get; set; }

	private bool active = false;
	private int currentAttackIndex = 0;
	private Global global;
	private PlayerBall playerBall;
	private EnemyBattleAttack enemy;
	private Array<GodotObject> bullets = new();
	private CollisionShape2D collisionShape;
	

	public bool Active { get { return active; } }
	public BattleInfo BattleInfo { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		collisionShape = GetNode<CollisionShape2D>("CollisionShape");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (playerBall != null)
		{
			Vector2 resolutionVector = new Vector2(global.Settings.WindowWidth, global.Settings.WindowHeight);
			Vector2 mousePosition = global.CurrentRoom.Camera.GlobalPosition - (resolutionVector / 2) + GetViewport().GetMousePosition();
			//playerBall.Position = mousePosition;
			playerBall.SetDeferred(PropertyName.Position, mousePosition);
		}
	}

	public void DoAttack()
	{
		PlayerBall playerBall = PlayerBallPackedScene.Instantiate<PlayerBall>();

		GetParent().CallDeferred(Node.MethodName.AddChild, playerBall);
		this.playerBall = playerBall;

		active = true;

		enemy = BattleInfo.Attacks.Instantiate<EnemyBattleAttack>();
		enemy.Area = this;
		enemy.PlayerGotDamaged += OnPlayerDamaged;

		SceneTreeTimer timer = GetTree().CreateTimer(enemy.Interval);
		timer.Timeout += OnTimeout;

		Thread thread = new Thread(() => {
			while (active)
			{
				enemy.Call(BattleInfo.AttackNames[0]);
			}
		});
		thread.Start();
	}

	public Vector2 GetRandomPoint()
	{
		Rect2 collisionRect = collisionShape.Shape.GetRect();

		float x = (float)GD.RandRange(Position.X - (collisionRect.Size.X / 2), Position.X + (collisionRect.Size.X / 2));
		float y = (float)GD.RandRange(Position.Y - (collisionRect.Size.Y / 2), Position.Y + (collisionRect.Size.Y / 2));

		Vector2 result = new Vector2(x, y);
		return result;
	}

	public Rect2 GetRect()
	{
		return collisionShape.Shape.GetRect();
	}

	public void SpawnBullet(PackedScene bulletPackedScene)
	{
		Rect2 collisionRect = collisionShape.Shape.GetRect();

		Vector2 bulletPos = GetRandomPoint();
		bulletPos = new Vector2(bulletPos.X, Position.Y - (collisionRect.Size.Y/2) + 2);

		EnemyBullet bullet = bulletPackedScene.Instantiate<EnemyBullet>();
		bullet.DamagedPlayer += OnPlayerDamaged;
		bullet.Position = bulletPos;

		GetParent().CallDeferred(Node.MethodName.AddChild, bullet);
		bullets.Add(bullet);
	}

	private void Clear()
	{
		enemy.Clear();
		for (int i = 0; i < bullets.Count; i++)
		{
			GodotObject bullet = bullets[i];
			if (GodotObject.IsInstanceValid(bullet))
			{
				if (!bullet.IsQueuedForDeletion())
				{
					bullet.Dispose();
				}
			}
			else
			{
				bullets.RemoveAt(i);
			}
		}
		if (!playerBall.IsQueuedForDeletion())
		{
			playerBall.QueueFree();
			playerBall = null;
		}	
	}

	public void OnTimeout()
	{		
		active = false;
		Clear();
		enemy.Dispose();
		EmitSignal(SignalName.Timeout);
	}

	public void OnPlayerDamaged(int value)
	{
		EmitSignal(SignalName.PlayerGotDamaged, new Variant[] {value});
	}
}
