using System;
using System.Xml.Linq;
using Godot;
using Godot.Collections;
using Moq;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Utils
{
    public class DataLoader
    {
        private Global Global { get; set; }

		public DataLoader(Global global)
		{
			Global = global;
		}

        public Variant GetJsonData(string fileName)
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

        public System.Collections.Generic.Dictionary<string, Item> LoadItems()
        {
            System.Collections.Generic.Dictionary<string, Item> items = new();

            Variant jsonData = GetJsonData("res://info/item_descriptions.json");
			Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

			foreach (var pair in parsedData)
			{
				var dict = pair.Value;

				string name = pair.Key;
                string description = (string)dict["Description"];
                int effect = (int)dict["Effect"];
                ItemType type = Enum.Parse<ItemType>((string)dict["Type"]);

                Item item = new Item(name, description, effect, type);

                if (dict.ContainsKey("Price"))
                {
                    int price = (int)dict["Price"];
					item.MakeSellable(price);
                }

                if (type == ItemType.Key || type == ItemType.Magic)
				{
					string[] data = (string[])dict["AdditionalData"];
                    item.AddAdditionalData(data);
                }

                items.Add(item.Name, item);
			}

            return items;
        }

        public System.Collections.Generic.Dictionary<string, MagicSpell> LoadMagicSpells()
        {
            System.Collections.Generic.Dictionary<string, MagicSpell> magicSpells = new();

            Variant jsonData = GetJsonData("res://info/magic_descriptions.json");
			Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

			foreach (var pair in parsedData)
			{
				var dict = pair.Value;

                string name = (string)dict["Name"];
                string description = (string)dict["Description"];
                int effect = (int)dict["Effect"];
                int cost = (int)dict["Cost"];
                int shopPrice = (int)dict["ShopPrice"];
                CharacterType targetType = Enum.Parse<CharacterType>((string)dict["TargetType"]);
                MagicSpellType spellType = Enum.Parse<MagicSpellType>((string)dict["Type"]);

                MagicSpell magicSpell = new MagicSpell(name, description, effect, cost, shopPrice, targetType, spellType);

                if (magicSpell.SpellType == MagicSpellType.ApplyBattleEffect)
                {
                    string[] additionalData = (string[])dict["BattleEffect"];
                    magicSpell.SetBattleEffectData(additionalData);
                }

                magicSpells.Add(pair.Key, magicSpell);
			}

            return magicSpells;
        }

        public System.Collections.Generic.Dictionary<string, Shop> LoadShops()
        {
            System.Collections.Generic.Dictionary<string, Shop> shops = new();

            Variant jsonData = GetJsonData("res://info/shops.json");
			Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

			foreach (var pair in parsedData)
			{
				var dict = pair.Value;

                string name = pair.Key;
                ShopType type = Enum.Parse<ShopType>((string)dict["Type"]);
                Array<string> items = (Array<string>)dict["Items"];

                Shop shop = new(name, type, items);
				shops.Add(pair.Key, shop);
			}

            return shops;
        }

        public System.Collections.Generic.Dictionary<string, Character> LoadCharacters()
        {
            System.Collections.Generic.Dictionary<string, Character> characters = new();

            Variant jsonData = GetJsonData("res://info/enemies.json");
			Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

			foreach (var pair in parsedData)
			{
				Dictionary<string, Variant> dict = pair.Value;

				if (pair.Value != null)
				{
					dict["Name"] = pair.Key;

					Character character = new(dict, Global);
					characters.Add(pair.Key, character);

					if (character.Type == CharacterType.Enemy)
					{
						ResourceLoader.LoadThreadedRequest($"res://assets/battle/enemies/{pair.Key}.png");
					}
				}
			}

            return characters;
        }

        public System.Collections.Generic.Dictionary<string, string> LoadGameIntro()
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

        public System.Collections.Generic.Dictionary<string, Armour> LoadArmours()
        {
            System.Collections.Generic.Dictionary<string, Armour> armours = new();

            Variant jsonData = GetJsonData("res://info/armours.json");
            Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

            foreach (var item in parsedData)
            {
                var dict = item.Value;

                string name = item.Key;
                string description = (string)dict["Description"];
                ArmourType armourType = Enum.Parse<ArmourType>((string)dict["Type"]);
                int effect = (int)dict["Effect"];
                BattleEffectType effectType = Enum.Parse<BattleEffectType>((string)dict["EffectType"]);

                Armour armour = new(name, description, armourType, effect, effectType);

                armours.Add(name, armour);
            }

            return armours;
        }
    }
}