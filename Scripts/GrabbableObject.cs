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
    
    protected bool isGrabbed;
    private Vector3 targetPosition;
    private Vector3 targetDirection;
    private Camera camera;
    private Vector2 mouseOffset;
    private Spatial _inspectPoint;

    private uint _originalCollisionLayer;
    private uint _originalCollisionMask; 
    
    public override void _Ready()
    {
        _originalCollisionLayer = CollisionLayer;
        _originalCollisionMask = CollisionMask; 
        
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
        }
    }

    private void InspectionReset()
    {
        if (isGrabbed)
        {
            Sleeping = false;
        }
    }

    public void UpdateTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        targetDirection = GlobalTranslation.DirectionTo(targetPosition);
    }

    public void ForcePosition(Vector3 newPosition, Vector3 newRotation)
    {
        GlobalTranslation = newPosition;
        GlobalRotation = newRotation; 
    }
    
    public virtual void Grab(bool disableCollisions = false)
    {
        GD.Print($"{Name} Grabbed");
        isGrabbed = true;

        if (disableCollisions)
        {
            CollisionMask = 0;
            CollisionLayer = 0; 
        }
    }

    public virtual void Drop()
    {
        GD.Print($"{Name} dropped");
        isGrabbed = false;
        Sleeping = false;

        CollisionMask = _originalCollisionMask;
        CollisionLayer = _originalCollisionLayer;
    }

    public override void _PhysicsProcess(float delta)
    {
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

                if (Mathf.Abs(mousePosition.x) > Mathf.Abs(mousePosition.y) | Mathf.Abs(keyboardInput.x) > Mathf.Abs(keyboardInput.y))
                {
                    transform.basis = transform.basis.Rotated(
                        camera.Transform.basis.y,
                        (mousePosition.x + keyboardInput.x) * ROTATION_MOUSE_SCALE
                    );
                }
                else
                {
                    transform.basis = transform.basis.Rotated(
                        camera.Transform.basis.x,
                        (mousePosition.y + keyboardInput.y) * ROTATION_MOUSE_SCALE
                    );
                }

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