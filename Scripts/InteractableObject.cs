using Godot;
using System;

public class InteractableObject : KinematicBody
{
    public override void _Ready()
    {
        GD.Print("Object ready");
        Connect("mouse_entered", this, "OnMouseEntered");
    }

    public virtual void OnMouseEntered()
    {
        // Override me!
    }

    public virtual void OnInteractedWith()
    {
        
    }
}
