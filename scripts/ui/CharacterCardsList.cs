using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWizardCoder.Data;
using TheWizardCoder.Displays;

namespace TheWizardCoder.UI
{
    public partial class CharacterCardsList : Control
    {
        [Signal]
        public delegate void CharacterCardPressedEventHandler(int index);

        [Export]
        public PackedScene CharacterRectScene { get; set; }
        public PackedScene DamageIndicatorScene { get; set; }
        public DamageIndicator DamageIndicator { get; set; }
        public Array<CharacterRect> Cards { get; private set; }
        public bool NegativeY { get; set; } = false;

        public override void _Ready()
        {
            base._Ready();

            Cards = new Array<CharacterRect>();
            DamageIndicator = GetNode<DamageIndicator>("DamageIndicator");
        }

        public void AddCharacter(Character character)
        {
            int currentIndex = Cards.Count;

            CharacterRect rect = CharacterRectScene.Instantiate<CharacterRect>();
            rect.Name = $"Card-{character.Name}";
            AddChild(rect);

            Vector2 vector = new Vector2(0, (currentIndex) * (rect.Size.Y + 4) * 2);
            if (NegativeY)
            {
                rect.Position = vector;
            }
            else
            {
                rect.Position = -vector;
            }

            rect.ApplyData(character);
            rect.Pressed += () => OnCharacterCardPressed(currentIndex);

            if (Cards.Count >= 1)
            {
                rect.FocusNeighborBottom = Cards[Cards.Count - 1].GetPath();
                Cards[Cards.Count - 1].FocusNeighborTop = rect.GetPath();

                rect.FocusNeighborTop = Cards[0].GetPath();
                Cards[0].FocusNeighborBottom = rect.GetPath();
            }


            Cards.Add(rect);
        }

        public void FocusOnFirst()
        {
            if (Cards.Count > 0)
            {
                Cards[0].GrabFocus();
            }
        }

        public async Task TweenDamage(int index, int healthChange)
        {
            Color backgroundColor = new(0, 255, 0);
            if (healthChange < 0)
            {
                backgroundColor = new(255, 0, 0);
            }

            Cards[index].SetHealthValue();
            DamageIndicator.PlayAnimation(healthChange, Cards[index].Position + new Vector2(64, 0), backgroundColor);
            await Cards[index].TweenDamage(backgroundColor);
        }

        public async Task TweenManaChange(int index, int change)
        {
            Color backgroundColor = new("ff30ff00");
       
            Cards[index].SetPointsValue();
            DamageIndicator.PlayAnimation(change, Cards[index].Position + new Vector2(64, 0), backgroundColor);
            await Cards[index].TweenDamage(backgroundColor);
        }

        public void SetManaValue(int index)
        {
            Cards[index].SetPointsValue();
        }

        public void OnCharacterCardPressed(int index)
        {
            EmitSignal(SignalName.CharacterCardPressed, index);
        }

        public void Clear()
        {
            foreach (var item in Cards)
            {
                item.QueueFree();
            }
            Cards.Clear();
        }

        public CharacterRect this[int index]
        {
            get { return Cards[index]; }
        }
    }
}
