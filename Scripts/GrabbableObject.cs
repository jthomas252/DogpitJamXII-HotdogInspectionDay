using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public class GrabbableObject : RigidBody
{
    private readonly float ROTATION_MOUSE_SCALE = 0.01f;
    
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
        Sleeping = true;
    }

    public void UpdateTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    public void Drop()
    {
        GD.Print("Hotdog Dropped");
        isGrabbed = false; 
        Sleeping = false;
    }

    public override void _PhysicsProcess(float delta)
    {
        // TODO: Change this to a state
        if (isGrabbed)
        {
            // Move and slide, if distance is within range
            float targetDistance = GlobalTranslation.DistanceTo(targetPosition);
            
            // Just move the object to place
            GlobalTranslation = targetPosition;

            // TODO: Optionally try and move the rigidbody to fly towards the target position
            
            // Rotate the object with shift pressed
            if (BaseScene.currentState == BaseScene.PlayerState.Inspecting)
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