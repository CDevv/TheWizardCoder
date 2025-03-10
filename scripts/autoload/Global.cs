using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;
using TheWizardCoder.Abstractions;
using TheWizardCoder.Utils;
using Moq;

namespace TheWizardCoder.Autoload
{
    /// <summary>
    /// An autoload/singleton class for storing the current state of the game session.
    /// </summary>
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
        public System.Collections.Generic.Dictionary<string, Character> Characters { get; private set; } = new();
        public System.Collections.Generic.Dictionary<string, Shop> Shops { get; private set; } = new();
        public System.Collections.Generic.Dictionary<string, Armour> Armours { get; private set; } = new();
        public System.Collections.Generic.Dictionary<string, string> GameIntroStrings { get; private set; } = new();
        public CodeProblem FishingProblemData { get; private set; }
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
            Armours = DataLoader.LoadArmours();

            LoadFishingProblemData();
        }

        private void LoadFishingProblemData()
        {
            Variant jsonData = DataLoader.GetJsonData("res://info/fishing_problem.json");
            var dict = (Dictionary<string, Variant>)jsonData;

            string uniqueIdentifier = (string)dict["UniqueIdentifier"];
            string code = (string)dict["Code"];
            var items = new Array<string>((string[])dict["Items"]);

            var solvableAreasVectors = new Dictionary<string, Vector2>();
            var solvableAreas = (Dictionary<string, Variant>)dict["SolvableAreas"];
            foreach (var item in solvableAreas)
            {
                int[] arr = (int[])item.Value;
                Vector2 vector = new Vector2(arr[0], arr[1]);

                solvableAreasVectors[item.Key] = vector;
            }

            FishingProblemData = new(uniqueIdentifier, code, items, solvableAreasVectors);
        }

        /// <summary>
        /// Add an item to the player's inventory.
        /// </summary>
        /// <param name="item">Name of the item to be added.</param>
        /// <param name="onlyOne">Check if the item already exists.</param>
        public void AddToInventory(string item, bool onlyOne = false)
        {
            PlayerData.AddToInventory(item, onlyOne);
        }

        /// <summary>
        /// Add an item to the player's inventory.
        /// </summary>
        /// <param name="item">Name of the item to be added.</param>
        public void AddToInventory(string item)
        {
            PlayerData.AddToInventory(item, false);
        }

        /// <summary>
        /// Remove an item from the player's inventory if it exists.
        /// </summary>
        /// <param name="item">Name of the item to be removed.</param>
        public void RemoveFromInventory(string item)
        {
            PlayerData.RemoveFromInventory(item);
        }

        /// <summary>
        /// Check if the player has the specified <paramref name="item"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Whether the item with such a name eixsts within the player's inventory.</returns>
        public bool HasItemInInventory(string item)
        {
            return PlayerData.HasItemInInventory(item);
        }

        /// <summary>
        /// Add a magic spell to the player's inventory.
        /// </summary>
        /// <param name="name">Name of the magic spell.</param>
        public void AddMagicSpell(string name)
        {
            PlayerData.AddMagicSpell(name);
        }

        /// <summary>
        /// Change the current room to the one with the specified name.
        /// </summary>
        /// <param name="room">Name of the room.</param>
        public void ChangeRoom(string room)
        {
            ChangeRoom(room, "DefaultPlayerPos", Direction.Down);
        }

        /// <summary>
        /// Change the current room to the one with the specified name and place the <c>Player</c>
        /// on the position of the specified <c>Marker2D</c> and turn it to the specified direction.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="playerLocation"></param>
        /// <param name="direction"></param>
        public void ChangeRoom(string room, string playerLocation, Direction direction)
        {
            LocationMarkerName = playerLocation;
            PlayerDirection = direction;

            string fileName = $"res://scenes/rooms/{room}.tscn";
            GetTree().ChangeSceneToFile(fileName);
        }

        /// <summary>
        /// Change the current scene to the Main Menu.
        /// </summary>
        public void GoToMainMenu()
        {
            GetTree().ChangeSceneToFile("res://scenes/rooms/main_menu.tscn");
        }

        /// <summary>
        /// Change the current scene to the Game intro.
        /// </summary>
        public void GoToGameIntro()
        {
            GetTree().ChangeSceneToFile("res://scenes/rooms/game_intro.tscn");
        }

        /// <summary>
        /// Get the value of a playthrough property.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Variant GetPlayerData(string key)
        {
            return PlayerData.Get(key);
        }

        /// <summary>
        /// Set the value of a playthrough property.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetPlayerData(string key, Variant value)
        {
            PlayerData.Set(key, value);
        }

        /// <summary>
        /// Call a method of the current room with specified <paramref name="methodName"/>
        /// </summary>
        /// <param name="methodName">Name of the method</param>
        public void CallRoomMethod(string methodName)
        {
            CurrentRoom.CallDeferred(methodName);
        }

        /// <summary>
        /// Check if the player has a follower.
        /// </summary>
        /// <returns>Whether the player has a follower or not.</returns>
        public bool PlayerHasFollower()
        {
            return CurrentRoom.Player.HasFollower;
        }

        /// <summary>
        /// Open the <c>ShopDisplay</c> with a Shop name.
        /// </summary>
        /// <param name="shopName">Name of the <c>Shop</c></param>
        public void OpenShop(string shopName)
        {
            CanWalk = false;
            GameDisplayEnabled = false;
            CurrentRoom.ShopDisplay.ShowDisplay(shopName);
        }
    }
}