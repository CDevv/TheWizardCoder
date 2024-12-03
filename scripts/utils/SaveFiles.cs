using System;
using Godot;
using Newtonsoft.Json;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;

namespace TheWizardCoder.Utils
{
    public class SaveFileHelper
    {
        public Global Global { get; set; }
		public SaveFileData PlayerData { get; set; }

		public SaveFileHelper(Global global)
		{
			Global = global;
			PlayerData = new(global.Characters["Nolan"]);
		}

        public void CreateSaveFile(string fileName, string saveName)
        {
            SaveFileData data = new SaveFileData(Global.Characters["Nolan"]);
			data.FileName = fileName;
			data.SaveName = saveName;
			data.Stats.Global = null;

			FileAccess file = FileAccess.Open($"user://{fileName}.wand", FileAccess.ModeFlags.Write);
			file.StoreVar(JsonConvert.SerializeObject(data));
			file.Close();

			FileAccess hashFile = FileAccess.Open($"user://{fileName}.ini", FileAccess.ModeFlags.Write);
			byte[] hash = HashUtils.CalculateHash(fileName);
			hashFile.StoreVar(hash);
			hashFile.Close();

            PlayerData = data;
        }

        public void UpdateSaveFile(string fileName)
        {
            PlayerData.TimeSpent = Global.PlayerData.TimeSpent.Add(DateTime.Now - Global.PlayerData.LastSaved);
			PlayerData.LastSaved = DateTime.Now;
			PlayerData.LocationVector = Global.CurrentRoom.Player.Position;
			PlayerData.SceneDefaultMarker = Global.CurrentRoom.DefaultMarkerName;

			SaveFileData data = Global.PlayerData;
			data.Stats.Global = null;
			for (var i = 0; i < data.Allies.Count; i++)
			{
				data.Allies[i].Global = null;
			}

			FileAccess file = FileAccess.Open($"user://{fileName}.wand", FileAccess.ModeFlags.Write);
			file.StoreVar(JsonConvert.SerializeObject(data));
			file.Close();

			FileAccess hashFile = FileAccess.Open($"user://{fileName}.ini", FileAccess.ModeFlags.Write);
			byte[] hash = HashUtils.CalculateHash(fileName);
			hashFile.StoreVar(hash);
			hashFile.Close();

			Global.PlayerData.LocationVector = Vector2.Zero;
        }

        public SaveFileData ReadSaveFile(string saveName)
        {
            SaveFileData data = new SaveFileData(Global.Characters["Nolan"]);

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
			byte[] actualHash = HashUtils.CalculateHash(saveName);

			if (HashUtils.CompareHashes(actualHash, expectedHash))
			{
				using var readSave = FileAccess.Open($"user://{saveName}.wand", FileAccess.ModeFlags.Read);
				string savedData = (string)readSave.GetVar();
				data = JsonConvert.DeserializeObject<SaveFileData>(savedData, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace, NullValueHandling = NullValueHandling.Ignore });
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

        public void LoadSaveFile(string saveName)
        {
            SaveFileData data = ReadSaveFile(saveName);
			PlayerData = data;
			PlayerData.LastSaved = DateTime.Now;
			PlayerData.Stats.Global = Global;

			if (data.IsSaveEmpty)
			{
				Global.PlayerData.SaveName = saveName;
				CreateSaveFile(saveName, saveName);
			}
        }

        public void DeleteSaveFile(string saveName)
        {
            DirAccess dir = DirAccess.Open("user://");
			dir.Remove($"{saveName}.ini");
			dir.Remove($"{saveName}.wand");
        }
    }
}