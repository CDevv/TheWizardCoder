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

	private bool playerIsAttacking = false;
	private Vector2 lastMousePosition;
	private float mouseChange;
	private int level = 0;
	private Global global;
	private AnimationPlayer animationPlayer;
	private Marker2D playerMarker;
	private Marker2D enemyMarker;
	private CharacterRect playerRect;
	private CharacterRect enemyRect;
	private BattleDialogue battleDialogue;
	private Button fightButton;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		playerMarker = GetNode<Marker2D>("PlayerMarker");
		battleDialogue = GetNode<BattleDialogue>("%BattleDialogue");
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
			mouseChange = magnitude;
			battleDialogue.SetText($"Fight is WIP, {magnitude}");
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (level == 1)
			{
				battleDialogue.ShowDialogueLabel();
				fightButton.GrabFocus();
				level = 0;
			}
		}
	}

	public async Task ShowDisplay()
	{
		Vector2 resolutionVector = new Vector2(global.Settings.WindowWidth, global.Settings.WindowHeight);
		Vector2 playerFinalPosition = global.CurrentRoom.Camera.GlobalPosition - (resolutionVector / 2) + playerMarker.Position;

		Show();
		animationPlayer.Play("battle_intro");

		global.CurrentRoom.Player.PlayIdleAnimation(Direction.Right);
		Tween posTween = global.CurrentRoom.Player.CreateTween();
		posTween.TweenProperty(global.CurrentRoom.Player, (string)Player.PropertyName.Position, playerFinalPosition, 1);
		await ToSignal(posTween, Tween.SignalName.Finished);

		battleDialogue.SetDialogueLine(await DialogueManager.GetNextDialogueLine(DialogueResource, "battle_intro"));
		//dialogueMainText = battleDialogue.Text;

		fightButton.GrabFocus();
	}

	public void OnFightButton()
	{
		battleDialogue.SetText("Fight is WIP.");
		InitiatePlayerAttack();
	}

	public void OnItemsButton()
	{
		level = 1;
		battleDialogue.ShowItems();
	}

	public void OnMagicButton()
	{
		level = 1;
		battleDialogue.ShowMagic();
	}

	public void OnDefenseButton()
	{
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
		
		mouseChange = Mathf.Clamp(mouseChange, 0, 25);
		GD.Print(mouseChange);
		float bulletScale = (mouseChange - 0f) * (1.5f - 0.5f) / (25f - 0f) + 0.5f;

		global.CurrentRoom.Player.PlaySideAnimation("attack_idle2");
		animationPlayer.Play("hide_bottom");

		//Spawn player bullet
		Vector2 resolutionVector = new Vector2(global.Settings.WindowWidth, global.Settings.WindowHeight);
		Vector2 finalVector = global.CurrentRoom.Camera.GlobalPosition - (resolutionVector / 2) + playerMarker.Position;

		Bullet bullet = BulletPackedScene.Instantiate<Bullet>();
		bullet.Position = finalVector;
		bullet.Scale = new Vector2(bulletScale, bulletScale);
		global.CurrentRoom.AddChild(bullet);
	}
}
