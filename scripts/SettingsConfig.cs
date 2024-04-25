using System;
using Godot;
using Godot.Collections;

public class SettingsConfig
{
    public int WindowWidth { get; set; } = 640;
    public int WindowHeight { get; set; } = 480;
    public bool Fullscreen { get; set; } = false;

    private Dictionary<string, InputEvent> controls = new()
    {
        {"left", new InputEventKey(){Keycode = Key.Left, PhysicalKeycode = Key.Left}},
        {"right", new InputEventKey(){Keycode = Key.Right, PhysicalKeycode = Key.Right}},
        {"down", new InputEventKey(){Keycode = Key.Down, PhysicalKeycode = Key.Down}},
        {"up", new InputEventKey(){Keycode = Key.Up, PhysicalKeycode = Key.Up}}
    };

    public Dictionary<string, InputEvent> Controls 
    {
        get 
        {
            return controls;
        }
    }

    public void SaveSettings()
    {
        ConfigFile configFile = new ConfigFile();

        configFile.SetValue("General", "WindowWidth", WindowWidth);
        configFile.SetValue("General", "WindowHeight", WindowHeight);
        configFile.SetValue("General", "Fullscreen", Fullscreen);

        foreach (var item in controls)
        {
            configFile.SetValue("Controls", item.Key, item.Value);
        }

        configFile.Save("user://options.cfg");
    }

    public void LoadSettings()
    {
        ConfigFile configFile = new ConfigFile();

        Error err = configFile.Load("user://options.cfg");

        if (err != Error.Ok)
        {
            return;
        }

        WindowWidth = (int)configFile.GetValue("General", "WindowWidth");
        WindowHeight = (int)configFile.GetValue("General", "WindowHeight");
        Fullscreen = (bool)configFile.GetValue("General", "Fullscreen");

        foreach (var item in controls)
        {
            controls[item.Key] = (InputEvent)configFile.GetValue("Controls", item.Key);
        }
    }

    public void ApplySettings()
    {
        DisplayServer.WindowSetSize(new Vector2I(WindowWidth, WindowHeight));
        ToggleFullscreen(Fullscreen);
        foreach (var item in controls)
        {
            ChangeControl(item.Key, item.Value);
        }
    }

    public void ChangeWindowSize(WindowSize size, Window window)
    {
        switch (size)
        {
            case WindowSize.Size640by480:
                WindowWidth = 640;
                WindowHeight = 480;        
                break;
            case WindowSize.Size1280by960:
                WindowWidth = 1280;
                WindowHeight = 960;
                break;
            default:
                break;
        }
        window.Size = new Vector2I(WindowWidth, WindowHeight);
    }

    public void ToggleFullscreen(bool value)
    {
        Fullscreen = value;
        if (value)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }   
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
    }

    public void ChangeControl(string name, InputEvent input)
    {
        InputMap.ActionEraseEvent(name, controls[name]);
        controls[name] = input;
        InputMap.ActionAddEvent(name, input);
    }
}