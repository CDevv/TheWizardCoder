using DialogueManagerRuntime;
using Godot;
using Godot.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class BattleDisplay : CanvasLayer
{
	[Export]
	public string BattleName { get; set; }
	[Export]
	public PackedScene BulletPackedScene { get; set; }
	[Export]
	public PackedScene DamagablePackedScene { get; set; }
	[Export]
	public PackedScene CombatAreaPackedScene { get; set; }
	[Export]
	public PackedScene EnemyBattleAttack { get; set; }

	private Vector2 playerPreviousPosition;
	private bool canUseDisplay = true;
	private bool playerIsAttacking = false;
	private Vector2 lastMousePosition;
	private float mouseChange;
	private int level = 0;
	private int enemyHealth = 20;
	private string equippedMagicSpell = "";
	private bool playerIsDefending = false;
	private Global global;
	private TransitionRect transition;
	private AnimationPlayer animationPlayer;
	private Marker2D playerMarker;
	private Marker2D enemyMarker;
	private Marker2D areaMarker;
	private CharacterRect playerRect;
	private CharacterRect enemyRect;
	private BattleOptions battleOptions;
	private BattleDialogue battleDialogue;
	private NinePatchRect areaBorder;
	private CombatArea combatArea;
	private BattleInfo battleInfo;
	
	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		transition = GetNode<TransitionRect>("TransitionRect");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		playerMarker = GetNode<Marker2D>("PlayerMarker");
		enemyMarker = GetNode<Marker2D>("EnemyMarker");
		areaMarker = GetNode<Marker2D>("CombatAreaMarker");
		battleDialogue = GetNode<BattleDialogue>("%BattleDialogue");
		battleOptions = GetNode<BattleOptions>("BattleOptions");
		playerRect = GetNode<CharacterRect>("PlayerRect");
		enemyRect = GetNode<CharacterRect>("EnemyRect");
		areaBorder = GetNode<NinePatchRect>("AreaBorder");
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
		if (canUseDisplay)
		{
			if (Input.IsActionJustPressed("ui_cancel"))
			{
				if (level == 1)
				{
					battleOptions.ShowOptions();
					battleOptions.FocusFirst();
					battleDialogue.ShowDialogueLabel();
					level = 0;
				}
			}
		}
		else
		{
			if (Input.IsActionJustPressed("magic"))
			{
				if (equippedMagicSpell != "")
				{
					string methodName = global.MagicSpells[equippedMagicSpell].MethodName;
					combatArea.PlayerBall.Call(methodName);
				}
			}
		}
	}

	public async Task ShowDisplay()
	{
		ShowAll();

		battleInfo = new BattleInfo();
		battleInfo.Setup(BattleName);

		playerRect.SetHealthValue(global.PlayerData.Health);

		playerPreviousPosition = global.CurrentRoom.Player.Position;
		Vector2 playerFinalPosition = global.GetCameraBaseVector() + playerMarker.Position;

		Show();
		animationPlayer.Play("battle_intro");

		global.CurrentRoom.Player.PlayIdleAnimation(Direction.Right);
		Tween posTween = global.CurrentRoom.Player.CreateTween();
		posTween.TweenProperty(global.CurrentRoom.Player, (string)Player.PropertyName.Position, playerFinalPosition, 1);
		await ToSignal(posTween, Tween.SignalName.Finished);

		battleDialogue.SetDialogueLine(await DialogueManager.GetNextDialogueLine(battleInfo.DialogueResource, battleInfo.AttackNames[0].ToLower()));

		battleOptions.FocusFirst();

		InstantiateEnemyArea();
	}

	public async void InitiatePlayerAttack()
	{
		lastMousePosition = GetViewport().GetMousePosition();
		playerIsAttacking = true;

		SceneTreeTimer timer = GetTree().CreateTimer(5);
		global.CurrentRoom.Player.PlaySideAnimation("attack_intro");
		Thread thread = new Thread(async () => {
			await ToSignal(global.CurrentRoom.Player, Player.SignalName.AnimationFinished);
			global.CurrentRoom.Player.CallDeferred(Player.MethodName.PlaySideAnimation, "attack_idle");
		});

		await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		playerIsAttacking = false;
		
		GD.Print(mouseChange);
		float bulletScale = (mouseChange - 0f) * (1.5f - 0.5f) / (30f - 0f) + 0.5f;

		global.CurrentRoom.Player.PlaySideAnimation("attack_idle2");
		animationPlayer.Play("hide_bottom");

		//Spawn player bullet
		Vector2 finalVector = global.GetCameraBaseVector() + playerMarker.Position;

		Bullet bullet = BulletPackedScene.Instantiate<Bullet>();
		bullet.Position = finalVector;
		bullet.Damage *= bulletScale;
		bullet.Damage = Mathf.Ceil(bullet.Damage);
		bullet.Scale = new Vector2(bulletScale, bulletScale);
		global.CurrentRoom.AddChild(bullet);

		GD.Print(bullet.Damage);
	}

	public void InitiateEnemyAttack()
	{
		//Remove magic spell if it is OneTime
		if (equippedMagicSpell != "")
		{
			if (global.MagicSpells[equippedMagicSpell].OneTime)
			{
				global.PlayerData.RemoveMagicSpell(equippedMagicSpell);
			}
		}		

		//Initiate
		areaBorder.Show();
		combatArea.DoAttack();
	}

	public void InstantiateEnemyArea()
	{
		Vector2 baseVector = global.GetCameraBaseVector();
		Vector2 damagablePos = baseVector + enemyMarker.Position;
		Vector2 areaPos = baseVector + areaMarker.Position;

		Damagable enemyArea = DamagablePackedScene.Instantiate<Damagable>();
		enemyArea.Health = enemyHealth;
		enemyArea.Position = damagablePos;
		enemyArea.Died += OnEnemyDied;
		enemyArea.Damaged += OnEnemyDamaged;
		global.CurrentRoom.AddChild(enemyArea);

		CombatArea combatArea = CombatAreaPackedScene.Instantiate<CombatArea>();
		combatArea.Position = areaPos;
		combatArea.EnemyAttackPackedScene = EnemyBattleAttack;
		combatArea.BattleInfo = battleInfo;
		combatArea.Timeout += OnEnemyAttackFinished;
		combatArea.PlayerGotDamaged += OnPlayerDamaged;
		global.CurrentRoom.AddChild(combatArea);
		this.combatArea = combatArea;
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
		battleOptions.ShowItemDescription();
		battleDialogue.ShowItems();
	}

	public void OnMagicButton()
	{
		level = 1;
		battleOptions.ShowItemDescription();
		battleDialogue.ShowMagic();
	}

	public void OnDefenseButton()
	{
		playerIsDefending = true;
		canUseDisplay = false;
		animationPlayer.Play("hide_bottom");
		InitiateEnemyAttack();
	}

	
	public void OnItemTriggered(string itemName)
	{
		global.PlayerData.Health -= global.ItemDescriptions[itemName].Effect;
		playerRect.TweenHealthValue(global.PlayerData.Health);

		animationPlayer.Play("hide_bottom");
	}

	public void OnMagicSpellTriggered(string spellName)
	{
		equippedMagicSpell = spellName;
	}

	public void OnEnemyDamaged(int value)
	{
		global.CurrentRoom.Player.PlayIdleAnimation(Direction.Right);
		enemyHealth -= value;
		enemyRect.TweenHealthValue(enemyHealth);
		InitiateEnemyAttack();
	}

	public async void OnEnemyDied()
	{
		enemyHealth = 0;
		enemyRect.TweenHealthValue(0);
		battleDialogue.ShowDialogueLabel();
		battleDialogue.SetText("You won!");
		animationPlayer.Play("show_bottom");

		SceneTreeTimer textCooldown = GetTree().CreateTimer(2);
		await ToSignal(textCooldown, SceneTreeTimer.SignalName.Timeout);
		transition.Show();
		transition.PlayAnimation();

		await ToSignal(transition, TransitionRect.SignalName.AnimationFinished);		
		global.CurrentRoom.Player.Position = playerPreviousPosition;
		global.CurrentRoom.Player.PlayIdleAnimation(Direction.Down);
		global.CanWalk = true;
		HideAll();

		transition.PlayAnimationBackwards();
		await ToSignal(transition, TransitionRect.SignalName.AnimationFinished);	
		Hide();
		transition.Hide();
	}

	public void OnEnemyAttackFinished()
	{
		playerIsDefending = false;
		areaBorder.Hide();
		animationPlayer.Play("show_bottom");
		battleOptions.FocusFirst();
		battleDialogue.ShowDialogueLabel();
		canUseDisplay = true;
	}

	public void OnPlayerDamaged(int value)
	{
		if (playerIsDefending)
		{
			value = (int)Math.Ceiling(value * 0.7);
		}
		global.PlayerData.Health -= value;
		playerRect.TweenHealthValue(global.PlayerData.Health);
	}

	private void ShowAll()
	{
		playerRect.Show();
		enemyRect.Show();
		battleDialogue.Show();
		battleOptions.Show();
	}

	private void HideAll()
	{
		playerRect.Hide();
		enemyRect.Hide();
		battleDialogue.Hide();
		battleOptions.Hide();
	}
}
