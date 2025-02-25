using Godot;
using System;
using System.Collections.Generic;
using TheWizardCoder.Autoload;

namespace TheWizardCoder.Abstractions
{
    /// <summary>
    /// Base class for all Displays and Subdisplays. A Display contains UI elements like buttons and labels.
    /// </summary>
    public abstract partial class Display : CanvasLayer
    {
        protected Global global;
        public Dictionary<string, Display> Subdisplays { get; private set; } = new();

        public override void _Ready()
        {
            global = GetNode<Global>("/root/Global");
        }

        public abstract void ShowDisplay();
        public virtual void UpdateDisplay()
        {
            
        }
        
        public virtual void HideDisplay()
        {
            Hide();
        }

        public void AddSubdisplay(string name, Display subdisplay)
        {
            Subdisplays.Add(name, subdisplay);
        }

        public void ShowSubdisplay(string name)
        {
            Subdisplays[name].ShowDisplay();
        }

        public void HideAllSubdisplays()
        {
            foreach (var item in Subdisplays)
            {
                item.Value.Hide();
            }
        }

        public void ChangeSubdisplay(string name)
        {
            HideAllSubdisplays();
            ShowSubdisplay(name);
        }

        public void UpdateAllSubdisplays()
        {
            foreach (var item in Subdisplays)
            {
                item.Value.UpdateDisplay();
            }
        }
    }
}