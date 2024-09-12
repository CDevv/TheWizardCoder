using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

public partial class Global : Node
{
	public BaseRoom CurrentRoom { get; set; }
	public string LocationMarkerName { get; set; }
	public Direction PlayerDirection { get; set; }
	public bool HasLoadedGame { get; set; } = false;
	public bool CanWalk = true;
	public bool GameDisplayEnabled { get; set; } = true;
	public bool PlayerIsOnStairs { get; set; } = false;
	public bool StairsInverted { get; set; } = false;
	public bool StairsGoUp { get; set; } = true;
	public bool IsInCutscene { get; set; } = false;
	public SaveFileData PlayerData { get; set; } = new();
	public SettingsConfig Settings { get; set; } = new();
	public System.Collections.Generic.Dictionary<string, Item> ItemDescriptions { get; private set; } = new();
	public System.Collections.Generic.Dictionary<string, MagicSpell> MagicSpells { get; private set; } = new();
	public System.Collections.Generic.Dictionary<string, CharacterData> Characters { get; private set; } = new();
	public int[] ReverseDirections { get; } = { 1, 0, 3, 2 };

    public override void _Ready()
    {
		try
		{
			Settings.LoadSettings();
			Settings.ApplySettings();
			LoadItemDescriptions();
			LoadMagicSpells();
			LoadCharactersData();
		}
		catch (System.Exception e)
		{
			GD.PrintErr(e.ToString());
		}
    }

	public void AddToInventory(string item)
	{
		PlayerData.AddToInventory(item);
	}

	public void RemoveFromInventory(string item)
	{
		PlayerData.RemoveFromInventory(item);
	}

	public bool HasItemInInventory(string item)
	{
		return PlayerData.HasItemInInventory(item);
	}

	public void AddMagicSpell(string name)
	{
		PlayerData.AddMagicSpell(name);
	}

	private void LoadItemDescriptions()
	{
		Variant jsonData = GetJsonData("res://info/item_descriptions.json");
		Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

		foreach (var pair in parsedData)
		{
			Item item = new Item();
			item.ApplyDictionary(pair.Value);
			item.Name = pair.Key;
			ItemDescriptions.Add(item.Name, item);
		}
	}

	private void LoadMagicSpells()
	{
		Variant jsonData = GetJsonData("res://info/magic_descriptions.json");
		Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

		foreach (var pair in parsedData)
		{
			MagicSpell magicSpell = new MagicSpell();
			magicSpell.ApplyDictionary(pair.Value);
			MagicSpells.Add(pair.Key, magicSpell);
		}
	}

	public void LoadCharactersData()
	{
		Variant jsonData = GetJsonData("res://info/enemies.json");
		Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

		foreach (var pair in parsedData)
		{
			Dictionary<string, Variant> dict = pair.Value;
			dict["Name"] = pair.Key;
			CharacterData character = new();
			character.ApplyDictionary(dict);
			Characters.Add(pair.Key, character);

			ResourceLoader.LoadThreadedRequest($"res://assets/battle/enemies/{pair.Key}.png");
		}
	}

	public void CreateSaveFile(string saveName)
	{
		SaveFileData data = new SaveFileData();
		data.FileName = saveName;
		data.SaveName = saveName;
		
		FileAccess file = FileAccess.Open($"user://{saveName}.wand", FileAccess.ModeFlags.Write);
		file.StoreVar(JsonConvert.SerializeObject(data));
		file.Close();

		FileAccess hashFile = FileAccess.Open($"user://{saveName}.ini", FileAccess.ModeFlags.Write);
		byte[] hash = CalculateHash(saveName);
		hashFile.StoreVar(hash);
		hashFile.Close();
	}

	public void UpdateSaveFile(string saveName)
	{
		PlayerData.TimeSpent = PlayerData.TimeSpent.Add(DateTime.Now - PlayerData.LastSaved);
		PlayerData.LastSaved = DateTime.Now;
		PlayerData.LocationVector = CurrentRoom.Player.Position;
		PlayerData.SceneDefaultMarker = CurrentRoom.DefaultMarkerName;
		SaveFileData data = PlayerData;		

		FileAccess file = FileAccess.Open($"user://{saveName}.wand", FileAccess.ModeFlags.Write);
		file.StoreVar(JsonConvert.SerializeObject(data));
		file.Close();

		FileAccess hashFile = FileAccess.Open($"user://{saveName}.ini", FileAccess.ModeFlags.Write);
		byte[] hash = CalculateHash(saveName);
		hashFile.StoreVar(hash);
		hashFile.Close();

		PlayerData.LocationVector = Vector2.Zero;
	}

	public void LoadSaveFile(string saveName)
	{
		SaveFileData data = ReadSaveFileData(saveName);
		PlayerData = data;

		if (data.IsSaveEmpty)
		{
			PlayerData.SaveName = saveName;
			CreateSaveFile(saveName);
		}
	}

