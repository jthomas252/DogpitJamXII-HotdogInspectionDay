using Godot;
using System;

public class GrabableObject : KinematicBody
{
    [Export]
    private readonly float MOVE_DISTANCE_THRESHOLD = 2f;
    [Export]
    private readonly Vector3 MOVEMENT_DISTANCE_SCALE = new Vector3(2000f, 2000f, 2000f);

    private bool isGrabbed = false;
    private Vector3 targetPosition;
    private Vector3 targetDirection;

    private Skeleton skeleton; 

    public override void _Ready()
    {
        base._Ready();
        skeleton = GetNode<Skeleton>("Skeleton"); 
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

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (Input.IsKeyPressed((int)KeyList.P))
        {
            GlobalTranslation = new Vector3(0f, 10f, 15f);
        }
        
        if (Input.IsKeyPressed((int)KeyList.Key9))
        {
            GD.Print("StartSim");
            skeleton.PhysicalBonesStartSimulation();
        }  
        
        if (Input.IsKeyPressed((int)KeyList.Key0))
        {
            GD.Print("StopSim");
            skeleton.PhysicalBonesStopSimulation();
        }           
    }

    public static string debugInfo = "";
    
    public override void _PhysicsProcess(float delta)
    {
        
        if (isGrabbed)
        {
            // Move and slide, if distance is within range
            float targetDistance = GlobalTranslation.DistanceTo(targetPosition);
            if (targetDistance > MOVE_DISTANCE_THRESHOLD * delta)
            {
                targetDirection = targetPosition - GlobalTranslation;
                MoveAndSlide(targetDirection, targetDirection);
                Rotation = GlobalTranslation.DirectionTo(targetPosition);
            }
            else
            {
                MoveAndSlide(Vector3.Zero, targetDirection);
            }
            
            if (Input.IsKeyPressed((int)KeyList.Key1))
            {
                GD.Print($"${targetPosition} ${targetDirection} ${GlobalTranslation}");
            }
        }
        else
        {
            MoveAndSlide(Vector3.Zero, Vector3.Up);
        }
    }
}