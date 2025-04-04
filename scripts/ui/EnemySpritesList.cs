using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;

namespace TheWizardCoder.UI
{
    public partial class EnemySpritesList : Control, IEnumerable<EnemySprite>
    {
        [Signal]
        public delegate void EnemyPressedEventHandler(int index);

        private Array<EnemySprite> sprites;
        private EnemyHealthBar healthBar;

        [Export]
        public PackedScene EnemySpriteScene { get; set; }

        public override void _Ready()
        {
            base._Ready();
            sprites = new Array<EnemySprite>();
            healthBar = GetNode<EnemyHealthBar>("HealthBar");
        }

        public void AddSprite(Character character)
        {
            int currentIndex = sprites.Count;

            EnemySprite enemySprite = EnemySpriteScene.Instantiate<EnemySprite>();
            AddChild(enemySprite);

            enemySprite.Position = Vector2.Zero;
            enemySprite.ApplyData(character);
            enemySprite.ButtonPressed += () => OnEnemySpritePressed(currentIndex);

            sprites.Add(enemySprite);
        }

        public void ShowHealthBar(int index, int change, Character character)
        {
            Vector2 baseSize = healthBar.Size;
            Vector2 offset = new(baseSize.X / 2, baseSize.Y / 2);
            healthBar.Position = sprites[index].Position - offset;
            healthBar.ShowHealthBar(character.Health, character.Health + change, character.MaxHealth);
        }

        public void FocusOnFirst()
        {
            sprites[0].GrabFocus();
        }

        public void Clear()
        {
            foreach (var item in sprites)
            {
                item.QueueFree();
            }
            sprites.Clear();
        }

        public void OnEnemySpritePressed(int index)
        {
            EmitSignal(SignalName.EnemyPressed, index);
        }

        public IEnumerator<EnemySprite> GetEnumerator()
        {
            return sprites.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)sprites).GetEnumerator();
        }

        public EnemySprite this[int index]
        {
            get { return sprites[index]; }
        }
    }
}
