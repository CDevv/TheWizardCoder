using System;

public class SaveFileData
{
    public string SaveName { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public string Location { get; set; }

    public bool IsSaveEmpty { get; set; }
}