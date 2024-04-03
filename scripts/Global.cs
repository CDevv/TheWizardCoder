using Godot;
using Godot.Collections;
using System;

public partial class Global : Node
{
	public BaseRoom CurrentRoom { get; set; }
	public bool CanWalk = true;
	public string SaveName { get; set; }
	public DateTime StartedOn { get; set; }
	public TimeSpan TimeSpent { get; set; }
	public string Location { get; set; }
	public int Health { get; set; } = 100;

	private Array<string> inventory = new();

	public Array<string> Inventory
	{
		get
		{
			return inventory;
		}
	}

	public override void _Ready()
	{
	}

	public void AddToInventory(string item)
	{
		inventory.Add(item);
	}

	public void RemoveFromInventory(string item)
	{
		inventory.Remove(item);
	}

	public bool HasItemInInventory(string item)
	{
		return inventory.Contains(item);
	}

	public void SaveGame(string saveName)
	{
		TimeSpent = DateTime.Now.Subtract(StartedOn);

		using var writeSaveFile = FileAccess.Open($"user://{saveName}.wand", FileAccess.ModeFlags.Write);

		//Save to file
		writeSaveFile.StoreVar(SaveName);
		writeSaveFile.StoreVar(StartedOn.ToBinary());
		writeSaveFile.StoreVar(TimeSpent.Seconds);
		writeSaveFile.StoreVar(Location);
		writeSaveFile.StoreVar(Health);
		writeSaveFile.StoreVar(inventory);
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
		if (!FileAccess.FileExists($"user://{saveName}.wand"))
        {
            GD.PushWarning($"File {saveName}.wand does not exist");
			SaveName = saveName;
			StartedOn = DateTime.Now;
			TimeSpent = TimeSpan.Zero;
            SaveGame(saveName);
            return;
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

            GD.Print($"{saveName}.wand: save loaded");
			SaveName = (string)readSave.GetVar();
			StartedOn = DateTime.FromBinary((long)readSave.GetVar());
			TimeSpent = TimeSpan.FromSeconds((long)readSave.GetVar());
			Location = (string)readSave.GetVar();
            Health = (int)readSave.GetVar();
			inventory = (Array<string>)readSave.GetVar();
        }
        else
        {
            GD.PushWarning($"{saveName}.ini: The hashes don't match");
            SaveGame(saveName);
            return;
        }
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

			data.SaveName = (string)readSave.GetVar();
			readSave.GetVar();
			data.TimeSpent = new TimeSpan((long)readSave.GetVar());
			data.Location = (string)readSave.GetVar();
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

	public void ChangeRoom(PackedScene scene)
	{
		GetTree().ChangeSceneToPacked(scene);
	}
}
