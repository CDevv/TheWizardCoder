using Godot;
using System;
using TheWizardCoder.Autoload;
using TheWizardCoder.Components;

namespace TheWizardCoder.Abstractions
{
	public abstract partial class Interactable : Area2D
	{
		protected Global global;

		[Export]
		public bool Active { get; set; } = true;

		[Export]
		public bool OnCollision { get; set; } = false;

		public override void _Ready()
		{
			global = GetNode<Global>("/root/Global");
		}

		public void Interact()
		{
			if (Active)
			{
				Action();
			}
			else
			{
				OnNotActive();
			}
		}

		public abstract void Action();

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

		public virtual void OnNotActive() {}
	}
}