	public void DeleteSaveFile(string saveName)
	{
		DirAccess dir = DirAccess.Open("user://");
		dir.Remove($"{saveName}.ini");
		dir.Remove($"{saveName}.wand");
	}
	public SaveFileData ReadSaveFileData(string saveName)
	{
		SaveFileData data = new SaveFileData();

		if (!FileAccess.FileExists($"user://{saveName}.wand"))
        {
            GD.PushWarning($"File {saveName}.wand does not exist");
			data.IsSaveEmpty = true;
            return data;
        }

		//Get expected hash
		using var hashFile = FileAccess.Open($"user://{saveName}.ini", FileAccess.ModeFlags.Read);
        byte[] expectedHash = (byte[])hashFile.GetVar();
        hashFile.Close();

		//Actual hash
		byte[] actualHash = CalculateHash(saveName);

		if (CompareHashes(actualHash, expectedHash))
		{
			using var readSave = FileAccess.Open($"user://{saveName}.wand", FileAccess.ModeFlags.Read);
			string savedData = (string)readSave.GetVar();
			data = JsonConvert.DeserializeObject<SaveFileData>(savedData);
			data.IsSaveEmpty = false;
		}
		else
		{
			GD.PushWarning($"{saveName}.ini: The hashes don't match");
			data.IsSaveEmpty = true;
            return data;
		}

		return data;
	}

	private byte[] CalculateHash(string saveName)
	{
		using FileAccess file = FileAccess.Open($"user://{saveName}.wand", FileAccess.ModeFlags.Read);
        byte[] buffer = file.GetBuffer((long)file.GetLength());
        file.Close();

		HashingContext hashing = new();
        hashing.Start(HashingContext.HashType.Sha256);
        hashing.Update(buffer);
        byte[] data = hashing.Finish();

		return data;
	}

	public void ChangeRoom(string room)
	{
		ChangeRoom(room, "DefaultPlayerPos", Direction.Down);
	}

	public void ChangeRoom(string room, string playerLocation, Direction direction)
	{
		var result = GetTree().ChangeSceneToFile($"res://scenes/rooms/{room}.tscn");
		if (result == Error.Ok)
		{
			LocationMarkerName = playerLocation;
			PlayerDirection = direction;
		}
	}

	public void GoToMainMenu()
	{
		GetTree().ChangeSceneToFile("res://scenes/rooms/main_menu.tscn");
	}

	public Vector2 GetDirectionVector(Direction direction)
	{
		Vector2 resultVector = Vector2.Down;
		switch (direction)
		{
			case Direction.Up:
				resultVector = Vector2.Up;
				break;
			case Direction.Down:
				resultVector = Vector2.Down;
				break;
			case Direction.Left:
				resultVector = Vector2.Left;
				break;
			case Direction.Right:
				resultVector = Vector2.Right;
				break;
		}

		return resultVector;
	}

	public Direction GetDirectionFromVector(Vector2 vector)
	{
		float radianAngle = vector.Angle();
		float angle = Mathf.RadToDeg(radianAngle);
		Direction direction = Direction.Down;
		
		switch (angle)
		{
			case 0:
				direction = Direction.Right;
				break;
			case 90:
				direction = Direction.Down;
				break;
			case 180:
				direction = Direction.Left;
				break;
			case -90:
				direction = Direction.Up;
				break;
			default:
				direction = Direction.Down;
				break;
		}
	
		return direction;
	}

	public Vector2 GetCameraBaseVector()
	{
		Vector2 resolutionVector = new Vector2(Settings.WindowWidth, Settings.WindowHeight);
		Vector2 baseVector = CurrentRoom.Camera.GlobalPosition - (resolutionVector / 2);
		return baseVector;
	}

	private Variant GetJsonData(string fileName)
	{
		if (!FileAccess.FileExists(fileName))
		{
			GD.PrintErr($"{fileName} does not exist");
			return new Json();
		}

		using var data = FileAccess.Open(fileName, FileAccess.ModeFlags.Read);
		string jsonString = data.GetAsText();

		Json json = new Json();
		Error jsonError = json.Parse(jsonString);

		if (jsonError != Error.Ok)
		{
			GD.PrintErr($"Json parse error: {jsonError}");
		}

		return json.Data;
	}

	private bool CompareHashes(byte[] h1, byte[] h2)
    {
        if (h1 == null && h2 != null)
        {
            return false;
        }
        else if (h2 == null && h1 != null)
        {
            return false;
        }

        if (h1.Length != h2.Length)
        {
            return false;
        }

        for (int i = 0; i < h2.Length; i++)
        {
            var b1 = h1[i];
            var b2 = h2[i];
            if (b1 != b2)
            {
                return false;
            }
        }

        return true;
    }

	public Variant GetPlayerData(string key)
	{
		return PlayerData.Get(key);
	}

	public void SetPlayerData(string key, Variant value)
	{
		PlayerData.Set(key, value);
	}

	public void CallRoomMethod(string methodName)
	{
		CurrentRoom.CallDeferred(methodName);
	}
}
