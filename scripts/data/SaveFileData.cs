using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using TheWizardCoder.Autoload;
using TheWizardCoder.Enums;

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
        public CharacterData Stats { get; set; }
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
                return magicSpells;
            }
            private set
            {
                magicSpells = value;
            }
        }

        public List<CharacterData> Allies { get; set; } = new();

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

        public SaveFileData(CharacterData stats)
        {
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
    }
}