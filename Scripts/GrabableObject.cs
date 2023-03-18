using Godot;
using System;

public class GrabableObject : KinematicBody
{
    private readonly float MOVE_DISTANCE_THRESHOLD = 1f;
    private readonly float MOVEMENT_DISTANCE_SCALE = 10f;

    private bool isGrabbed = false;
    private Vector3 targetPosition;
    private Vector3 targetDirection;

    private Skeleton skeleton;
    private PhysicalBone bone;

    private Vector2 mouseOffset = new Vector2(0f,0f);
    
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

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouse inputEventMouse)
        {
            if (inputEventMouse.ButtonMask == (int)ButtonList.Left)
            {
                mouseOffset = inputEventMouse.Position; 
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (isGrabbed)
        {
            // Move and slide, if distance is within range
            float targetDistance = GlobalTranslation.DistanceTo(targetPosition);
            if (targetDistance > MOVE_DISTANCE_THRESHOLD)
            {
                targetDirection = targetPosition - GlobalTranslation;
                MoveAndSlide(targetDirection * MOVEMENT_DISTANCE_SCALE, Rotation);
            }
            else
            {
                targetDirection = targetPosition - GlobalTranslation;
                MoveAndSlide(Vector3.Zero, targetDirection);
            }
            
            // Rotate the object with shift pressed
            if (Input.IsKeyPressed((int)KeyList.Shift))
            {
                Vector2 mousePos = GetViewport().GetMousePosition() - mouseOffset;
                mouseOffset = GetViewport().GetMousePosition();

                RotateX(mousePos.x * 0.25f * delta);
                if (Input.IsKeyPressed((int)KeyList.Control))
                {
                    RotateY(mousePos.y * 0.25f * delta);
                }
                else
                {
                    RotateZ(mousePos.y * 0.25f * delta);
                }
            }
            
            
        }
        else
        {
            MoveAndSlide(Vector3.Zero, Vector3.Up);
            GlobalTranslation = bone.GlobalTranslation;
            GlobalRotation = bone.GlobalRotation;
        }
    }
}