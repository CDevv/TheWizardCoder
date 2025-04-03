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

        [Export]
        public PackedScene EnemySpriteScene { get; set; }

        public void AddSprite(Character character)
        {
            int currentIndex = sprites.Count;

            EnemySprite enemySprite = EnemySpriteScene.Instantiate<EnemySprite>();
            AddChild(enemySprite);

            enemySprite.Position = Position;
            enemySprite.ApplyData(character);
            enemySprite.ButtonPressed += () =>
            {
                //EmitSignal(SignalName.EnemyPressed, currentIndex);
            };

            sprites.Add(enemySprite);
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
