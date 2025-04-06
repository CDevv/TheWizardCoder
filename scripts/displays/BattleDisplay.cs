using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using TheWizardCoder.Subdisplays;
using TheWizardCoder.UI;

namespace TheWizardCoder.Displays
{
    public partial class BattleDisplay : Display
    {
        [Signal]
        public delegate void BattleEndedEventHandler();
        [Signal]
        public delegate void TurnFinishedEventHandler();

        [Export]
        public Resource TutorialDialogueResource { get; set; }
        [Export]
        public AlliesSideDisplay Allies { get; private set; }
        [Export]
        public EnemiesSideDisplay Enemies { get; private set; }

        public bool IsBattleEnded { get; private set; } = false;
        public bool IsTutorial { get; set; } = false;

        private BattleOptions battleOptions;
        private Button invisButton;
        private TextureRect backgroundRect;
        private bool lockBattleOptions = false;

        public BattleOptions BattleOptions => battleOptions;

        public override void _Ready()
        {
            base._Ready();
            battleOptions = GetNode<BattleOptions>("BattleOptions");
            invisButton = GetNode<Button>("InvisButton");
            backgroundRect = GetNode<TextureRect>("Background");
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (!Visible)
            {
                return;
            }

            if (Input.IsActionJustPressed("ui_cancel"))
            {
                if (!lockBattleOptions)
                {
                    battleOptions.ShowOptions();
                }
            }
        }

        public override void ShowDisplay()
        {
            ShowDisplay(new() { "Glitch" }, backgroundRect.Texture);
        }

        public void ShowDisplay(Array<string> enemies)
        {
            ShowDisplay(enemies, backgroundRect.Texture);
        }

        public async void ShowDisplay(Array<string> enemies, Texture2D background)
        {
            lockBattleOptions = false;
            IsBattleEnded = false;
            backgroundRect.Texture = background;

            //Add allies
            Allies.AddCharacter(global.PlayerData.Stats);
            if (global.PlayerData.Allies.Count > 0)
            {
                foreach (Character ally in global.PlayerData.Allies)
                {
                    Allies.AddCharacter(ally);
                }
            }

            //Add enemies
            foreach (string enemyName in enemies)
            {
                Character newEnemy = global.Characters[enemyName];
                Enemies.AddCharacter(newEnemy);
            }

            global.CurrentRoom.TransitionRect.PlayAnimation();
            await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
            Show();
            battleOptions.ShowDisplay();
            Allies.Show();
            Enemies.Show();
            global.CurrentRoom.TransitionRect.PlayAnimationBackwards();

            if (IsTutorial)
            {
                await global.CurrentRoom.ShowDialogue(TutorialDialogueResource, "tutorial_battle_0");
                global.GameDisplayEnabled = false;
            }

            Allies.StartTurn();
        }

        private void Clear()
        {
            for (int i = 0; i < Allies.Characters.Count; i++)
            {
                Character character = Allies.Characters[i];
                if (character.Health <= 0)
                {
                    character.Health = 5;
                }
            }

            Allies.Clear();
            Enemies.Clear();
        }

        public async Task Routine()
        {
            lockBattleOptions = true;

            invisButton.GrabFocus();

            List<CharacterBattleState> participants = new();
            participants.AddRange(Allies.Characters.BattleStates);
            participants.AddRange(Enemies.Characters.BattleStates);
            participants = participants.OrderByDescending((p) => p.Character.AgilityPoints).ToList();

            foreach (CharacterBattleState participant in participants)
            {
                switch (participant.Character.Type)
                {
                    case CharacterType.Ally:
                        await Allies.OnTurn(participant.InternalIndex);
                        break;
                    case CharacterType.Enemy:
                        await Enemies.OnTurn(participant.InternalIndex);
                        break;
                }

                if (Enemies.Characters.GetTotalHealth() <= 0)
                {
                    HideDisplay();
                    return;
                }
                else if (Allies.Characters.GetTotalHealth() <= 0)
                {
                    IsBattleEnded = true;
                    global.CurrentRoom.TransitionRect.PlayAnimation();
                    await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);

                    global.CurrentRoom.GameOverDisplay.ShowDisplay();
                    Clear();
                    Allies.Hide();
                    Enemies.Hide();
                    battleOptions.Hide();
                }
                else
                {
                    Allies.StartTurn();
                }
            }

            if (Visible)
            {
                EmitSignal(SignalName.TurnFinished);
            }

            lockBattleOptions = false;
        }

        public override async void HideDisplay()
        {
            IsBattleEnded = true;
            IsTutorial = false;
            lockBattleOptions = true;

            int wonGold = Allies.Characters.Sum(x => x.AttackPoints * x.Level);

            battleOptions.ShowInfoLabel($"You won! {wonGold} Gold and {Allies.CollectedExperience[0]} EXP obtained.");

            await StartTransition();

            Hide();
            Allies.Hide();
            Enemies.Hide();
            battleOptions.HideDisplay();

            global.CanWalk = true;
            global.GameDisplayEnabled = true;
            EndTransition();
            EmitSignal(SignalName.BattleEnded);

            Allies.AwardExperience();
            global.PlayerData.Gold += wonGold;
            Clear();
        }

        private async Task StartTransition()
        {
            SceneTreeTimer timer = GetTree().CreateTimer(2);
            await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
            global.CurrentRoom.TransitionRect.PlayAnimation();
            await ToSignal(global.CurrentRoom.TransitionRect, TransitionRect.SignalName.AnimationFinished);
        }

        private void EndTransition()
        {
            global.CurrentRoom.TransitionRect.PlayAnimationBackwards();
        }
    }
}