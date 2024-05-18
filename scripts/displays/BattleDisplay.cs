using DialogueManagerRuntime;
using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class BattleDisplay : CanvasLayer
{
	[Export]
	public Resource DialogueResource { get; set; }
	[Export]
	public PackedScene BulletPackedScene { get; set; }
	[Export]
	public PackedScene DamagablePackedScene { get; set; }

	private bool canUseDisplay = true;
	private bool playerIsAttacking = false;
	private Vector2 lastMousePosition;
	private float mouseChange;
	private int level = 0;
	private int enemyHealth = 100;
	private Global global;
	private AnimationPlayer animationPlayer;
	private Marker2D playerMarker;
	private Marker2D enemyMarker;
	private CharacterRect playerRect;
	private CharacterRect enemyRect;
	private BattleDialogue battleDialogue;
	private VBoxContainer optionsContainer;
	private Label itemDescription;
	private Button fightButton;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		playerMarker = GetNode<Marker2D>("PlayerMarker");
		enemyMarker = GetNode<Marker2D>("EnemyMarker");
		battleDialogue = GetNode<BattleDialogue>("%BattleDialogue");
		optionsContainer = GetNode<VBoxContainer>("%OptionsContainer");
		itemDescription = GetNode<Label>("%ItemDescription");
		playerRect = GetNode<CharacterRect>("PlayerRect");
		enemyRect = GetNode<CharacterRect>("EnemyRect");
		fightButton = GetNode<Button>("%FightButton");
	}

	public override void _Process(double delta)
	{
		if (playerIsAttacking)
		{
			Vector2 newMousePosition = GetViewport().GetMousePosition();
			float magnitude = (lastMousePosition - newMousePosition).Length();
			lastMousePosition = newMousePosition;
			mouseChange = Mathf.Clamp(magnitude, 0, 30);
			battleDialogue.SetFightPowerBarValue(mouseChange);
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!canUseDisplay)
		{
			return;
		}

		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (level == 1)
			{
				optionsContainer.Show();
				itemDescription.Hide();

				battleDialogue.ShowDialogueLabel();
				fightButton.GrabFocus();
				level = 0;
			}
		}
	}

	public async Task ShowDisplay()
	{
		playerRect.SetHealthValue(global.PlayerData.Health);

		Vector2 resolutionVector = new Vector2(global.Settings.WindowWidth, global.Settings.WindowHeight);
		Vector2 baseVector = global.CurrentRoom.Camera.GlobalPosition - (resolutionVector / 2);
		Vector2 playerFinalPosition = baseVector + playerMarker.Position;

		Show();
		animationPlayer.Play("battle_intro");

		global.CurrentRoom.Player.PlayIdleAnimation(Direction.Right);
		Tween posTween = global.CurrentRoom.Player.CreateTween();
		posTween.TweenProperty(global.CurrentRoom.Player, (string)Player.PropertyName.Position, playerFinalPosition, 1);
		await ToSignal(posTween, Tween.SignalName.Finished);

		battleDialogue.SetDialogueLine(await DialogueManager.GetNextDialogueLine(DialogueResource, "battle_intro"));

		fightButton.GrabFocus();

		InstantiateEnemyArea();
	}

	public void OnFightButton()
	{
		canUseDisplay = false;
		battleDialogue.ShowFightContainer();
		InitiatePlayerAttack();		
	}

	public void OnItemsButton()
	{
		level = 1;
		optionsContainer.Hide();
		itemDescription.Show();
		battleDialogue.ShowItems();
	}

	public void OnMagicButton()
	{
		level = 1;
		optionsContainer.Hide();
		itemDescription.Show();
		battleDialogue.ShowMagic();
	}

	public void OnDefenseButton()
	{
		canUseDisplay = false;
		animationPlayer.Play("hide_bottom");
	}

	public async void InitiatePlayerAttack()
	{
		lastMousePosition = GetViewport().GetMousePosition();
		playerIsAttacking = true;

		SceneTreeTimer timer = GetTree().CreateTimer(5);
		global.CurrentRoom.Player.PlaySideAnimation("attack_intro");
		Thread attackAnimationThread = new Thread(async () => {
			await ToSignal(global.CurrentRoom.Player, Player.SignalName.AnimationFinished);
			//global.CurrentRoom.Player.PlayIdleAnimation(Direction.Right);
			global.CurrentRoom.Player.PlaySideAnimation("attack_idle");
		});

		await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		playerIsAttacking = false;
		
		GD.Print(mouseChange);
		float bulletScale = (mouseChange - 0f) * (1.5f - 0.5f) / (30f - 0f) + 0.5f;

		global.CurrentRoom.Player.PlaySideAnimation("attack_idle2");
		animationPlayer.Play("hide_bottom");

		//Spawn player bullet
		Vector2 resolutionVector = new Vector2(global.Settings.WindowWidth, global.Settings.WindowHeight);
		Vector2 finalVector = global.CurrentRoom.Camera.GlobalPosition - (resolutionVector / 2) + playerMarker.Position;

		Bullet bullet = BulletPackedScene.Instantiate<Bullet>();
		bullet.Position = finalVector;
		bullet.Damage *= bulletScale;
		bullet.Damage = Mathf.Ceil(bullet.Damage);
		bullet.Scale = new Vector2(bulletScale, bulletScale);
		global.CurrentRoom.AddChild(bullet);

		GD.Print(bullet.Damage);
	}

	public void InstantiateEnemyArea()
	{
		Vector2 resolutionVector = new Vector2(global.Settings.WindowWidth, global.Settings.WindowHeight);
		Vector2 pos = global.CurrentRoom.Camera.GlobalPosition - (resolutionVector / 2) + enemyMarker.Position;

		Damagable enemyArea = DamagablePackedScene.Instantiate<Damagable>();
		enemyArea.Position = pos;
		enemyArea.Damaged += OnEnemyDamaged;
		global.CurrentRoom.AddChild(enemyArea);
	}

	public void OnItemTriggered(string itemName)
	{
		global.PlayerData.Health -= global.ItemDescriptions[itemName].Effect;
		playerRect.TweenHealthValue(global.PlayerData.Health);

		animationPlayer.Play("hide_bottom");
	}

	public void OnEnemyDamaged(int value)
	{
		enemyHealth -= value;
		enemyRect.TweenHealthValue(enemyHealth);
	}
}
