using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public class GrabbableObject : RigidBody
{
    private readonly float ROTATION_MOUSE_SCALE = 0.01f;
    private readonly float MOVEMENT_SCALE = 50f;
    private readonly float MOVEMENT_DISTANCE_THRESHOLD = 5f;
    
    private bool isGrabbed = false;
    private Vector3 targetPosition;
    private Camera camera; 
    private Vector2 mouseOffset = new Vector2(0f,0f);

    public override void _Ready()
    {
        camera = GetViewport().GetCamera();
    }

    public void Grab()
    {
        GD.Print("Hotdog Grabbed");
        isGrabbed = true;
    }

    public void UpdateTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    public void Drop()
    {
        GD.Print("Hotdog Dropped");
        isGrabbed = false; 
    }

    public override void _PhysicsProcess(float delta)
    {
        // TODO: Change this to a state
        if (isGrabbed)
        {
            // Move and slide, if distance is within range
            float targetDistance = GlobalTranslation.DistanceTo(targetPosition);
            if (targetDistance > MOVEMENT_DISTANCE_THRESHOLD)
            {
                SetAxisVelocity(GlobalTranslation.DirectionTo(targetPosition) * (targetDistance * delta * MOVEMENT_SCALE));
            }
            else
            {
                AngularVelocity = Vector3.Zero;
                LinearVelocity = Vector3.Zero;
            }

            ComputerScreen.UpdateBodyText(
                $"DIR: {GlobalTranslation.DirectionTo(targetPosition).ToString()}\n" +
                $"VEL: {LinearVelocity.ToString()}\n" +
                $"DIS: {targetDistance.ToString()}\n"
            );
            
            // Rotate the object with shift pressed
            if (BaseScene.GetPlayerState() == BaseScene.PlayerState.Inspecting)
            {
                // TODO: Convert this to generic position for gamepad support
                Vector2 mousePosition = (GetViewport().GetMousePosition() - mouseOffset);
                
                Transform transform = GlobalTransform;
                transform.basis = transform.basis.Rotated(
                    camera.Transform.basis[1], 
                    mousePosition.x * ROTATION_MOUSE_SCALE
                );
                
                transform.basis = transform.basis.Rotated(
                     camera.Transform.basis[2], 
                    mousePosition.y * ROTATION_MOUSE_SCALE
                );
                GlobalTransform = transform;

                mouseOffset = GetViewport().GetMousePosition();
            }
        }
    }
}