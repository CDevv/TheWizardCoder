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

	private bool canUseDisplay = true;
	private bool playerIsAttacking = false;
	private Vector2 lastMousePosition;
	private float mouseChange;
	private int level = 0;
	private int enemyHealth = 100;
	private string equippedMagicSpell = "";
	private Global global;
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
		battleInfo = new BattleInfo();
		battleInfo.Setup(BattleName);

		playerRect.SetHealthValue(global.PlayerData.Health);

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
			global.CurrentRoom.Player.PlaySideAnimation("attack_idle");
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
		enemyArea.Position = damagablePos;
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
		enemyHealth -= value;
		enemyRect.TweenHealthValue(enemyHealth);
		InitiateEnemyAttack();
	}

	public void OnEnemyAttackFinished()
	{
		areaBorder.Hide();
		animationPlayer.Play("show_bottom");
		battleOptions.FocusFirst();
		battleDialogue.ShowDialogueLabel();
		canUseDisplay = true;
	}

	public void OnPlayerDamaged(int value)
	{
		global.PlayerData.Health -= value;
		playerRect.TweenHealthValue(global.PlayerData.Health);
	}
}
