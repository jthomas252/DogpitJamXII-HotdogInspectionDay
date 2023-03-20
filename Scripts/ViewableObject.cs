using Godot;
using System;

public class ViewableObject : KinematicBody
{
    private readonly float ROTATION_MOUSE_SCALE = 0.01f;

    private Vector3 _originalPoint;
    private Vector3 _originalRotation; 
    
    private Vector3 _inspectPoint;

    private Vector2 mouseOffset;
    private Camera camera;

    private bool isGrabbed = false;
    private Sprite3D _sprite;
    
    public override void _Ready()
    {
        camera = GetViewport().GetCamera();
        _originalPoint = GlobalTranslation;
        _originalRotation = GlobalRotation;
        _inspectPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/InspectPointDocument").GlobalTranslation;

        _sprite = GetNode<Sprite3D>("Sprite3D");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        // if (isGrabbed)
        // {
        //     // TODO: Convert this to generic position for gamepad support
        //     Vector2 mousePosition = (GetViewport().GetMousePosition() - mouseOffset);
        //
        //     Transform transform = GlobalTransform;
        //     transform.basis = transform.basis.Rotated(
        //         Input.IsKeyPressed((int)KeyList.Alt) ? camera.Transform.basis[0] : camera.Transform.basis[1],
        //         mousePosition.x * ROTATION_MOUSE_SCALE
        //     );
        //
        //     transform.basis = transform.basis.Rotated(
        //         camera.Transform.basis[2],
        //         mousePosition.y * ROTATION_MOUSE_SCALE
        //     );
        //     GlobalTransform = transform;
        //
        //     mouseOffset = GetViewport().GetMousePosition();
        // }
    }

    public void Inspect()
    {
        GD.Print("Document grabbed");
        isGrabbed = true; 
        
        GlobalTranslation = _inspectPoint;
        GlobalRotation = GlobalTranslation.DirectionTo(camera.GlobalTranslation);

        _sprite.Billboard = SpatialMaterial.BillboardMode.Enabled;
        _sprite.Shaded = false; 
    }

    public void Drop()
    {
        GD.Print("Document dropped");
        isGrabbed = false; 
        
        GlobalTranslation = _originalPoint;
        GlobalRotation = _originalRotation;

        _sprite.Billboard = SpatialMaterial.BillboardMode.Disabled;
        _sprite.Shaded = true;         
    }
}
