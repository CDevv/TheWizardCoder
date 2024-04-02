using Godot;
using Godot.Collections;

namespace DialogueManagerRuntime
{
  public partial class DialogueBalloon : CanvasLayer
  {
    const string NEXT_ACTION = "ui_accept";
    const string SKIP_ACTION = "ui_cancel";

    Global global;
    Control balloon;
    RichTextLabel characterLabel;
    RichTextLabel dialogueLabel;
    VBoxContainer responsesMenu;
    NinePatchRect responsesRect;
    MarginContainer textMarginContainer;
    AnimatedSprite2D portrait;
    AudioStreamPlayer2D audioPlayer;

    Resource resource;
    Array<Variant> temporaryGameStates = new Array<Variant>();
    bool isWaitingForInput = false;
    bool willHideBalloon = false;

    DialogueLine dialogueLine;
    DialogueLine DialogueLine
    {
      get => dialogueLine;
      set
      {
        isWaitingForInput = false;
        balloon.FocusMode = Control.FocusModeEnum.All;
        balloon.GrabFocus();

        if (value == null)
        {
          QueueFree();
          return;
        }

        dialogueLine = value;
        UpdateDialogue();
      }
    }


    public override void _Ready()
    {
      global = GetNode<Global>("/root/Global");
      balloon = GetNode<Control>("%Balloon");
      characterLabel = GetNode<RichTextLabel>("%CharacterLabel");
      dialogueLabel = GetNode<RichTextLabel>("%DialogueLabel");
      responsesMenu = GetNode<VBoxContainer>("%ResponsesMenu");
      responsesRect = GetNode<NinePatchRect>("%ResponsesRect");
      textMarginContainer = GetNode<MarginContainer>("%TextMarginContainer");
      portrait = GetNode<AnimatedSprite2D>("%Portrait");
      audioPlayer = GetNode<AudioStreamPlayer2D>("AudioPlayer");

      balloon.Hide();

      balloon.GuiInput += (@event) =>
      {
        if ((bool)dialogueLabel.Get("is_typing"))
        {
          bool mouseWasClicked = @event is InputEventMouseButton && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left && @event.IsPressed();
          bool skipButtonWasPressed = @event.IsActionPressed(SKIP_ACTION);
          if (mouseWasClicked || skipButtonWasPressed)
          {
            GetViewport().SetInputAsHandled();
            dialogueLabel.Call("skip_typing");
            return;
          }
        }

        if (!isWaitingForInput) return;
        if (dialogueLine.Responses.Count > 0) return;

        GetViewport().SetInputAsHandled();

        if (@event is InputEventMouseButton && @event.IsPressed() && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left)
        {
          Next(dialogueLine.NextId);
        }
        else if (@event.IsActionPressed(NEXT_ACTION) && GetViewport().GuiGetFocusOwner() == balloon)
        {
          Next(dialogueLine.NextId);
        }
      };

      responsesMenu.Connect("response_selected", Callable.From((DialogueResponse response) =>
      {
        Next(response.NextId);
      }));

      Engine.GetSingleton("DialogueManager").Connect("dialogue_ended", Callable.From((Resource dialogueResource) =>
      {
        balloon.Hide();

        global.CanWalk = true;
      }));

      DialogueManager.Mutated += OnMutated;
    }


    public override void _ExitTree()
    {
      DialogueManager.Mutated -= OnMutated;
    }


    public override void _UnhandledInput(InputEvent @event)
    {
      // Only the balloon is allowed to handle input while it's showing
      GetViewport().SetInputAsHandled();
    }


    public async void Start(Resource dialogueResource, string title, Array<Variant> extraGameStates = null)
    {
      temporaryGameStates = extraGameStates ?? new Array<Variant>();
      isWaitingForInput = false;
      resource = dialogueResource;

      DialogueLine = await DialogueManager.GetNextDialogueLine(resource, title, temporaryGameStates);
    }


