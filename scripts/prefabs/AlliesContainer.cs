using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class AlliesContainer : Node
{
	[Signal]
	public delegate void AllyPressedEventHandler(int index);

	[Export]
	public EnemiesContainer Enemies { get; set; }
	[Export]
	public DamageIndicator DamageIndicator { get; set; }
	[Export]
	public BattleOptions BattleOptions { get; set; }
	[Export]
	public BattleDisplay BattleDisplay { get; set; }
	[Export]
	public Marker2D BaseCardPosition { get; set; }
	[Export]
	public PackedScene CharacterRectScene { get; set; }

	private Global global;
	private Vector2 startingPoint = Vector2.Zero;
	private int currentCharacter = 0;
	private Array<CharacterRect> alliesCards = new();
	private Array<CharacterData> allies = new();
	private List<CharacterBattleState> battleStates = new();

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
		startingPoint = BaseCardPosition.Position;
	}

	public void AddAlly(CharacterData data)
	{
		int currentIndex = allies.Count;

		CharacterRect rect = CharacterRectScene.Instantiate<CharacterRect>();
		rect.Position = startingPoint - new Vector2(0, allies.Count * (rect.Size.Y + 4) * 2);
		GD.Print(allies.Count * (rect.Size.Y));
		BattleDisplay.AddChild(rect);
		rect.ApplyData(data);
		rect.Pressed += () => OnCharacterCardPressed(currentIndex);
		alliesCards.Add(rect);

		allies.Add(data);
		CharacterBattleState state = new();
		battleStates.Add(state);
	}

	public void StartTurn()
	{
		alliesCards[currentCharacter].ShowAsCurrentCharacter();
		BattleOptions.UpdateDisplay(allies[currentCharacter]);
		BattleOptions.ShowOptions();
	}

	public void FocusOnFirst()
	{
		alliesCards[0].GrabFocus();
	}

	public async void PassToNextAlly()
	{
		currentCharacter++;
		if (currentCharacter >= allies.Count)
		{
			alliesCards[currentCharacter-1].HideBackground();
			currentCharacter = 0;
			await BattleDisplay.Routine();
		}
		else
		{
			alliesCards[currentCharacter-1].HideBackground();
			StartTurn();
		}
	}

	public async Task AlliesTurn()
	{
		for (int i = 0; i < allies.Count; i++)
		{
			if (BattleDisplay.BattleEnded)
			{
				break;
			}

			CharacterData ally = allies[i];
			switch (battleStates[i].Action)
			{
				case CharacterAction.Attack:
					BattleOptions.ShowInfoLabel($"{ally.Name} attacks {Enemies.GetEnemyName(battleStates[i].Target)}!");
					await Enemies.DamageEnemy(battleStates[i].Target, allies[i].AttackPoints);
					break;
				case CharacterAction.Defend:
					await DefendAlly(i);
					break;
				case CharacterAction.Items:
					await HealAlly(i);
					break;
				case CharacterAction.Magic:
					string currentMagicSpellName = allies[i].MagicSpells[battleStates[i].ActionModifier];
					MagicSpell currentMagicSpell = global.MagicSpells[currentMagicSpellName];
					if (currentMagicSpell.TargetType == TargetType.Enemy)
					{
						string enemyName = Enemies.GetEnemyName(battleStates[i].Target);
						BattleOptions.ShowInfoLabel($"{ally.Name} casted {currentMagicSpellName} on {enemyName}!");
						await Enemies.DamageEnemy(battleStates[i].Target, currentMagicSpell.Effect);
					}
					break;
			}

			if (Enemies.GetTotalHealth() <= 0)
			{
				BattleDisplay.HideDisplay();
			}
		}
	}

	private async Task DefendAlly(int index)
	{
		BattleOptions.ShowInfoLabel($"{allies[index].Name} defends!");
		SceneTreeTimer timer = GetTree().CreateTimer(3);
		await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
	}

	public async Task HealAlly(int index)
	{
		CharacterBattleState state = battleStates[index];
		string itemName = global.PlayerData.Inventory[state.ActionModifier];
		Item item = global.ItemDescriptions[itemName];
		CharacterData target = allies[state.Target];
		if (state.Target == index)
		{
			BattleOptions.ShowInfoLabel($"{allies[index].Name} gave {itemName} to themselves!");
		}
		else
		{
			BattleOptions.ShowInfoLabel($"{allies[index].Name} gave {itemName} to {target.Name}!");
		}
		
		int newHealth = Mathf.Clamp(target.Health + item.Effect, target.Health, target.MaxHealth);
		allies[state.Target].Health = newHealth;

		alliesCards[state.Target].TweenDamage(new Color(0, 255, 0));
		DamageIndicator.PlayAnimation(target.MaxHealth - newHealth, alliesCards[state.Target].Position + new Vector2(64, 0), new Color(0, 255, 0));
		
		alliesCards[state.Target].SetHealthValue(newHealth);

		global.PlayerData.RemoveFromInventory(state.ActionModifier);
		
		SceneTreeTimer timer = GetTree().CreateTimer(3);
		await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
	}

	public async Task DamageRandomAlly(string enemyName, int damage)
	{
		int targetIndex = (int)(GD.Randi() % allies.Count);

		if (battleStates[targetIndex].Action == CharacterAction.Defend)
		{
			damage = Mathf.Clamp(damage - allies[targetIndex].DefensePoints, 0, damage);
		}
		allies[targetIndex].Health -= damage;

		//Visuals
		BattleOptions.ShowInfoLabel($"{enemyName} attacks {allies[targetIndex].Name}!");
		DamageIndicator.PlayAnimation(damage, alliesCards[targetIndex].Position + new Vector2(64, 0), new Color(255, 0, 0));
		alliesCards[targetIndex].SetHealthValue(global.PlayerData.Stats.Health);
		await alliesCards[targetIndex].TweenDamage(new Color(255, 0, 0));
	}

	private void OnCharacterCardPressed(int index)
	{
		battleStates[currentCharacter].Target = index;
		PassToNextAlly();
		EmitSignal(SignalName.AllyPressed, index);
	}

	private void OnEnemyPressed(int index)
	{
		battleStates[currentCharacter].Target = index;
		if (battleStates[currentCharacter].Action == CharacterAction.Magic)
		{
			int indexInInventory = battleStates[currentCharacter].ActionModifier;
			
			string magicSpellName = allies[currentCharacter].MagicSpells[indexInInventory];
			MagicSpell magicSpell = global.MagicSpells[magicSpellName];

			allies[currentCharacter].Points -= magicSpell.Cost;
			alliesCards[currentCharacter].SetPointsValue(allies[currentCharacter].Points);
		}

		PassToNextAlly();
	}

	private void OnAttackButton()
	{
		battleStates[currentCharacter].Action = CharacterAction.Attack;
		Enemies.FocusOnFirst();
		BattleOptions.ShowInfoLabel("Select an enemy!");
	}

	private void OnDefendButton()
	{
		battleStates[currentCharacter].Action = CharacterAction.Defend;
		PassToNextAlly();
	}

	private void OnItemButton(int index)
	{
		battleStates[currentCharacter].Action = CharacterAction.Items;
		battleStates[currentCharacter].ActionModifier = index;
		alliesCards[0].GrabFocus();
		BattleOptions.ShowInfoLabel("Select an ally!");
	}

	private void OnMagicButton(int index)
	{
		CharacterData ally = allies[currentCharacter];

		battleStates[currentCharacter].Action = CharacterAction.Magic;
		battleStates[currentCharacter].ActionModifier = index;

		string magicSpellName = ally.MagicSpells[index];
		MagicSpell magicSpell = global.MagicSpells[magicSpellName];
		if (magicSpell.TargetType == TargetType.Enemy)
		{
			Enemies.FocusOnFirst();
			BattleOptions.ShowInfoLabel("Select an enemy to cast on!");
		}
	}

	public int GetTotalHealth()
	{
		int result = 0;
		foreach (CharacterData ally in allies)
		{
			result += ally.Health;
		}
		return result;
	}

	public void Clear()
	{
		allies.Clear();
		alliesCards.Clear();
		battleStates.Clear();
	}
}
