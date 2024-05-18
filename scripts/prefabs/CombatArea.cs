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
	public PackedScene PlayerBallPackedScene { get; set; }

	private bool active = false;
	private Global global;
	private PlayerBall playerBall;
	private Array<GodotObject> bullets = new();
	private CollisionShape2D collisionShape;

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
		
		//GetParent().AddChild(playerBall);
		GetParent().CallDeferred(Node.MethodName.AddChild, new Variant[] {playerBall});
		this.playerBall = playerBall;

		active = true;

		SceneTreeTimer timer = GetTree().CreateTimer(30);
		timer.Timeout += OnTimeout;

		Thread thread = new Thread(() => {
			while (active)
			{
				GD.Print("bullet spawned");
				CallDeferred(MethodName.SpawnBullet);
				Thread.Sleep(5 * 1000);
			}
		});
		thread.Start();
	}

	private Vector2 GetRandomPoint()
	{
		Rect2 collisionRect = collisionShape.Shape.GetRect();

		float x = (float)GD.RandRange(Position.X - (collisionRect.Size.X / 2), Position.X + (collisionRect.Size.X / 2));
		float y = (float)GD.RandRange(Position.Y - (collisionRect.Size.Y / 2), Position.Y + (collisionRect.Size.Y / 2));
		//float x = (float)GD.RandRange(collisionRect.Position.X, collisionRect.End.X);
		//float y = (float)GD.RandRange(collisionRect.Position.Y, collisionRect.End.Y);

		Vector2 result = new Vector2(x, y);
		return result;
	}

	private void SpawnBullet()
	{
		Rect2 collisionRect = collisionShape.Shape.GetRect();

		Vector2 bulletPos = GetRandomPoint();
		bulletPos = new Vector2(bulletPos.X, Position.Y - (collisionRect.Size.Y/2) + 2);

		EnemyBullet bullet = EnemyBulletPackedScene.Instantiate<EnemyBullet>();
		bullet.DamagedPlayer += OnPlayerDamaged;
		bullet.Position = bulletPos;

		GetParent().CallDeferred("add_child", new Variant[] {bullet});
		bullets.Add(bullet);
	}

	private void Clear()
	{
		foreach (var bullet in bullets)
		{
			if (!bullet.IsQueuedForDeletion())
			{
				bullet.Dispose();
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
		EmitSignal(SignalName.Timeout);
	}

	public void OnPlayerDamaged(int value)
	{
		EmitSignal(SignalName.PlayerGotDamaged, new Variant[] {value});
	}
}
