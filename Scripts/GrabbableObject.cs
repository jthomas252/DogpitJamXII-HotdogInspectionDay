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
    private Vector2 mouseOffset = new Vector2(0f,0f);

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
    }

    private void OnInspection()
    {
        if (isGrabbed)
        {
            GlobalRotation = Vector3.Zero;
            
            Sleeping = true;
            mouseOffset = GetViewport().GetMousePosition();

            GD.Print(GlobalRotation);
            // Maybe capture previous position / rotation to easily return?
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
        ComputerScreen.FlashMessage("HOTDOG DROPPED");
        ComputerScreen.PlayErrorSound();
        GD.Print("Hotdog Dropped");
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
                    Input.IsKeyPressed((int)KeyList.Alt) ? camera.Transform.basis[0] : camera.Transform.basis[1], 
                    (mousePosition.x + keyboardInput.x) * ROTATION_MOUSE_SCALE
                );
                
                transform.basis = transform.basis.Rotated(
                     camera.Transform.basis[0], 
                    (mousePosition.y + keyboardInput.y) * ROTATION_MOUSE_SCALE
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