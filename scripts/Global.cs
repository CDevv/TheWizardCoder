using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

public partial class Global : Node
{
	public BaseRoom CurrentRoom { get; set; }
	public string LocationMarkerName { get; set; }
	public Direction PlayerDirection { get; set; }
	public bool CanWalk = true;
	public string SaveName { get; set; }
	public DateTime LastSaved { get; set; }
	public DateTime StartedOn { get; set; }
	public TimeSpan TimeSpent { get; set; }
	public string Location { get; set; }
	public int Health { get; set; } = 100;

	public SaveFileData PlayerData { get; set; } = new();
	public SettingsConfig Settings { get; set; } = new();

    public override void _Ready()
    {
		Settings.LoadSettings();
		Settings.ApplySettings();
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

	public void SaveGame(string saveName)
	{
		GD.Print(LastSaved.ToString());
		GD.Print(DateTime.Now.ToString());
		PlayerData.TimeSpent = TimeSpent.Add(DateTime.Now - LastSaved);
		PlayerData.LastSaved = DateTime.Now;

		Dictionary<string, Variant> saveData = PlayerData.GenerateDictionary();

		//Save to file
		var writeSaveFile = FileAccess.Open($"user://{saveName}.wand", FileAccess.ModeFlags.Write);
		writeSaveFile.StoreVar(saveData);
		writeSaveFile.Close();

		//Compute hash
		using var readSaveFile = FileAccess.Open($"user://{saveName}.wand", FileAccess.ModeFlags.Read);
        var bytes = readSaveFile.GetBuffer((long)readSaveFile.GetLength());
        readSaveFile.Close();

        HashingContext hashing = new();
        hashing.Start(HashingContext.HashType.Sha256);
        hashing.Update(bytes);
        byte[] data = hashing.Finish();

		//Save hash
        using var hashFile = FileAccess.Open($"user://{saveName}.ini", FileAccess.ModeFlags.Write);
        hashFile.StoreVar(data);
        hashFile.Close();
	}

	public void LoadGame(string saveName)
	{
		LastSaved = DateTime.Now;
		if (!FileAccess.FileExists($"user://{saveName}.wand"))
        {
            GD.PushWarning($"File {saveName}.wand does not exist");
			PlayerData.SaveName = saveName;
			PlayerData.StartedOn = DateTime.Now;
			PlayerData.TimeSpent = TimeSpan.Zero;
            SaveGame(saveName);
            return;
        }

		var data = ReadSaveFileData(saveName);
		PlayerData = data;
	}

	public SaveFileData ReadSaveFileData(string saveName)
	{
		SaveFileData data = new();

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

		//Read save file
		using var saveFile = FileAccess.Open($"user://{saveName}.wand", FileAccess.ModeFlags.Read);
        byte[] buffer = saveFile.GetBuffer((long)saveFile.GetLength());
        saveFile.Close();

		//Compute actual hash
		HashingContext hashing = new();
        hashing.Start(HashingContext.HashType.Sha256);
        hashing.Update(buffer);
        byte[] actualHash = hashing.Finish();

		if (CompareHashes(actualHash, expectedHash))
		{
			using var readSave = FileAccess.Open($"user://{saveName}.wand", FileAccess.ModeFlags.Read);
			var savedData = (Dictionary<string, Variant>)readSave.GetVar();
			data.ApplyDictionary(savedData);
		}
		else
		{
			GD.PushWarning($"{saveName}.ini: The hashes don't match");
			data.IsSaveEmpty = true;
            return data;
		}

		return data;
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

	public void ChangeRoom(PackedScene room)
	{
		GetTree().ChangeSceneToPacked(room);
		LocationMarkerName = string.Empty;
		PlayerDirection = Direction.Down;
	}

	public void ChangeRoom(string room)
	{
		GetTree().ChangeSceneToFile($"res://scenes/rooms/{room}.tscn");
		LocationMarkerName = string.Empty;
		PlayerDirection = Direction.Down;
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

	public async Task PlayRoomCutscene(string name)
	{
		await CurrentRoom.PlayCutscene(name);
	}

	public void ChangeWindowSize(WindowSize size)
	{
		Settings.ChangeWindowSize(size, GetWindow());		
	}
}
