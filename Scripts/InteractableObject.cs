using Godot;
using System;

public class InteractableObject : Spatial
{
    public override void _Ready()
    {
        GD.Print("Object ready");

        // Connect("mouse_entered", this, "OnMouseEntered");
    }
}
