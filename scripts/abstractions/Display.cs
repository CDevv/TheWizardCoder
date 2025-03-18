using Godot;
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

        /// <summary>
        /// Show this display.
        /// </summary>
        public abstract void ShowDisplay();
        public virtual void UpdateDisplay()
        {

        }

        /// <summary>
        /// Hide this display.
        /// </summary>
        public virtual void HideDisplay()
        {
            Hide();
        }

        /// <summary>
        /// Add a subdisplay to this <c>Display</c>
        /// </summary>
        /// <param name="name">Friendly name for this subdisplay</param>
        /// <param name="subdisplay">The subdisplay object itself</param>
        public void AddSubdisplay(string name, Display subdisplay)
        {
            Subdisplays.Add(name, subdisplay);
        }

        /// <summary>
        /// Show a sibdisplay by its name
        /// </summary>
        /// <param name="name">Friendly name of the subdisplay</param>
        public void ShowSubdisplay(string name)
        {
            Subdisplays[name].ShowDisplay();
        }

        /// <summary>
        /// Hide all subdisplays that this <c>Display</c> contains
        /// </summary>
        public void HideAllSubdisplays()
        {
            foreach (KeyValuePair<string, Display> item in Subdisplays)
            {
                item.Value.Hide();
            }
        }

        /// <summary>
        /// Switch visibility to another subdisplay. After this operation only the chosen subdisplay will be shown.
        /// </summary>
        /// <param name="name"></param>
        public void ChangeSubdisplay(string name)
        {
            HideAllSubdisplays();
            ShowSubdisplay(name);
        }

        /// <summary>
        /// Call <c>UpdateDisplay()</c> for each subdisplay that this <c>Display</c> contains.
        /// </summary>
        public void UpdateAllSubdisplays()
        {
            foreach (KeyValuePair<string, Display> item in Subdisplays)
            {
                item.Value.UpdateDisplay();
            }
        }
    }
}