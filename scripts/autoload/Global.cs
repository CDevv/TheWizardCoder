using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Utils;

namespace TheWizardCoder.Autoload
{
	public partial class Global : Node
	{
		public BaseRoom CurrentRoom { get; set; }
		public string LocationMarkerName { get; set; }
		public Direction PlayerDirection { get; set; }
		public string ChosenSaveSlot { get; set; }
		public bool HasLoadedGame { get; set; } = false;
		public bool CanWalk = true;
		public bool GameDisplayEnabled { get; set; } = true;
		public bool IsInShop { get; set; }
		public bool PlayerIsOnStairs { get; set; } = false;
		public bool StairsInverted { get; set; } = false;
		public bool StairsGoUp { get; set; } = true;
		public bool IsInCutscene { get; set; } = false;
		public SaveFileData PlayerData { get; set; } = new();
		public SettingsConfig Settings { get; set; } = new();
		public System.Collections.Generic.Dictionary<string, Item> ItemDescriptions { get; private set; } = new();
		public System.Collections.Generic.Dictionary<string, MagicSpell> MagicSpells { get; private set; } = new();
		public System.Collections.Generic.Dictionary<string, CharacterData> Characters { get; private set; } = new();
		public System.Collections.Generic.Dictionary<string, Shop> Shops { get; set; } = new();
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
				LoadShops();
			}
			catch (System.Exception e)
			{
				GD.PrintErr(e.ToString());
			}
		}

		public void AddToInventory(string item, bool onlyOne = false)
		{
			PlayerData.AddToInventory(item, onlyOne);
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

				if (character.Type == CharacterType.Enemy)
				{
					ResourceLoader.LoadThreadedRequest($"res://assets/battle/enemies/{pair.Key}.png");
				}
			}
		}

		public void LoadShops()
		{
			Variant jsonData = GetJsonData("res://info/shops.json");
			Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

			foreach (var pair in parsedData)
			{
				Dictionary<string, Variant> dict = pair.Value;
				dict["Name"] = pair.Key;

				Shop shop = new();
				shop.ApplyDictionary(dict);

				Shops.Add(pair.Key, shop);
			}
		}

		public void CreateSaveFile(string fileName, string saveName)
		{
			SaveFileData data = new SaveFileData();
			data.FileName = fileName;
			data.SaveName = saveName;
			
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
			PlayerData.TimeSpent = PlayerData.TimeSpent.Add(DateTime.Now - PlayerData.LastSaved);
			PlayerData.LastSaved = DateTime.Now;
			PlayerData.LocationVector = CurrentRoom.Player.Position;
			PlayerData.SceneDefaultMarker = CurrentRoom.DefaultMarkerName;
			SaveFileData data = PlayerData;		

			FileAccess file = FileAccess.Open($"user://{fileName}.wand", FileAccess.ModeFlags.Write);
			file.StoreVar(JsonConvert.SerializeObject(data));
			file.Close();

			FileAccess hashFile = FileAccess.Open($"user://{fileName}.ini", FileAccess.ModeFlags.Write);
			byte[] hash = HashUtils.CalculateHash(fileName);
			hashFile.StoreVar(hash);
			hashFile.Close();

			PlayerData.LocationVector = Vector2.Zero;
		}

		public void LoadSaveFile(string saveName)
		{
			SaveFileData data = ReadSaveFileData(saveName);
			PlayerData = data;
			PlayerData.LastSaved = DateTime.Now;

			if (data.IsSaveEmpty)
			{
				PlayerData.SaveName = saveName;
				CreateSaveFile(saveName, saveName);
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

		public void GoToGameIntro()
		{
			GetTree().ChangeSceneToFile("res://scenes/rooms/game_intro.tscn");
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

		public void OpenShop(string shopName)
		{
			CanWalk = false;
			GameDisplayEnabled = false;
			CurrentRoom.ShopDisplay.ShowDisplay(shopName);
		}
	}
}