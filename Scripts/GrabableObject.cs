using Godot;
using System;
using System.Numerics;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public class GrabableObject : RigidBody
{
    private readonly float MOVE_DISTANCE_THRESHOLD = 5f;
    private readonly float MOVEMENT_DISTANCE_SCALE = 1f;
    private readonly float ROTATION_MOUSE_SCALE = 0.01f;

    private bool isGrabbed = false;
    private Vector3 targetPosition;
    private Vector3 targetDirection;

    private Skeleton skeleton;
    private PhysicalBone bone;

    private Vector2 mouseOffset = new Vector2(0f,0f);

    // public override void _Ready()
    // {
    //     base._Ready();
    //     skeleton = GetNodeOrNull<Skeleton>("Skeleton");
    //     bone = skeleton.GetNodeOrNull<PhysicalBone>("Physical Bone topdog");
    //     skeleton.PhysicalBonesStartSimulation();
    // }

    public void Grab()
    {
        // skeleton.PhysicalBonesStopSimulation();

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
        // skeleton.PhysicalBonesStartSimulation();
        
        GD.Print("Hotdog Dropped");
        isGrabbed = false; 
        Sleeping = false;
    }

    // public override void _UnhandledInput(InputEvent @event)
    // {
    //     if (@event is InputEventMouse inputEventMouse)
    //     {
    //         if (inputEventMouse.ButtonMask == (int)ButtonList.Left)
    //         {
    //             mouseOffset = inputEventMouse.Position; 
    //         }
    //     }
    // }

    public override void _PhysicsProcess(float delta)
    {
        
        if (isGrabbed)
        {
            // Move and slide, if distance is within range
            float targetDistance = GlobalTranslation.DistanceTo(targetPosition);
            
            GlobalTranslation = targetPosition;
            
            // if (targetDistance > MOVE_DISTANCE_THRESHOLD)
            // {
            //     targetDirection = targetPosition - GlobalTranslation;
            //     // MoveAndSlide(targetDirection * MOVEMENT_DISTANCE_SCALE, Rotation);
            //     SetAxisVelocity(targetDirection * MOVEMENT_DISTANCE_SCALE);
            // }
            // else
            // {
            //     targetDirection = targetPosition - GlobalTranslation;
            //     // MoveAndSlide(Vector3.Zero, targetDirection);
            //     SetAxisVelocity(Vector3.Zero);
            // }
            
            // Rotate the object with shift pressed
            if (Cursor.inInspectionMode)
            {
                // TODO: Move this elsewhere, shouldn't need it every frame
                Camera c = GetViewport().GetCamera();
                
                Vector2 mousePosition = (GetViewport().GetMousePosition() - mouseOffset);
                
                Transform transform = GlobalTransform;
                transform.basis = transform.basis.Rotated(
                    Input.IsKeyPressed((int)KeyList.Alt) ? c.Transform.basis[0] : c.Transform.basis[1], 
                    mousePosition.x * ROTATION_MOUSE_SCALE
                );
                
                transform.basis = transform.basis.Rotated(
                     c.Transform.basis[2], 
                    mousePosition.y * ROTATION_MOUSE_SCALE
                );
                GlobalTransform = transform;

                mouseOffset = GetViewport().GetMousePosition();
            }
        }
        
        // Logic if we go back to skeletons, keep the kinematic body lined up with bone. 
        // else
        // {
        //     MoveAndSlide(Vector3.Zero, Vector3.Up);
        //     GlobalTranslation = bone.GlobalTranslation;
        //     GlobalRotation = bone.GlobalRotation;
        // }
    }
}