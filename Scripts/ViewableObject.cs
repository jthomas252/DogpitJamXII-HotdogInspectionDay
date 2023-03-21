using Godot;
using System;

public class ViewableObject : KinematicBody
{
    private Vector3 _originalPoint;
    private Vector3 _originalRotation;
    private Spatial _inspectPoint;
    private Sprite3D _sprite;

    public override void _Ready()
    {
        _originalPoint = GlobalTranslation;
        _originalRotation = GlobalRotation;
        _inspectPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/InspectPointDocument");
        _sprite = GetNode<Sprite3D>("Sprite3D");
    }

    public void Inspect()
    {
        GD.Print("Document grabbed");
        
        GlobalTranslation = _inspectPoint.GlobalTranslation;
        GlobalRotation = _inspectPoint.GlobalRotation;

        _sprite.Billboard = SpatialMaterial.BillboardMode.Enabled;
    }

    public void Drop()
    {
        GD.Print("Document dropped");
        
        GlobalTranslation = _originalPoint;
        GlobalRotation = _originalRotation;

        _sprite.Billboard = SpatialMaterial.BillboardMode.Disabled;
    }
}
