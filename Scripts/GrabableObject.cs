using Godot;
using System;

public class GrabableObject : KinematicBody
{
    private readonly float MOVE_DISTANCE_THRESHOLD = 1f;
    private readonly float MOVEMENT_DISTANCE_SCALE = 30.25f;

    private bool isGrabbed = false;
    private Vector3 targetPosition;
    private Vector3 targetDirection;

    private Skeleton skeleton;
    private PhysicalBone bone;
    
    public override void _Ready()
    {
        base._Ready();
        skeleton = GetNode<Skeleton>("Skeleton"); 
        bone = skeleton.GetNode<PhysicalBone>("TopBone");
        skeleton.PhysicalBonesStartSimulation();
    }

    public void Grab()
    {
        isGrabbed = true;
        skeleton.PhysicalBonesStopSimulation();
    }

    public void UpdateTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    public void Drop()
    {
        skeleton.PhysicalBonesStartSimulation();
        isGrabbed = false; 
    }

    public override void _PhysicsProcess(float delta)
    {
        // Grab 
        if (isGrabbed)
        {
            // Move and slide, if distance is within range
            float targetDistance = GlobalTranslation.DistanceTo(targetPosition);
            if (targetDistance > MOVE_DISTANCE_THRESHOLD)
            {
                targetDirection = targetPosition - GlobalTranslation;
                Rotation = targetDirection;
                
                if (Input.IsKeyPressed((int)KeyList.Shift))
                {
                    MoveAndSlide(Vector3.Zero, targetDirection);
                }
                else
                {
                    MoveAndSlide(targetDirection * MOVEMENT_DISTANCE_SCALE, targetDirection);
                }
            }
            else
            {
                targetDirection = targetPosition - GlobalTranslation;
                MoveAndSlide(Vector3.Zero, targetDirection);
            }
        }
        // Release
        else
        {
            MoveAndSlide(Vector3.Zero, Vector3.Up);
            GlobalTranslation = bone.GlobalTranslation;
            GlobalRotation = bone.GlobalRotation;
        }
    }
}