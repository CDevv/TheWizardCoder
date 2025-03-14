using Godot;

public partial class IntField : Button
{
    public int Value { get; private set; } = 1;

    private AnimatedSprite2D leftArrow;
    private AnimatedSprite2D rightArrow;

    private bool isFocused = false;

    public override void _Ready()
    {
        leftArrow = GetNode<AnimatedSprite2D>("Left");
        rightArrow = GetNode<AnimatedSprite2D>("Right");

        Text = $"{Value}";
    }

    public override void _Process(double delta)
    {
        if (isFocused)
        {
            if (Input.IsActionJustPressed("ui_left"))
            {
                if (Value > 1)
                {
                    Value--;
                    Text = $"{Value}";
                }
            }
            if (Input.IsActionJustPressed("ui_right"))
            {
                if (Value < 99)
                {
                    Value++;
                    Text = $"{Value}";
                }
            }
        }
    }

    public void Reset()
    {
        Value = 1;
        Text = $"{Value}";
    }

    private void OnFocusEntered()
    {
        isFocused = true;
        leftArrow.Play();
        rightArrow.Play();
    }

    private void OnFocusExited()
    {
        isFocused = false;
        leftArrow.Stop();
        rightArrow.Stop();
    }
}
