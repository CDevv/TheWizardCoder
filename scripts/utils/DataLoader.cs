using System;
using Godot;
using Godot.Collections;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Utils
{
    public static class DataLoader
    {
        public static Global Global { get; set; }

        public static Variant GetJsonData(string fileName)
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

        public static System.Collections.Generic.Dictionary<string, Item> LoadItems()
        {
            System.Collections.Generic.Dictionary<string, Item> items = new();

            Variant jsonData = GetJsonData("res://info/item_descriptions.json");
			Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

			foreach (var pair in parsedData)
			{
				Item item = new Item();
				item.ApplyDictionary(pair.Value);
				item.Name = pair.Key;
				items.Add(item.Name, item);
			}

            return items;
        }

        public static System.Collections.Generic.Dictionary<string, MagicSpell> LoadMagicSpells()
        {
            System.Collections.Generic.Dictionary<string, MagicSpell> magicSpells = new();

            Variant jsonData = GetJsonData("res://info/magic_descriptions.json");
			Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

			foreach (var pair in parsedData)
			{
				MagicSpell magicSpell = new MagicSpell();
				magicSpell.ApplyDictionary(pair.Value);
				magicSpells.Add(pair.Key, magicSpell);
			}

            return magicSpells;
        }

        public static System.Collections.Generic.Dictionary<string, Shop> LoadShops()
        {
            System.Collections.Generic.Dictionary<string, Shop> shops = new();

            Variant jsonData = GetJsonData("res://info/shops.json");
			Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

			foreach (var pair in parsedData)
			{
				Dictionary<string, Variant> dict = pair.Value;
				dict["Name"] = pair.Key;

				Shop shop = new();
				shop.ApplyDictionary(dict);

				shops.Add(pair.Key, shop);
			}

            return shops;
        }

        public static System.Collections.Generic.Dictionary<string, CharacterData> LoadCharacters()
        {
            System.Collections.Generic.Dictionary<string, CharacterData> characters = new();

            Variant jsonData = GetJsonData("res://info/enemies.json");
			Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

			foreach (var pair in parsedData)
			{
				Dictionary<string, Variant> dict = pair.Value;

				if (pair.Value != null)
				{
					dict["Name"] = pair.Key;

					CharacterData character = new(dict, Global);
					characters.Add(pair.Key, character);

					if (character.Type == CharacterType.Enemy)
					{
						ResourceLoader.LoadThreadedRequest($"res://assets/battle/enemies/{pair.Key}.png");
					}
				}
			}

            return characters;
        }

        public static System.Collections.Generic.Dictionary<string, string> LoadGameIntro()
        {
            System.Collections.Generic.Dictionary<string, string> gameIntroStrings = new();

            Variant jsonData = GetJsonData("res://info/game_intro.json");
			Dictionary<string, string> parsedData = (Dictionary<string, string>)jsonData;

			foreach (var item in parsedData)
			{
				gameIntroStrings[item.Key] = item.Value;
			}

            return gameIntroStrings;
        }
    }
}