    public async void Next(string nextId)
    {
      DialogueLine = await DialogueManager.GetNextDialogueLine(resource, nextId, temporaryGameStates);
    }


    #region Helpers


    private async void UpdateDialogue()
    {
      if (!IsNodeReady())
      {
        await ToSignal(this, SignalName.Ready);
      }

      // Set up the character name
      characterLabel.Visible = !string.IsNullOrEmpty(dialogueLine.Character);
      characterLabel.Text = Tr(dialogueLine.Character, "dialogue");

      // Set up the dialogue
      dialogueLabel.Hide();
      dialogueLabel.Set("dialogue_line", dialogueLine);

      // Set up the responses
      responsesRect.Hide();
      responsesMenu.Hide();
      responsesMenu.Set("responses", dialogueLine.Responses);

      //Set up portrait
      string portraitTag = DialogueLine.GetTagValue("portrait");
      if (!string.IsNullOrEmpty(portraitTag))
      {
        textMarginContainer.Set(MarginContainer.PropertyName.Position, new Vector2(58, 0));
        textMarginContainer.Set(MarginContainer.PropertyName.Size, new Vector2(485, 164));

        portrait.Show();
        //SpriteFrames spriteFrames = GD.Load<SpriteFrames>($"res://sprites/characters/portraits/{DialogueLine.Character.ToLower()}");
        portrait.Set(AnimatedSprite2D.PropertyName.Animation, DialogueLine.Character.ToLower());
        portrait.Set(AnimatedSprite2D.PropertyName.Frame, int.Parse(portraitTag));
      }
      else
      {
        textMarginContainer.Set(MarginContainer.PropertyName.Position, new Vector2(0, 0));
        textMarginContainer.Set(MarginContainer.PropertyName.Size, new Vector2(600, 164));

        portrait.Hide();   
      }

      //Set up audio
      var audioStream = GD.Load<AudioStream>($"res://sounds/voices/{dialogueLine.Character.ToLower()}_voice.wav");
      if (audioStream != null)
      {
        audioPlayer.Set("stream", audioStream);
      }
      else
      {
        var unknownVoice = GD.Load<AudioStream>($"res://sounds/voices/nolan_voice.wav");
        audioPlayer.Set("stream", unknownVoice);
      }  

      // Type out the text
      balloon.Show();
      willHideBalloon = false;
      dialogueLabel.Show();
      if (!string.IsNullOrEmpty(dialogueLine.Text))
      {
        dialogueLabel.Call("type_out");
        await ToSignal(dialogueLabel, "finished_typing");
      }

      // Wait for input
      if (dialogueLine.Responses.Count > 0)
      {
        balloon.FocusMode = Control.FocusModeEnum.None;
        responsesMenu.Show();
        responsesRect.Show();
      }
      else if (!string.IsNullOrEmpty(dialogueLine.Time))
      {
        float time = 0f;
        if (!float.TryParse(dialogueLine.Time, out time))
        {
          time = dialogueLine.Text.Length * 0.02f;
        }
        await ToSignal(GetTree().CreateTimer(time), "timeout");
        Next(dialogueLine.NextId);
      }
      else
      {
        isWaitingForInput = true;
        balloon.FocusMode = Control.FocusModeEnum.All;
        balloon.GrabFocus();
      }
    }


    #endregion


    #region signals


    private void OnMutated(Dictionary _mutation)
    {
      isWaitingForInput = false;
      willHideBalloon = true;
      GetTree().CreateTimer(0.1f).Timeout += () =>
      {
        if (willHideBalloon)
        {
          global.CanWalk = true;

          willHideBalloon = false;
          balloon.Hide();
        }
      };
    }


    #endregion

    public void OnSpokeCharacter(string letter, int letterIndex, float speed)
    {
      if (letter != " ")
      {
        audioPlayer.Play();
      }
    }
  }
}