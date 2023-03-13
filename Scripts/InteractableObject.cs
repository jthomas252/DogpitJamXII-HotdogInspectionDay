using Godot;
using System;

public class InteractableObject : RigidBody
{
    public override void _Ready()
    {
        GD.Print("Object ready");

        Connect("mouse_entered", this, "OnMouseEntered");
    }
    
    public void OnMouseEntered()
    {
        GD.Print("??");
    }
}
