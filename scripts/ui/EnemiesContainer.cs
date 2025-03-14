using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;
using TheWizardCoder.Enums;

namespace TheWizardCoder.UI
{
    public partial class EnemiesContainer : CharactersContainer
    {
        [Signal]
        public delegate void EnemyPressedEventHandler(int index);

        [Export]
        public AlliesContainer Allies { get; set; }
        [Export]
        public EnemyHealthBar EnemyHealthBar { get; set; }
        [Export]
        public DamageIndicator DamageIndicator { get; set; }
        [Export]
        public Marker2D BaseEnemyPosition { get; set; }
        [Export]
        public Marker2D BaseEnemyCardPosition { get; set; }
        [Export]
        public PackedScene EnemySpriteScene { get; set; }
        [Export]
        public PackedScene CharacterRectScene { get; set; }

        private Vector2 enemySpritePoint = Vector2.Zero;
        private Vector2 enemyCardPoint = Vector2.Zero;

        private Array<EnemySprite> enemySprites = new();
        private Array<CharacterRect> enemyCards = new();

        public override void _Ready()
        {
            base._Ready();
            enemySpritePoint = BaseEnemyPosition.Position;
            enemyCardPoint = BaseEnemyCardPosition.Position;
        }

        public override void AddCharacter(Character character)
        {
            int currentIndex = Characters.Count;

            Characters.Add(character.Clone());
            CharacterBattleState characterBattleState = new(character, currentIndex);
            BattleStates.Add(characterBattleState);

            EnemySprite enemySprite = EnemySpriteScene.Instantiate<EnemySprite>();
            BattleDisplay.AddChild(enemySprite);

            enemySprite.Position = enemySpritePoint;
            enemySprite.ApplyData(character);
            enemySprite.ButtonPressed += () =>
            {
                EmitSignal(SignalName.EnemyPressed, currentIndex);
            };

            enemySprites.Add(enemySprite);

            CharacterRect enemyRect = CharacterRectScene.Instantiate<CharacterRect>();
            BattleDisplay.AddChild(enemyRect);

            enemyRect.Position = enemyCardPoint + new Vector2(0, currentIndex * (enemyRect.Size.Y + 4) * 2);
            enemyRect.ApplyData(character);

            enemyCards.Add(enemyRect);
        }

        public void FocusOnFirst()
        {
            enemySprites[0].GrabFocus();
        }

        public override async Task Turn(int index)
        {
            if (BattleDisplay.IsBattleEnded)
            {
                return;
            }

            int targetIndex = (int)(GD.Randi() % Allies.Characters.Count);
            string targetName = Allies.Characters[targetIndex].Name;

            Character currentEnemy = Characters[index];
            string enemyName = currentEnemy.Name;

            CharacterAction action = currentEnemy.ChooseBehaviour();

            switch (action)
            {
                case CharacterAction.Attack:
                    BattleOptions.ShowInfoLabel($"{enemyName} attacks {targetName}!");
                    await Allies.DamageCharacter(targetIndex, currentEnemy.AttackPoints);

                    break;
                case CharacterAction.Defend:
                    break;
                case CharacterAction.Items:
                    break;
                case CharacterAction.Magic:
                    string currentMagicSpellName = currentEnemy.GetRandomMagicSpell();
                    MagicSpell currentMagicSpell = global.MagicSpells[currentMagicSpellName];



                    switch (currentMagicSpell.TargetType)
                    {
                        case CharacterType.Enemy:
                            await OnMagicSpellEnemyTarget(index, targetIndex, currentMagicSpell);

                            break;
                        case CharacterType.Ally:
                            await OnMagicSpellAllyTarget(index, targetIndex, currentMagicSpell);

                            break;
                    }

                    break;
                default:
                    break;
            }
        }

        private async Task OnMagicSpellAllyTarget(int currentEnemyIndex, int targetIndex, MagicSpell spell)
        {
            string targetName = Allies.Characters[targetIndex].Name;

            Character currentEnemy = Characters[currentEnemyIndex];
            string enemyName = currentEnemy.Name;

            switch (spell.SpellType)
            {
                case MagicSpellType.Attack:
                    break;
                case MagicSpellType.Heal:
                    break;
                case MagicSpellType.ApplyBattleEffect:
                    break;
            }
        }

        private async Task OnMagicSpellEnemyTarget(int currentEnemyIndex, int targetIndex, MagicSpell spell)
        {
            string targetName = Allies.Characters[targetIndex].Name;

            Character currentEnemy = Characters[currentEnemyIndex];
            string enemyName = currentEnemy.Name;

            BattleOptions.ShowInfoLabel($"{enemyName} casted {spell.Name} on {targetName}!");
            switch (spell.SpellType)
            {
                case MagicSpellType.Attack:
                    await Allies.DamageCharacter(targetIndex, spell.Effect);

                    break;
                case MagicSpellType.Heal:
                    break;
                case MagicSpellType.ApplyBattleEffect:
                    if (Allies.BattleStates[targetIndex].HasBattleEffect)
                    {
                        await Allies.DamageCharacter(targetIndex, spell.Effect);
                    }
                    else
                    {
                        await Allies.ApplyBattleEffect(targetIndex, spell.BattleEffect);
                    }

                    break;
            }
        }

        public override void OnNextCharacterPassed()
        {
            throw new NotImplementedException();
        }

        public override async Task DamageCharacter(int index, int damage)
        {
            await base.DamageCharacter(index, damage);
            await DisplayHealthChange(index, damage);
        }

        public override async Task DisplayHealthChange(int index, int change)
        {
            Character enemyData = Characters[index];
            EnemySprite sprite = enemySprites[index];

            enemyCards[index].SetHealthValue(enemyData.Health, enemyData.MaxHealth);

            DamageIndicator.PlayAnimation(change, sprite.Position - new Vector2(DamageIndicator.Size.X, 10), new Color(255, 0, 0));

            Vector2 barPosition = sprite.Position - new Vector2(EnemyHealthBar.Size.X / 2, -20);
            EnemyHealthBar.Position = barPosition;
            await EnemyHealthBar.ShowHealthBar(enemyData.Health + change, enemyData.Health, enemyData.MaxHealth);
        }

        public override Task DisplayBattleEffect(int index)
        {
            throw new NotImplementedException();
        }

        public override void HideBattleEffect(int index)
        {
            throw new NotImplementedException();
        }

        public override Task DisplayManaChange(int index, int change)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            base.Clear();
            foreach (var item in enemySprites)
            {
                item.QueueFree();
            }
            enemySprites = new();

            foreach (var item in enemyCards)
            {
                item.QueueFree();
            }
            enemyCards = new();
        }
    }
}