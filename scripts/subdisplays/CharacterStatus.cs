using Godot;
using System;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;

namespace TheWizardCoder.Subdisplays
{
	public partial class CharacterStatus : Display
	{
		private AnimatedSprite2D portrait;
		private Label name;
		private Label health;
		private TextureProgressBar healthBar;
		private Label points;
		private Label attackPoints;
		private Label defensePoints;
		private Label agilityPoints;
		private Label levelLabel;
		private Label levelPointsLabel;
		private TextureProgressBar levelBar;
		private AnimatedSprite2D healthIcon;
		private AnimatedSprite2D manaIcon;

		public override void _Ready()
		{
			base._Ready();

			portrait = GetNode<AnimatedSprite2D>("%Portrait");
			name = GetNode<Label>("%NameLabel");
			health = GetNode<Label>("%HealthLabel");
			healthBar = GetNode<TextureProgressBar>("%HealthBar");
			points = GetNode<Label>("%PointsLabel");
			attackPoints = GetNode<Label>("%AttackPointsLabel");
			defensePoints = GetNode<Label>("%DefensePointsLabel");
			agilityPoints = GetNode<Label>("%AgilityPointsLabel");
			levelLabel = GetNode<Label>("%LevelLabel");
			levelPointsLabel = GetNode<Label>("%LevelPointsLabel");
			levelBar = GetNode<TextureProgressBar>("%LevelBar");
			healthIcon = GetNode<AnimatedSprite2D>("%HealthIcon");
			manaIcon = GetNode<AnimatedSprite2D>("%ManaIcon");
		}

		public override void ShowDisplay()
		{
			throw new NotImplementedException();
		}

		public void ShowDisplay(Character character)
		{
			portrait.Animation = character.Name;
			name.Text = character.Name;

			health.Text = $"Health: {character.Health}/{character.MaxHealth}";
			healthBar.MaxValue = character.MaxHealth;
			healthBar.Value = character.Health;

			points.Text = $"MP: {character.Points}/{character.MaxPoints}";
			attackPoints.Text = $"Attack: {character.AttackPoints}";
			defensePoints.Text = $"Defense: {character.DefensePoints}";
			agilityPoints.Text = $"Agility: {character.AgilityPoints}";

			levelLabel.Text = $"Level: {character.Level}";
			levelPointsLabel.Text = $"XP: {character.LevelPoints}/{character.GetMaxLevelPoints()}";

			levelBar.MaxValue = character.GetMaxLevelPoints();
			levelBar.Value = character.LevelPoints;

			Show();
		}

		private void OnHealthLabelResized()
		{
            Vector2 healthIconPosition = new Vector2(health.Position.X - (8), health.Position.Y + (health.Size.Y/3));
            healthIcon.Position = healthIconPosition;
        }

		private void OnManaLabelResized()
		{
            if (points != null)
            {
                Vector2 manaIconPosition = new Vector2(points.Position.X - 8, points.Position.Y + (points.Size.Y / 3));
                manaIcon.Position = manaIconPosition;
            }
		}
	}
}