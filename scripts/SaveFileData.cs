using System;
using Godot;
using Godot.Collections;

public class SaveFileData
{
    public bool IsSaveEmpty { get; set; }
    public string SaveName { get; set; }
	public DateTime LastSaved { get; set; }
	public DateTime StartedOn { get; set; }
	public TimeSpan TimeSpent { get; set; }
	public string Location { get; set; }
	public int Health { get; set; } = 100;
    private Array<string> inventory = new();

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

    //Playthrough properties
    public bool HasPlayedIntro{ get; set; } = false;

    public Dictionary<string, Variant> GenerateDictionary()
    {
        return new Dictionary<string, Variant>()
        {
            {"SaveName", SaveName},
            {"LastSaved", LastSaved.ToBinary()},
            {"StartedOn", StartedOn.ToBinary()},
            {"TimeSpent", TimeSpent.TotalSeconds},
            {"Location", Location},
            {"Health", Health},
            {"Inventory", Inventory},
            {"HasPlayedIntro", HasPlayedIntro},
        };
    }

    public void ApplyDictionary(Dictionary<string, Variant> dictionary)
    {
        SaveName = (string)dictionary["SaveName"];
        LastSaved = DateTime.FromBinary((long)(dictionary["LastSaved"]));
        StartedOn = DateTime.FromBinary((long)(dictionary["StartedOn"]));
        TimeSpent = TimeSpan.FromSeconds((double)(dictionary["TimeSpent"]));
        Location = (string)dictionary["Location"];
        Health = (int)dictionary["Health"];
        Inventory = (Array<string>)dictionary["Inventory"];

        //Playthrough
        HasPlayedIntro = (bool)dictionary["HasPlayedIntro"];
    }

    public void AddToInventory(string item)
	{
		inventory.Add(item);
	}

	public void RemoveFromInventory(string item)
	{
		inventory.Remove(item);
	}

	public bool HasItemInInventory(string item)
	{
		return inventory.Contains(item);
	}
}