using Godot;
using Godot.Collections;

public class BattleInfo
{
    public string Name { get; set; }
    public Array<string> AttackNames { get; set; }
    public Resource DialogueResource { get; private set; }
    public PackedScene Attacks { get; private set; }

    public void Setup(string name)
    {
        Name = name;

        string jsonFileName = $"res://info/enemy_battles.json";
		if (!FileAccess.FileExists(jsonFileName))
		{
			GD.PrintErr($"{jsonFileName} does not exist");
			return;
		}

		using var data = FileAccess.Open(jsonFileName, FileAccess.ModeFlags.Read);
		string jsonString = data.GetAsText();

		Json json = new Json();
		Error jsonError = json.Parse(jsonString);

		if (jsonError != Error.Ok)
		{
			GD.PrintErr($"Json parse error: {jsonError}");
			return;
		}

		Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)json.Data;

		Dictionary<string, Variant> item = parsedData[name];

		ApplyDictionary(item);

        DialogueResource = GD.Load<Resource>($"res://dialogue/battles/{name}.dialogue");
        Attacks = GD.Load<PackedScene>($"res://scenes/enemy_attacks/{name}.tscn");
    }

    private void ApplyDictionary(Dictionary<string, Variant> dict)
    {
        AttackNames = (Array<string>)dict["Attacks"];
    }
}