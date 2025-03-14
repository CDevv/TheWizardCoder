using Godot;
using System.Collections.Generic;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.UI;

namespace TheWizardCoder.Subdisplays
{
    public partial class PartyMembersList : Display
    {
        private const float CardSizeY = 40;
        private const float Padding = 5;

        [Signal]
        public delegate void CharacterPressedEventHandler(bool isProtagonist, int allyIndex);

        [Export]
        public PackedScene PartyMemberPackedScene;

        [Export]
        public ControlsDisplay Controls { get; set; }

        private CharacterPartyMember partyMember;
        private Marker2D basePos;

        private List<CharacterPartyMember> partyMembers = new();
        private List<Character> characterData = new();

        public override void _Ready()
        {
            base._Ready();
            partyMember = GetNode<CharacterPartyMember>("Nolan");
            basePos = GetNode<Marker2D>("BasePos");

            partyMembers.Add(partyMember);
            characterData.Add(global.PlayerData.Stats);

            UpdateDisplay();
        }

        public override void ShowDisplay()
        {
            Show();
            partyMember.GrabFocus();

            Controls.ChangeXLabel("Go Back");
            Controls.ChangeZLabel("Select");
        }

        public override void UpdateDisplay()
        {
            partyMember.ApplyData(global.PlayerData.Stats);

            for (int i = 0; i < partyMembers.Count; i++)
            {
                partyMembers[i].ApplyData(characterData[i]);
            }

            foreach (var ally in global.PlayerData.Allies)
            {
                if (!characterData.Contains(ally))
                {
                    AddCard(ally);
                }
            }
        }

        public void AddCard(Character character)
        {
            int currentIndex = partyMembers.Count;

            characterData.Add(character);

            CharacterPartyMember partyMember = PartyMemberPackedScene.Instantiate<CharacterPartyMember>();
            partyMember.Position = new Vector2(basePos.Position.X, basePos.Position.Y + (2 * partyMembers.Count * (CardSizeY + Padding)));
            partyMembers.Add(partyMember);

            AddChild(partyMember);
            partyMember.ApplyData(character);

            partyMembers[0].FocusNeighborBottom = partyMember.GetPath();
            partyMember.FocusNeighborTop = partyMembers[^2].GetPath();

            partyMember.Pressed += () => OnCharacterPressed(false, currentIndex);
        }

        private void OnCharacterPressed(bool isProtagonist, int allyIndex)
        {
            EmitSignal(SignalName.CharacterPressed, isProtagonist, allyIndex);
        }
    }
}