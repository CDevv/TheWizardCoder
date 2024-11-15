using Godot;
using System;
using TheWizardCoder.Abstractions;

public partial class LevelUpDisplay : Display
{
	private LevelUpOption healthOption;
	private LevelUpOption magicOption;
	private AnimationPlayer animationPlayer;
	
	public override void _Ready()
	{
		base._Ready();
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		healthOption = GetNode<LevelUpOption>("HealthOption");
		magicOption = GetNode<LevelUpOption>("MagicOption");
	}

    public override void ShowDisplay()
    {
		global.CanWalk = false;
		global.GameDisplayEnabled = false;
		healthOption.SetText($"Increase Max HP\n{global.PlayerData.Stats.MaxHealth} -> {global.PlayerData.Stats.MaxHealth + 5}");
		magicOption.SetText($"Increase Max MP\n{global.PlayerData.Stats.MaxPoints} -> {global.PlayerData.Stats.MaxPoints + 5}");
		healthOption.GrabFocus();
        Show();
		animationPlayer.Play("show");
    }

	public override async void HideDisplay()
	{
		animationPlayer.PlayBackwards("show");
		await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
		Hide();

		global.CanWalk = true;
		global.GameDisplayEnabled = true;
	}

	private void IncreaseMaxHealth()
	{
		global.PlayerData.Stats.SetMaxHealth(global.PlayerData.Stats.MaxHealth + 5);
		HideDisplay();
	}

	private void IncreaseMaxPoints()
	{
		global.PlayerData.Stats.SetMaxPoints(global.PlayerData.Stats.MaxPoints + 5);
		HideDisplay();
	}
}
