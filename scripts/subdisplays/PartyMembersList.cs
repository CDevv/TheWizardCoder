using Godot;
using System;

public partial class PartyMembersList : Display
{
	[Signal]
	public delegate void CharacterPressedEventHandler();

	private CharacterPartyMember partyMember;
	public override void _Ready()
	{
		base._Ready();
		partyMember = GetNode<CharacterPartyMember>("CharacterPartyMember");
	}

	public override void ShowDisplay()
	{
		Show();
		partyMember.GrabFocus();
	}

	public override void UpdateDisplay()
	{
		partyMember.ApplyData(global.PlayerData.Stats);
	}

	private void OnCharacterPressed()
	{
		EmitSignal(SignalName.CharacterPressed);
	}
}
