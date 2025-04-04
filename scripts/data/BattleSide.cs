using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Displays;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class BattleSide : IEnumerable<Character>
    {
        public List<Character> Characters { get; private set; } = new();
        public List<CharacterBattleState> BattleStates { get; private set; } = new();

        public int Count => Characters.Count;

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

        public int GetRandomCharacter()
        {
            int index = (int)(GD.Randi() % Characters.Count);

            while (Characters[index].Health <= 0)
            {
                index = (int)(GD.Randi() % Characters.Count);
            }

            return index;
        }

        public CharacterBattleState GetCharacterState(Character character)
        {
            return BattleStates.Find(x => x.Character == character);
        }

        public CharacterBattleState GetCharacterState(int index)
        {
            return BattleStates[index];
        }

        public Character FromIndex(int index)
        {
            return Characters[index];
        }

        public virtual void RemoveCharacter(Character character)
        {
            int index = Characters.IndexOf(character);
            Characters.Remove(character);
            BattleStates.RemoveAt(index);
        }

        public void ChangeHealth(int index, int health)
        {
            Characters[index].ChangeHealth(health);
        }

        public void ChangeMana(int index, int mana)
        {
            Characters[index].ChangeMana(mana);
        }

        public void ApplyBattleEffect(int index, BattleEffect battleEffect)
        {
            if (BattleStates[index].HasBattleEffect)
            {
                return;
            }

            BattleStates[index].BattleEffect = battleEffect;
            BattleStates[index].HasBattleEffect = true;
            Characters[index].ApplyBattleEffect(battleEffect);
        }

        public void RemoveBattleEffect(int index)
        {
            BattleStates[index].HasBattleEffect = false;
            Characters[index].RemoveBattleEffect(BattleStates[index].BattleEffect);
            BattleStates[index].BattleEffect = null;
        }

        public void DefendCharacter(int index)
        {
            BattleStates[index].Action = CharacterAction.Defend;
        }

        public int GetTotalHealth()
        {
            int result = 0;
            foreach (Character character in Characters)
            {
                result += character.Health;
            }
            return result;
        }

        public void Clear()
        {
            Characters.Clear();
            BattleStates.Clear();
        }

        public IEnumerator<Character> GetEnumerator()
        {
            return ((IEnumerable<Character>)Characters).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Characters).GetEnumerator();
        }

        public Character this[int index] 
        {
            get { return Characters[index]; }
            set { Characters[index] = value; }
        }
    }
}
