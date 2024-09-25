using System;
using Godot;
using Godot.Collections;
using TheWizardCoder.Enums;

namespace TheWizardCoder.Data
{
    public class SettingsConfig
    {
        public WindowSize WindowSize { get; set; }
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

            configFile.SetValue("General", "WindowSize", (int)WindowSize);
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

            WindowSize = (WindowSize)(int)configFile.GetValue("General", "WindowSize");
            Fullscreen = (bool)configFile.GetValue("General", "Fullscreen");

            foreach (var item in controls)
            {
                controls[item.Key] = (InputEvent)configFile.GetValue("Controls", item.Key);
            }
        }

        public void ApplySettings()
        {
            //DisplayServer.WindowSetSize(new Vector2I(WindowWidth, WindowHeight));
            ChangeWindowSize(WindowSize);

            ToggleFullscreen(Fullscreen);
            foreach (var item in controls)
            {
                ChangeControl(item.Key, item.Value);
            }
        }

        public void ChangeWindowSize(WindowSize size, Window window)
        {
            int windowHeight = 0;
            int windowWidth = 0;

            switch (size)
            {
                case WindowSize.Size640by480:
                    windowWidth = 640;
                    windowHeight = 480;        
                    break;
                case WindowSize.Size1280by960:
                    windowWidth = 1280;
                    windowHeight = 960;
                    break;
                default:
                    break;
            }

            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
            WindowSize = size;
            window.Size = new Vector2I(WindowWidth, WindowHeight);

        }

        public void ChangeWindowSize(WindowSize size)
        {
            int windowHeight = 0;
            int windowWidth = 0;

            switch (size)
            {
                case WindowSize.Size640by480:
                    windowWidth = 640;
                    windowHeight = 480;        
                    break;
                case WindowSize.Size1280by960:
                    windowWidth = 1280;
                    windowHeight = 960;
                    break;
                default:
                    break;
            }

            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
            WindowSize = size;
            DisplayServer.WindowSetSize(new Vector2I(WindowWidth, WindowHeight));
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
            InputMap.ActionEraseEvents(name);
            controls[name] = input;
            InputMap.ActionAddEvent(name, input);
        }
    }
}