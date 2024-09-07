using Godot;
using System;

public abstract partial class Display : CanvasLayer
{
    protected Global global;

    public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");
	}

    public abstract void ShowDisplay();
    public abstract void UpdateDisplay();
    public virtual void HideDisplay()
    {
        Hide();
    }
}