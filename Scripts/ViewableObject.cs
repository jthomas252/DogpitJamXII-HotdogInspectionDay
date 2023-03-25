using Godot;
using System;

public class ViewableObject : KinematicBody
{
    private Vector3 _originalPoint;
    private Vector3 _originalRotation;
    private Spatial _inspectPoint;
    private Sprite3D _sprite;
    protected bool _inspecting;
    
    public override void _Ready()
    {
        _originalPoint = GlobalTranslation;
        _originalRotation = GlobalRotation;
        _inspectPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/InspectPointDocument");
        _sprite = GetNode<Sprite3D>("Sprite3D");
    }

    public virtual void Inspect()
    {
        if (_inspecting) return;
        GD.Print($"Document grabbed {Name}");
        
        GlobalTranslation = _inspectPoint.GlobalTranslation;
        GlobalRotation = _inspectPoint.GlobalRotation;

        _sprite.Billboard = SpatialMaterial.BillboardMode.Enabled;
        _inspecting = true;
    }

    public virtual void Drop()
    {
        if (!_inspecting) return;
        
        GD.Print($"Document dropped {Name}");
        
        GlobalTranslation = _originalPoint;
        GlobalRotation = _originalRotation;

        _sprite.Billboard = SpatialMaterial.BillboardMode.Disabled;
        _inspecting = false;
    }
}
