using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;
using TheWizardCoder.Subdisplays;

namespace TheWizardCoder.Abstractions
{
    /// <summary>
    /// A class that acts as container for information about participants during a battle.
    /// </summary>
    public abstract partial class CharactersContainer : Node
    {
        [Export]
        public BattleDisplay BattleDisplay { get; set; }
        [Export]
        public BattleOptions BattleOptions { get; set; }

        public List<Character> Characters { get; private set; } = new();
        public List<CharacterBattleState> BattleStates { get; set; } = new();
        public int CurrentCharacter { get; set; } = 0;
        protected Global global;

        public override void _Ready()
        {
            global = GetNode<Global>("/root/Global");
        }

        /// <summary>
        /// Add a character to this container.
        /// </summary>
        /// <param name="character">The <c>Character</c> to add</param>
        public virtual void AddCharacter(Character character)
        {
            int currentIndex = Characters.Count;

            Characters.Add(character);
            CharacterBattleState characterBattleState = new(character, currentIndex);
            BattleStates.Add(characterBattleState);
        }

        /// <summary>
        /// Start the turn for the participants that this container contains.
        /// </summary>
        public virtual void StartTurn()
        { }

        /// <summary>
        /// Pass to the next character.
        /// </summary>
        public async void PassToNext()
        {
            CurrentCharacter++;
            if (CurrentCharacter >= Characters.Count)
            {
                OnNextCharacterPassed();
                CurrentCharacter = 0;
                await BattleDisplay.Routine();
            }
            else
            {
                OnNextCharacterPassed();
                if (Characters[CurrentCharacter].Health > 0)
                {
                    StartTurn();
                }
                else
                {
                    CurrentCharacter++;

                    if (CurrentCharacter < Characters.Count)
                    {
                        StartTurn();
                    }
                    else
                    {
                        CurrentCharacter = 0;
                        await BattleDisplay.Routine();
                    }
                }
            }
        }

        public abstract void OnNextCharacterPassed();

        public abstract Task Turn(int index);

        public abstract Task DisplayHealthChange(int index, int change);
        public abstract Task DisplayManaChange(int index, int change);

        public abstract Task DisplayBattleEffect(int index);

        public abstract void HideBattleEffect(int index);

        /// <summary>
        /// Damage a <c>Character</c> in this container with the given <paramref name="index"/> and <paramref name="damage"/>
        /// </summary>
        /// <param name="index">The target's index</param>
        /// <param name="damage">The negative health change</param>
        public virtual async Task DamageCharacter(int index, int damage)
        {
            Characters[index].RemoveHealth(damage);
        }

        /// <summary>
        /// Heal a <c>Character</c> in this container with <paramref name="targetIndex"/> and <paramref name="addedHealth"/>
        /// </summary>
        /// <param name="targetIndex">The index of the <c>Character</c> to heal</param>
        /// <param name="addedHealth">Added health to the <c>Character</c></param>
        public async Task HealCharacter(int targetIndex, int addedHealth)
        {
            CharacterBattleState state = BattleStates[targetIndex];

            int healthChange = Mathf.Clamp(addedHealth, 0, state.Character.MaxHealth - state.Character.Health);
            Characters[state.Target].AddHealth(healthChange);

            await DisplayHealthChange(targetIndex, healthChange);

            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

        public async Task DefendCharacter(int index)
        {
            BattleOptions.ShowInfoLabel($"{Characters[index].Name} defends!");
            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

        public async Task AddManaToCharacter(int targetIndex, int addedMana)
        {
            CharacterBattleState state = BattleStates[targetIndex];

            int manaChange = Mathf.Clamp(addedMana, 0, state.Character.MaxPoints - state.Character.Points);
            Characters[targetIndex].Points += manaChange;

            await DisplayManaChange(targetIndex, manaChange);

            SceneTreeTimer timer = GetTree().CreateTimer(3);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }

        /// <summary>
        /// Apply a <c>BattleEffect</c> to a <c>Character</c> at the <paramref name="targetIndex"/>
        /// </summary>
        /// <param name="targetIndex">Index of the target <c>Character</c></param>
        /// <param name="effect">The <c>BattleEffect</c> object</param>
        /// <returns></returns>
        public async Task ApplyBattleEffect(int targetIndex, BattleEffect effect)
        {
            if (BattleStates[targetIndex].HasBattleEffect)
            {
                return;
            }

            BattleStates[targetIndex].BattleEffect = effect;
            BattleStates[targetIndex].HasBattleEffect = true;
            int multiplier = effect.IsNegative ? -1 : 1;

            GD.Print(effect.Effect);

            switch (effect.Action)
            {
                case Enums.BattleEffectType.Attack:
                    Characters[targetIndex].AttackPoints += effect.Effect * multiplier;
                    break;
                case Enums.BattleEffectType.Defense:
                    Characters[targetIndex].DefensePoints += effect.Effect * multiplier;
                    break;
                case Enums.BattleEffectType.Mana:
                    Characters[targetIndex].MaxPoints += effect.Effect * multiplier;
                    Characters[targetIndex].Points = Characters[targetIndex].MaxPoints;
                    break;
                case Enums.BattleEffectType.Speed:
                    Characters[targetIndex].AgilityPoints += effect.Effect * multiplier;
                    break;
                case Enums.BattleEffectType.Health:
                    Characters[targetIndex].MaxHealth += effect.Effect * multiplier;
                    Characters[targetIndex].Health = Characters[targetIndex].MaxHealth;
                    break;
            }

            GD.Print(Characters[targetIndex].DefensePoints);

            await DisplayBattleEffect(targetIndex);
        }

        /// <summary>
        /// Remove the <c>BattleEffect</c> of the <c>Character</c> at the specified index.
        /// </summary>
        /// <param name="targetIndex">Index of the target <c>Character</c></param>
        public void RemoveBattleEffect(int targetIndex)
        {
            BattleEffect effect = BattleStates[targetIndex].BattleEffect;
            int multiplier = effect.IsNegative ? 1 : -1;

            switch (effect.Action)
            {
                case Enums.BattleEffectType.Attack:
                    Characters[targetIndex].AttackPoints += effect.Effect * multiplier;
                    break;
                case Enums.BattleEffectType.Defense:
                    Characters[targetIndex].DefensePoints += effect.Effect * multiplier;
                    break;
                case Enums.BattleEffectType.Speed:
                    Characters[targetIndex].AgilityPoints += effect.Effect * multiplier;
                    break;
                case Enums.BattleEffectType.Health:
                    Characters[targetIndex].MaxHealth += effect.Effect * multiplier;
                    break;
                case Enums.BattleEffectType.Mana:
                    Characters[targetIndex].MaxPoints += effect.Effect * multiplier;
                    break;
            }

            BattleStates[targetIndex].BattleEffect = null;
            BattleStates[targetIndex].HasBattleEffect = false;

            HideBattleEffect(targetIndex);
        }

        /// <summary>
        /// Get the total health of every <c>Character</c> in this container.
        /// </summary>
        /// <returns>The total health of every character.</returns>
        public int GetTotalHealth()
        {
            int result = 0;
            foreach (var item in Characters)
            {
                result += item.Health;
            }
            return result;
        }

        /// <summary>
        /// Clear the <c>Character</c>s in this container.
        /// </summary>
        public virtual void Clear()
        {
            Characters = new();
            BattleStates = new();
        }
    }
}
