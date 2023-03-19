using Godot;
using System;

public class InteractableObject : KinematicBody
{
    [Signal]
    public delegate void Interacted();
    
    public override void _Ready()
    {
        Connect("mouse_entered", this, "OnMouseEntered");
    }

    public virtual void OnMouseEntered(){}

    public virtual void OnInteractedWith()
    {
        GD.Print($"Button was interacted with of ${GetPath()}");
        EmitSignal(nameof(Interacted));
    }
}
