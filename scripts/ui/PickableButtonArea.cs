using Godot;
using System;

namespace TheWizardCoder.UI
{
	public partial class PickableButtonArea : Area2D
	{
		[Signal]
		public delegate void ButtonAddedEventHandler(string buttonText);
		[Signal]
		public delegate void ButtonRemovedEventHandler();

		public string ButtonText { get; set; }
		public bool Taken { get; set; } = false;
	}
}