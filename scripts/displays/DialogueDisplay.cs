using Godot;
using Godot.Collections;
using System;
using DialogueManagerRuntime;

public partial class DialogueDisplay : Display
{
	[Signal]
	public delegate void DialogueEndedEventHandler();

	[Export]
	private PackedScene ResponseTemplate { get; set; }

	private RichTextLabel label;
	private AnimatedSprite2D portrait;
	private MarginContainer responsesRect;
	private VBoxContainer responsesMenu;
	private AudioStreamPlayer audioPlayer;
	private DialogueLine dialogueLine;
	private Resource dialogueResource;
	private Marker2D responsesRectMarker;
	private bool isTyping = false;
	private double waitingTime = 0.02;
	private string text = "";
	private int index = -1;
	private bool hasResponses = false;
	private float maxSize = 0;

	public override void _Ready()
	{
		base._Ready();
		label = GetNode<RichTextLabel>("%DialogueText");
		portrait = GetNode<AnimatedSprite2D>("%Portrait");
		responsesRect = GetNode<MarginContainer>("%ResponsesRect");
		responsesMenu = GetNode<VBoxContainer>("%Responses");
		audioPlayer = GetNode<AudioStreamPlayer>("%AudioPlayer");
		responsesRectMarker = GetNode<Marker2D>("ResponsesContainerMarker");
	}

	public override async void _Process(double delta)
	{
		if (!Visible) return;

		if (isTyping)
		{
			if (label.VisibleRatio == 1)
			{
				isTyping = false;
				if (dialogueLine.Responses.Count > 0)
				{
					responsesRect.Show();
					UpdateResponses(dialogueLine.Responses);				
					//responsesRect.Size = new Vector2(responsesRect.Size.X, 68);
					//responsesRect.Position = responsesRectMarker.Position - new Vector2(responsesRect.Size.X * 2, 0);
					responsesRect.Position = responsesRectMarker.Position - new Vector2((maxSize + (8 * 2)) * 2, 0);
				}
				return;
			}

			if (waitingTime > 0)
			{
				waitingTime = waitingTime - delta;
			}
			else
			{
				TypeCharacter();
				waitingTime = 0.02;
			}
		}

		if (Input.IsActionJustPressed("ui_accept"))
		{
			if (hasResponses)
			{
				return;
			}

			if (!isTyping)
			{
				label.VisibleCharacters = 0;
				index = 0;
				string nextId = dialogueLine.NextId;

				if (nextId == "END!" || nextId == "END")
				{
					OnDialogueEnded();
				}
				else
				{
					dialogueLine = await DialogueManager.GetNextDialogueLine(dialogueResource, nextId);
					UpdateDisplay(dialogueLine);
				}
			}
		}

		if (Input.IsActionJustPressed("ui_cancel"))
		{
			label.VisibleCharacters = -1;
			index = dialogueLine.Text.Length - 1;
		}
	}

    public override void ShowDisplay()
    {
        Show();
    }

    public async void ShowDisplay(Resource dialogueResource, string title)
	{
		global.CanWalk = false;
		global.GameDisplayEnabled = false;
		this.dialogueResource = dialogueResource;
		dialogueLine = await DialogueManager.GetNextDialogueLine(dialogueResource, title, new Array<Variant>());
		UpdateDisplay(dialogueLine);
		Show();
	}

    public override void UpdateDisplay()
    {
        UpdateDisplay(dialogueLine);
    }

    private void UpdateDisplay(DialogueLine line)
	{
		if (line == null)
		{
			OnDialogueEnded();
			return;
		}

		responsesRect.Hide();
		label.VisibleCharacters = 0;
		label.Text = line.Text;
		index = -1;

		//Set up portrait
		string portraitTag = line.GetTagValue("portrait");
		if (!string.IsNullOrEmpty(portraitTag))
		{
			label.Set(MarginContainer.PropertyName.Position, new Vector2(72, 8));
			label.Set(MarginContainer.PropertyName.Size, new Vector2(416, 112));

			portrait.Show();
			portrait.Set(AnimatedSprite2D.PropertyName.Animation, line.Character.ToLower());
			portrait.Set(AnimatedSprite2D.PropertyName.Frame, int.Parse(portraitTag));
		}
		else
		{
			label.Set(MarginContainer.PropertyName.Position, new Vector2(8, 8));
			label.Set(MarginContainer.PropertyName.Size, new Vector2(544, 112));

			portrait.Hide();   
		}

		//Set up audio
		var audioStream = GD.Load<AudioStream>($"res://sounds/voices/base_voice.wav");
		audioPlayer.Set("stream", audioStream);

		hasResponses = line.Responses.Count > 0;
		isTyping = true;
	}

	private void TypeCharacter()
	{
		label.VisibleCharacters++;
		index++;
		if (dialogueLine.Text[index] != ' ')
		{
			audioPlayer.Play();
		}
	}

	private void UpdateResponses(Array<DialogueResponse> responses)
	{
		ClearResponses();

		maxSize = 0;
		Array<Button> buttons = new Array<Button>();
		for (int i = 0; i < responses.Count; i++)
		{
			DialogueResponse response = responses[i];
			Button button = ResponseTemplate.Instantiate<Button>();
			button.Text = response.Text;
			button.Set("theme_override_font_sizes/font_size", 16);
			button.Pressed += () => OnResponseSelected(response.NextId);
			responsesMenu.AddChild(button);
			buttons.Add(button);

			if (i == 0)
			{
				maxSize = button.Size.X;
				button.GrabFocus();
			}
			else if (button.Size.X > maxSize)
			{
				maxSize = button.Size.X;
			}
		}

		buttons[0].FocusNeighborTop = buttons[buttons.Count - 1].GetPath();
		buttons[0].FocusNeighborBottom = buttons[1].GetPath();
		for (int i = 1; i < buttons.Count - 1; i++)
		{
			buttons[i].FocusNeighborTop = buttons[i-1].GetPath();
			buttons[i].FocusNeighborBottom = buttons[i+1].GetPath();
		}
		buttons[buttons.Count - 1].FocusNeighborTop = buttons[buttons.Count - 2].GetPath();
		buttons[buttons.Count - 1].FocusNeighborBottom = buttons[0].GetPath();

		GD.Print(responsesRect.Size);
	}

	private void ClearResponses()
	{
		Array<Node> oldResponses = responsesMenu.GetChildren();
		foreach (Node item in oldResponses)
		{
			item.QueueFree();
		}
	}

	private async void OnResponseSelected(string nextId)
	{
		dialogueLine = await DialogueManager.GetNextDialogueLine(dialogueResource, nextId);
		UpdateDisplay(dialogueLine);
	}

	private void OnDialogueEnded()
	{
		Hide();
		global.GameDisplayEnabled = true;
		global.CanWalk = true;
		EmitSignal(SignalName.DialogueEnded);
	}
}
