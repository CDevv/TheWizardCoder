using System;
using Godot;
using Godot.Collections;

public partial class SaveFileData : Node
{
    public bool IsSaveEmpty { get; set; } = false;
    public string SaveName { get; set; }
	public DateTime LastSaved { get; set; }
	public DateTime StartedOn { get; set; }
	public TimeSpan TimeSpent { get; set; }
    public string SceneFileName { get; set; }
    public string SceneDefaultMarker { get; set; }
	public string Location { get; set; }
    public Vector2 LocationVector { get; set; }
    public CharacterData Stats { get; set; }
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

    public SaveFileData()
    {
        IsSaveEmpty = true;
        
        StartedOn = DateTime.Now;
        LastSaved = DateTime.Now;
        TimeSpent = TimeSpan.Zero;

        SceneFileName = "first_room";
        Location = "Home";
        SceneDefaultMarker = "AfterCutsceneMarker";

        Stats = new CharacterData() {
            Name = "Nolan",
            MaxHealth = 50, Health = 50,
            MaxPoints = 10, Points = 10,
            AttackPoints = 20, DefensePoints = 10,
            MagicSpells = new() {
                "Fireball"
            }
        };
    }

    public Dictionary<string, Variant> GenerateDictionary()
    {
        return new Dictionary<string, Variant>()
        {
            {"SaveName", SaveName},
            {"LastSaved", LastSaved.ToBinary()},
            {"StartedOn", StartedOn.ToBinary()},
            {"TimeSpent", TimeSpent.TotalSeconds},
            {"SceneDefaultMarker", SceneDefaultMarker},
            {"SceneFileName", SceneFileName},
            {"Location", Location},
            {"LocationVector", LocationVector},
            {"Health", Stats.Health},
            {"Inventory", Inventory},

            {"HasPlayedIntro", HasPlayedIntro},
            {"HasMessageFromShimble", HasMessageFromShimble},
            {"HasQuestFromShimble", HasQuestFromShimble},
            {"HasSolvedShimbleChair", HasSolvedShimbleChair},
            {"HasVisitedZenHouse", HasVisitedZenHouse},
            {"HasSolvedZenHouse", HasSolvedZenHouse},
        };
    }

    public void ApplyDictionary(Dictionary<string, Variant> dictionary)
    {
        SaveName = (string)dictionary["SaveName"];
        LastSaved = DateTime.FromBinary((long)(dictionary["LastSaved"]));
        StartedOn = DateTime.FromBinary((long)(dictionary["StartedOn"]));
        TimeSpent = TimeSpan.FromSeconds((double)(dictionary["TimeSpent"]));
        SceneFileName = (string)dictionary["SceneFileName"];
        SceneDefaultMarker = (string)dictionary["SceneDefaultMarker"];
        Location = (string)dictionary["Location"];
        LocationVector = (Vector2)dictionary["LocationVector"];
        Stats.Health = (int)dictionary["Health"];
        Inventory = (Array<string>)dictionary["Inventory"];

        //Playthrough
        HasPlayedIntro = (bool)dictionary["HasPlayedIntro"];
        HasMessageFromShimble = (bool)dictionary["HasMessageFromShimble"];
        HasQuestFromShimble = (bool)dictionary["HasQuestFromShimble"];
        HasSolvedShimbleChair = (bool)dictionary["HasSolvedShimbleChair"];
        HasVisitedZenHouse = (bool)dictionary["HasVisitedZenHouse"];
        HasSolvedZenHouse = (bool)dictionary["HasSolvedZenHouse"];
    }

    public void AddToInventory(string item)
	{
		inventory.Add(item);
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
        magicSpells.Add(name);
    }

    public void RemoveMagicSpell(string name)
    {
        magicSpells.Remove(name);
    }
}