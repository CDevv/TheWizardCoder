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
		public bool CanWalk { get; set; } = true;
		public bool GameDisplayEnabled { get; set; } = true;
		public bool IsInShop { get; set; }
		public bool PlayerIsOnStairs { get; set; } = false;
		public bool StairsInverted { get; set; } = false;
		public bool StairsGoUp { get; set; } = true;
		public bool IsInCutscene { get; set; } = false;
		public bool IsFishing { get; set; } = false;
        public SaveFileData PlayerData 
		{ 
			get { return SaveFiles.PlayerData; }
		}
		public SaveFileHelper SaveFiles { get; private set; }
		public DataLoader DataLoader { get; private set; }
		public SettingsConfig Settings { get; set; } = new();
		public System.Collections.Generic.Dictionary<string, Item> ItemDescriptions { get; private set; } = new();
		public System.Collections.Generic.Dictionary<string, MagicSpell> MagicSpells { get; private set; } = new();
		public System.Collections.Generic.Dictionary<string, CharacterData> Characters { get; private set; } = new();
		public System.Collections.Generic.Dictionary<string, Shop> Shops { get; private set; } = new();
		public System.Collections.Generic.Dictionary<string, string> GameIntroStrings { get; private set; } = new();
        public CodeProblem FishingProblemData { get; private set; } = new();
        public int[] ReverseDirections { get; } = { 1, 0, 3, 2 };

		public override void _Ready()
		{
			try
			{			
				DataLoader = new(this);
				LoadData();
				SaveFiles = new(this);

				Settings.LoadSettings();
				Settings.ApplySettings();
			}
			catch (System.Exception e)
			{
				GD.PrintErr(e.ToString());
			}
		}

		private void LoadData()
		{
			GameIntroStrings = DataLoader.LoadGameIntro();
			ItemDescriptions = DataLoader.LoadItems();
			MagicSpells = DataLoader.LoadMagicSpells();
			Characters = DataLoader.LoadCharacters();
			Shops = DataLoader.LoadShops();

            Variant jsonData = DataLoader.GetJsonData("res://info/fishing_problem.json");
            Dictionary<string, Variant> parsedData = (Dictionary<string, Variant>)jsonData;

			CodeProblem codeProblem = new();
			codeProblem.ApplyDictionary(parsedData);
			FishingProblemData = codeProblem;
        }

		public void AddToInventory(string item, bool onlyOne = false)
		{
			PlayerData.AddToInventory(item, onlyOne);
		}

		public void AddToInventory(string item)
		{
			PlayerData.AddToInventory(item, false);
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

		public void ChangeRoom(string room)
		{
			ChangeRoom(room, "DefaultPlayerPos", Direction.Down);
		}

		public void ChangeRoom(string room, string playerLocation, Direction direction)
		{
			CurrentRoom.TransitionToRoom(room, playerLocation, direction);
        }

        public void GoToMainMenu()
		{
			GetTree().ChangeSceneToFile("res://scenes/rooms/main_menu.tscn");
		}

		public void GoToGameIntro()
		{
			GetTree().ChangeSceneToFile("res://scenes/rooms/game_intro.tscn");
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

		public bool PlayerHasFollower()
		{
			return CurrentRoom.Player.HasFollower;
		}

		public void OpenShop(string shopName)
		{
			CanWalk = false;
			GameDisplayEnabled = false;
			CurrentRoom.ShopDisplay.ShowDisplay(shopName);
		}
	}
}