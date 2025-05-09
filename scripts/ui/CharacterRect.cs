using Godot;
using System.Threading.Tasks;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;

namespace TheWizardCoder.UI
{
    public partial class CharacterRect : NinePatchRect
    {
        [Signal]
        public delegate void PressedEventHandler();

        private Color currentCharacterColor = new(255, 255, 255, 0.2f);
        private Color invisibleColor = new(255, 255, 255, 0);
        private Color focusColor = new(255, 255, 255, 0.5f);
        private AnimatedSprite2D sprite;
        private Label nameLabel;
        private Label healthLabel;
        private Label pointsLabel;
        private TextureProgressBar healthBar;
        private ColorRect background;
        private AnimatedSprite2D selectArrow;
        private AnimatedSprite2D effectIndicator;
        private Label effectTurns;
        private bool focused = false;

        private Character character;

        public override void _Ready()
        {
            FocusMode = FocusModeEnum.All;
            sprite = GetNode<AnimatedSprite2D>("Sprite");
            nameLabel = GetNode<Label>("NameLabel");
            background = GetNode<ColorRect>("BackgroundColor");
            healthLabel = GetNode<Label>("HealthLabel");
            pointsLabel = GetNode<Label>("PointsLabel");
            healthBar = GetNode<TextureProgressBar>("HealthBar");
            selectArrow = GetNode<AnimatedSprite2D>("SelectArrow");
            effectIndicator = GetNode<AnimatedSprite2D>("EffectIndicator");
            effectTurns = GetNode<Label>("EffectTurns");
        }

        public void ApplyData(Character data)
        {
            character = data;

            SetNameText(data.Name);
            SetHealthValue(data.Health, data.MaxHealth);
            SetPointsValue(data.Points);
            SetSpriteTexture(data.Name);
        }

        private void OnGuiInput(InputEvent inputEvent)
        {
            if (Input.IsActionJustPressed("ui_accept") && focused)
            {
                EmitSignal(SignalName.Pressed);
            }
        }

        private void SetSpriteTexture(string characterName)
        {
            sprite.Play(characterName);
        }

        private void SetNameText(string text)
        {
            nameLabel.Text = text;
        }

        public void SetHealthValue(int value, int maxValue)
        {
            healthLabel.Text = $"{character.Health}/{character.MaxHealth} HP";

            healthBar.MaxValue = maxValue;
            healthBar.Value = value;
        }

        public void SetHealthValue()
        {
            healthLabel.Text = $"{character.Health}/{character.MaxHealth} HP";

            healthBar.MaxValue = character.MaxHealth;
            healthBar.Value = character.Health;
        }

        public void SetPointsValue(int value)
        {
            pointsLabel.Text = $"{value} MP";
        }

        public void SetPointsValue()
        {
            pointsLabel.Text = $"{character.Points} MP";
        }

        public void ShowAsCurrentCharacter()
        {
            background.Modulate = currentCharacterColor;
        }

        public void HideBackground()
        {
            background.Modulate = invisibleColor;
        }

        public void ShowEffectIndicator(BattleEffectType type, int effect, bool isNegative)
        {
            if (!isNegative)
            {
                effectIndicator.Animation = "effect+";
            }
            else
            {
                effectIndicator.Animation = "effect-";
            }
            effectIndicator.Frame = (int)type;

            effectIndicator.Show();
            effectTurns.Show();
        }

        public void HideEffectIndicator()
        {
            effectIndicator.Hide();
            effectTurns.Hide();
        }

        public void UpdateTurnsLabel(int turns)
        {
            effectTurns.Text = $"{turns}";
        }

        private void OnFocusEntered()
        {
            focused = true;
            background.Modulate = focusColor;
            selectArrow.Show();
            selectArrow.Play("default");
        }

        private void OnFocusExited()
        {
            focused = false;
            background.Modulate = invisibleColor;
            selectArrow.Hide();
            selectArrow.Stop();
        }

        public async Task TweenDamage(Color color)
        {
            Tween tween = CreateTween();
            tween.TweenProperty(background, "modulate", new Color(color.R, color.G, color.B, 0.5f), 0.3);
            tween.TweenProperty(background, "modulate", new Color(color.R, color.G, color.B, 0), 0.3);
            SceneTreeTimer timer = GetTree().CreateTimer(2);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
            tween.Stop();
        }
    }
}