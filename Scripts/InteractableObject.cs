using Godot;
using System;

public class InteractableObject : KinematicBody
{
    private ShaderMaterial material;

    public override void _Ready()
    {
        GD.Print("Object ready");
        Connect("mouse_entered", this, "OnMouseEntered");

        MeshInstance mesh = GetNode<MeshInstance>("MeshInstance");
        mesh.GetActiveMaterial(0);
    }

    public virtual void OnMouseEntered()
    {
        // Override me!
    }

    public virtual void OnInteractedWith()
    {
        
    }
}
