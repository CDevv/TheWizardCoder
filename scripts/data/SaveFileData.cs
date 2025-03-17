using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using TheWizardCoder.Autoload;

namespace TheWizardCoder.Data
{
    public partial class SaveFileData : Node
    {
        public new StringName Name { get; set; } = "SaveFileNode";
        public bool IsSaveEmpty { get; set; } = false;
        public string FileName { get; set; }
        public string SaveName { get; set; }
        public DateTime LastSaved { get; set; }
        public DateTime StartedOn { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public string SceneFileName { get; set; }
        public string SceneDefaultMarker { get; set; }
        public string Location { get; set; }
        public Vector2 LocationVector { get; set; }
        public Character Stats { get; set; }
        public int Gold { get; set; }
        private Array<string> inventory = new();
        private Array<string> magicSpells = new();

        public Array<string> Inventory
        {
            get
            {
                return inventory;
            }
            private set
            {
                inventory = value;
            }
        }

        public Array<string> MagicSpells
        {
            get
            {
                return Stats.MagicSpells;
            }
            private set
            {
                Stats.MagicSpells = value;
            }
        }

        public List<string> Armours { get; set; } = new();

        public List<Character> Allies { get; set; } = new();

        public bool TestCodeProblem { get; set; } = false;
        public bool TestChest { get; set; } = false;
        public bool TestChest2 { get; set; } = false;
        public bool TestCutscene { get; set; } = false;

        //Playthrough properties
        public bool HasPlayedIntro { get; set; } = false;
        public bool HasMessageFromShimble { get; set; } = false;
        public bool HasQuestFromShimble { get; set; } = false;
        public bool HasSolvedShimbleChair { get; set; } = false;
        public bool HasVisitedZenHouse { get; set; } = false;
        public bool HasSolvedZenHouse { get; set; } = false;
        public bool HasEncounteredKeenelm { get; set; } = false;
        public bool HasEncounteredNara { get; set; } = false;
        public bool HasPlayedKeenelmCutscene { get; set; } = false;
        public bool HasSolvedFarmGlitch { get; set; } = false;
        public bool HasReceivedAppleFromNara { get; set; } = false;
        public bool HasMetBerry { get; set; } = false;
        public bool TavernPuzzleIntro { get; set; } = false;
        public bool HasSolvedTavernGlitch { get; set; } = false;
        public bool HasSolvedWatchtowerGlitch { get; set; } = false;
        public bool HasMetLinton { get; set; } = false;
        public bool LintonDummyCutscene { get; set; } = false;
        public bool WatchtowerChest { get; set; } = false;
        public bool VillageAppleChest { get; set; } = false;
        public bool UnlockedNobeCabin { get; set; } = false;
        public bool HasMetGertrude { get; set; } = false;
        public bool SeenRaft { get; set; } = false;
        public bool UsedRaft { get; set; } = false;
        public bool PassedWaterChallenges { get; set; } = false;
        public bool HasMetTimothy { get; set; } = false;
        public bool HasMetGregory { get; set; } = false;
        public bool VisitedTimothyHouse { get; set; } = false;
        public bool FishingRodSolved { get; set; } = false;
        public bool VindiTreeSolved { get; set; } = false;
        public bool HasMetKulber { get; set; } = false;
        public bool HasMetCraig { get; set; } = false;
        public bool HasFinishedCraigQuest { get; set; } = false;
        public bool DefeatedQuestEnemy1 { get; set; } = false;
        public bool DefeatedQuestEnemy2 { get; set; } = false;
        public bool DefeatedQuestEnemy3 { get; set; } = false;
        public bool GotGeraldHouseCyanberry { get; set; } = false;
        public bool VindiHousesPuzzle { get; set; } = false;
        public bool CompletedAllVindiQuests { get; set; } = false;
        public bool SolvedHotelRoom { get; set; } = false;

        public SaveFileData(Character stats)
        {
            Armours = new();

            IsSaveEmpty = true;

            StartedOn = DateTime.Now;
            LastSaved = DateTime.Now;
            TimeSpent = TimeSpan.Zero;

            SceneFileName = "first_room";
            Location = "Home";
            SceneDefaultMarker = "AfterCutsceneMarker";

            Stats = stats;
            Gold = 100;
        }

        public void EnsureDefaults(Global global)
        {
            if (Armours.Count == 0)
            {
                AddArmour("Wooden Wand");
            }

            if (Stats.EquippedArmours.Count == 0)
            {
                Stats.InitializeDefaults();

                Godot.Collections.Dictionary<int, string> characterEquippedArmours = global.Characters[Stats.Name].EquippedArmours;

                for (int i = 0; i < characterEquippedArmours.Count; i++)
                {
                    Stats.EquipArmour(i, characterEquippedArmours[i]);
                }

                Stats.ApplyArmourEffects();
            }
        }

        public void AddToInventory(string item, bool onlyOne = false)
        {
            if (onlyOne)
            {
                if (!inventory.Contains(item))
                {
                    inventory.Add(item);
                }
            }
            else
            {
                inventory.Add(item);
            }
        }

        public void RemoveFromInventory(string item)
        {
            inventory.Remove(item);
        }

        public void RemoveFromInventory(int id)
        {
            inventory.RemoveAt(id);
        }

        public bool HasItemInInventory(string item)
        {
            return inventory.Contains(item);
        }

        public void AddMagicSpell(string name)
        {
            Stats.MagicSpells.Add(name);
        }

        public void RemoveMagicSpell(string name)
        {
            Stats.MagicSpells.Remove(name);
        }

        public void AddArmour(string name)
        {
            Armours.Add(name);
        }

        public bool OwnsArmour(string name)
        {
            return Armours.Contains(name);
        }
    }
}