using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public class GrabbableObject : RigidBody
{
    private readonly float ROTATION_MOUSE_SCALE = 0.01f;
    private readonly float ROTATION_KEYBOARD_SCALE = 2.2f;
    private readonly float MOVEMENT_SCALE = 200f;
    private readonly float MOVEMENT_DISTANCE_THRESHOLD = 1f;
    
    private bool isGrabbed = false;
    private Vector3 targetPosition;
    private Vector3 targetDirection;
    private Camera camera;
    private Vector2 mouseOffset;
    private Vector2 rotOffset;
    private Spatial _inspectPoint; 
    
    public override void _Ready()
    {
        camera = GetViewport().GetCamera();

        if (GetTree().CurrentScene is BaseScene baseScene)
        {
            baseScene.Connect("Inspection", this, nameof(OnInspection));
            baseScene.Connect("Grabbed", this, nameof(InspectionReset));
            baseScene.Connect("Normal", this, nameof(InspectionReset));
        }
        else
        {
            GD.PrintErr("BaseScene not present on root.");
        }
        
        _inspectPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/InspectPointDocument");
    }

    private void OnInspection()
    {
        if (isGrabbed)
        {
            GlobalTranslation = _inspectPoint.GlobalTranslation;
            GlobalRotation = _inspectPoint.GlobalRotation;
            Sleeping = true;
            mouseOffset = GetViewport().GetMousePosition();
            rotOffset = new Vector2(0f, 0f);
        }
    }

    private void InspectionReset()
    {
        if (isGrabbed)
        {
            Sleeping = false;
        }
    }

    public void Grab()
    {
        GD.Print("Hotdog Grabbed");
        isGrabbed = true;
    }

    public void UpdateTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        targetDirection = GlobalTranslation.DirectionTo(targetPosition);
    }

    public void Drop()
    {
        GD.Print("Hotdog dropped");
        isGrabbed = false;
        Sleeping = false;
    }

    public override void _PhysicsProcess(float delta)
    {
        // TODO: Change this to a state
        if (isGrabbed)
        {
            // Rotate the object with shift pressed
            if (BaseScene.GetPlayerState() == BaseScene.PlayerState.Inspecting)
            {
                // Always lock to the inspection position
                GlobalTranslation = targetPosition;

                // TODO: Convert this to generic position for gamepad support
                Vector2 mousePosition = (GetViewport().GetMousePosition() - mouseOffset);
                Vector2 keyboardInput = new Vector2(
                    Input.IsKeyPressed((int)KeyList.A) ? -ROTATION_KEYBOARD_SCALE : (Input.IsKeyPressed((int)KeyList.D) ? ROTATION_KEYBOARD_SCALE : 0f),
                    Input.IsKeyPressed((int)KeyList.W) ? -ROTATION_KEYBOARD_SCALE : (Input.IsKeyPressed((int)KeyList.S) ? ROTATION_KEYBOARD_SCALE : 0f)
                );

                Transform transform = GlobalTransform;

                transform.basis = transform.basis.Rotated(
                    camera.Transform.basis.y,
                    (mousePosition.x + keyboardInput.x) * ROTATION_MOUSE_SCALE
                );

                GlobalTransform = transform;

                mouseOffset = GetViewport().GetMousePosition();
            }
            else
            {
                // Zero out the previous velocity to prevent issues
                AngularVelocity = Vector3.Zero;
                LinearVelocity = Vector3.Zero;
            
                // Move and slide, if distance is within range
                float targetDistance = GlobalTranslation.DistanceTo(targetPosition);
                if (targetDistance > MOVEMENT_DISTANCE_THRESHOLD)
                {
                    float speed = targetDistance * delta * MOVEMENT_SCALE;
                    SetAxisVelocity(targetDirection * speed);
                }
            }
        }
    }
}