using Godot;
using Godot.Collections;
using System;
using TheWizardCoder.Autoload;
using TheWizardCoder.Data;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Utils
{
    public static class DataLoader
    {
        public static Variant GetJsonData(string fileName)
        {
            if (!FileAccess.FileExists(fileName))
            {
                GD.PrintErr($"{fileName} does not exist");
                return new Json();
            }

            using FileAccess data = FileAccess.Open(fileName, FileAccess.ModeFlags.Read);
            string jsonString = data.GetAsText();

            Json json = new();
            Error jsonError = json.Parse(jsonString);

            if (jsonError != Error.Ok)
            {
                GD.PrintErr($"Json parse error: {jsonError}");
            }

            return json.Data;
        }

        public static System.Collections.Generic.Dictionary<string, Item> LoadItems(Global global)
        {
            System.Collections.Generic.Dictionary<string, Item> items = new();

            Variant jsonData = GetJsonData("res://info/item_descriptions.json");
            Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

            foreach (System.Collections.Generic.KeyValuePair<string, Dictionary<string, Variant>> pair in parsedData)
            {
                Dictionary<string, Variant> dict = pair.Value;

                string name = pair.Key;
                string description = (string)dict["Description"];
                int effect = (int)dict["Effect"];
                ItemType type = Enum.Parse<ItemType>((string)dict["Type"]);

                Item item = new(name, description, effect, type);

                if (dict.ContainsKey("Price"))
                {
                    int price = (int)dict["Price"];
                    item.MakeSellable(price);
                }

                if (type == ItemType.Key || type == ItemType.Magic)
                {
                    if (dict.ContainsKey("AdditionalData"))
                    {
                        string[] data = (string[])dict["AdditionalData"];
                        item.SetOnUsed(global, data);
                    }

                    if (dict.ContainsKey("OnEquipped"))
                    {
                        string[] onEquippedArr = (string[])dict["OnEquipped"];
                        item.SetOnEquipped(global, onEquippedArr);
                    }
                }

                if (dict.ContainsKey("Equippable"))
                {
                    item.Equippable = dict["Equippable"].AsBool();
                }

                items.Add(item.Name, item);
            }

            return items;
        }

        public static System.Collections.Generic.Dictionary<string, MagicSpell> LoadMagicSpells()
        {
            System.Collections.Generic.Dictionary<string, MagicSpell> magicSpells = new();

            Variant jsonData = GetJsonData("res://info/magic_descriptions.json");
            Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

            foreach (System.Collections.Generic.KeyValuePair<string, Dictionary<string, Variant>> pair in parsedData)
            {
                Dictionary<string, Variant> dict = pair.Value;

                string name = (string)dict["Name"];
                string description = (string)dict["Description"];
                int effect = (int)dict["Effect"];
                int cost = (int)dict["Cost"];
                int shopPrice = (int)dict["ShopPrice"];
                CharacterType targetType = Enum.Parse<CharacterType>((string)dict["TargetType"]);
                MagicSpellType spellType = Enum.Parse<MagicSpellType>((string)dict["Type"]);

                MagicSpell magicSpell = new(name, description, effect, cost, shopPrice, targetType, spellType);

                if (magicSpell.SpellType == MagicSpellType.ApplyBattleEffect)
                {
                    string[] additionalData = (string[])dict["BattleEffect"];
                    magicSpell.SetBattleEffectData(additionalData);
                }

                magicSpells.Add(pair.Key, magicSpell);
            }

            return magicSpells;
        }

        public static System.Collections.Generic.Dictionary<string, Shop> LoadShops()
        {
            System.Collections.Generic.Dictionary<string, Shop> shops = new();

            Variant jsonData = GetJsonData("res://info/shops.json");
            Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

            foreach (System.Collections.Generic.KeyValuePair<string, Dictionary<string, Variant>> pair in parsedData)
            {
                Dictionary<string, Variant> dict = pair.Value;

                string name = pair.Key;
                Array<string> items = (Array<string>)dict["Items"];

                Shop shop = new(name, items);
                shops.Add(pair.Key, shop);
            }

            return shops;
        }

        public static System.Collections.Generic.Dictionary<string, Character> LoadCharacters(Global global)
        {
            System.Collections.Generic.Dictionary<string, Character> characters = new();

            Variant jsonData = GetJsonData("res://info/characters.json");
            Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

            foreach (System.Collections.Generic.KeyValuePair<string, Dictionary<string, Variant>> pair in parsedData)
            {
                Dictionary<string, Variant> dict = pair.Value;

                if (pair.Value != null)
                {
                    dict["Name"] = pair.Key;

                    Character character = new(dict, global);
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

            foreach (System.Collections.Generic.KeyValuePair<string, string> item in parsedData)
            {
                gameIntroStrings[item.Key] = item.Value;
            }

            return gameIntroStrings;
        }

        public static System.Collections.Generic.Dictionary<string, Armour> LoadArmours()
        {
            System.Collections.Generic.Dictionary<string, Armour> armours = new();

            Variant jsonData = GetJsonData("res://info/armours.json");
            Dictionary<string, Dictionary<string, Variant>> parsedData = (Dictionary<string, Dictionary<string, Variant>>)jsonData;

            foreach (System.Collections.Generic.KeyValuePair<string, Dictionary<string, Variant>> item in parsedData)
            {
                Dictionary<string, Variant> dict = item.Value;

                string name = item.Key;
                string description = (string)dict["Description"];
                ArmourType armourType = Enum.Parse<ArmourType>((string)dict["Type"]);
                int effect = (int)dict["Effect"];
                BattleEffectType effectType = Enum.Parse<BattleEffectType>((string)dict["EffectType"]);
                int price = (int)dict["Price"];

                Armour armour = new(name, description, armourType, effect, effectType, price);

                armours.Add(name, armour);
            }

            return armours;
        }
    }
}