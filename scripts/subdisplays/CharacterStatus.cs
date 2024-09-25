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
		}

		public override void ShowDisplay()
		{
			throw new NotImplementedException();
		}

		public void ShowDisplay(CharacterData character)
		{
			portrait.Animation = character.Name;
			name.Text = character.Name;

			health.Text = $"Health: {character.Health}";
			healthBar.MaxValue = character.MaxHealth;
			healthBar.Value = character.Health;

			points.Text = $"MP: {character.Points}";
			attackPoints.Text = $"Attack: {character.AttackPoints}";
			defensePoints.Text = $"Defense: {character.DefensePoints}";
			agilityPoints.Text = $"Agility: {character.AgilityPoints}";

			Show();
		}
	}
}