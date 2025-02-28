using Godot;
using System;
using TheWizardCoder.Autoload;
using TheWizardCoder.Components;

namespace TheWizardCoder.Abstractions
{
	/// <summary>
	/// Base class for interactables. An interactable is what the player can interact with.
	/// </summary>
	public partial class Interactable : Area2D
	{
		[Signal]
		public delegate void InteractedEventHandler();

		protected Global global;

        /// <value>Whether or not the <c>Interactable</c> can be interacted with.</value>
        [Export]
		public bool Active { get; set; } = true;

		/// <value>Whether this <c>Interactable</c> activates when the <c>Player</c> collides with it.</value>
		[Export]
		public bool OnCollision { get; set; } = false;

		public override void _Ready()
		{
			global = GetNode<Global>("/root/Global");
		}

		/// <summary>
		/// Method to be called when the <c>Player</c> prompts to interact.
		/// </summary>
		public void Interact()
		{
			if (Active)
			{
				Action();
				EmitSignal(SignalName.Interacted);
			}
			else
			{
				OnNotActive();
			}
		}

		/// <summary>
		/// Method to be called when the <c>Interactable</c> is <see cref="Interactable.Active"/>
		/// </summary>
		public virtual void Action() {}

		private void OnBodyEntered(Node2D node)
		{
			if (Active && OnCollision)
			{
				if (node.GetType() == typeof(Player))
				{
					Action();
				}
			}
		}

		/// <summary>
		/// Method to be called when the <c>Interactable</c> is not <see cref="Interactable.Active"/>
		/// </summary>
		public virtual void OnNotActive()
		{
			global.CanWalk = true;
		}
	}
}
