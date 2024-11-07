using Godot;
using System;
using System.Linq;
using System.Text;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Interactables;

public partial class Forest15 : BaseRoom
{
	private const int ButtonsCount = 5;

	private bool[] bools = { false, false, false, false, false };
	private SwitchBlock[] buttons = new SwitchBlock[ButtonsCount];
	private CodeMessagePoint codeMessage;

	private TileMap tileMap;

    public override void OnReady()
    {
        base.OnReady();

		tileMap = GetNode<TileMap>("TileMap");

		codeMessage = GetNode<CodeMessagePoint>("CodeMessagePoint");
		for (int i = 0; i < ButtonsCount; i++)
		{
			int currentIndex = i;
			SwitchBlock interactable = GetNode<SwitchBlock>($"Button{i+1}");
			buttons[i] = interactable;
			buttons[i].Pushed += () => OnButtonPushed(currentIndex);
		}
    }

    private void OnButtonPushed(int i)
	{
		bools[i] = !bools[i];
		codeMessage.CodeText = BuildCodeString();
	}

	private string BuildCodeString()
	{
		StringBuilder builder = new();
		builder.AppendLine("int c = 5;");
		builder.AppendLine("bool[] arr =");
		builder.Append("{ ");
		builder.Append(string.Join(", ", bools.Select(x => x.ToString()).Select(x => x.ToLower())));
		builder.AppendLine(" };");
		builder.AppendLine("for (int i = 0; i < c; i++)");
		builder.AppendLine("	if (IsEven(i) == arr[i])");
		builder.AppendLine("		RemoveTrees();");

		return builder.ToString();
	}

	private void CheckCode()
	{
		GD.Print("execute");
		int count = 0;
		for (int i = 0; i < ButtonsCount; i++)
		{
			if ((i % 2 == 0) == buttons[i].Value)
			{
				count++;
			}
		}

		if (count == ButtonsCount)
		{
			GD.Print("Passed!");
			ClearTrees();
		}
	}

	private void ClearTrees()
	{
		for (int i = 8; i <= 17; i++)
		{
			tileMap.SetCell(0, new Vector2I(56, i));
			tileMap.SetCell(0, new Vector2I(57, i));
		}
	}
}
