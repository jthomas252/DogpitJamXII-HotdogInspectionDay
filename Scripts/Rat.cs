using Godot;
using System.Collections.Generic;
using Godot.Collections;

public class Rat : GrabbableObject
{
    private const float ANIMATION_SPEED_MOVE = 2f;
    private const float ANIMATION_SPEED_GRABBED = 10f;
    
    private const float ESCAPE_GRAB_TIME_MIN = 1.5f;
    private const float ESCAPE_GRAB_TIME_MAX = 4f;
    
    private const float WAIT_TIME_MIN = 3.5f;
    private const float WAIT_TIME_MAX = 6f; 
    
    private const float MOVEMENT_SPEED_SCALE = 18f;

    private const float CHANCE_MOVE_RANDOM = 0.2f;
    private const float CHANCE_WAIT = 0.1f; 
    
    private static List<Spatial> movementPointList;

    [Export] public AudioStream[] soundRatAlert;
    [Export] public AudioStream[] soundRatSqueak;

    private AnimationPlayer _animationPlayer;
    private Spatial _movementTarget;
    private Area _searchArea;

    private GrabbableObject _grabbedObject;

    private float _escapeTime;
    private float _waitTime;
    private Spatial _originPoint;

    public override void _Ready()
    {
        base._Ready();

        _originPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/RatSpawnPoint");
        _searchArea = GetNode<Area>("SearchArea");
        
        _animationPlayer = GetNode<AnimationPlayer>("RootNode/AnimationPlayer");
        _animationPlayer.GetAnimation("walk").Loop = true;
        _animationPlayer.PlaybackSpeed = ANIMATION_SPEED_MOVE;
        _animationPlayer.Play("walk");

        // If this is the first rat, set up the movement point list.
        if (movementPointList is null)
        {
            var pointList = GetTree().CurrentScene.GetNode("RatPointList").GetChildren();
            movementPointList = new List<Spatial>();
            foreach (Spatial point in pointList) movementPointList.Add(point);
        }

        _movementTarget = FindNearestObject();

        GD.Print("Rat: I SPAWNED");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_waitTime > 0)
        {
            _waitTime -= delta; 
            if (_waitTime < 0)
            {
                _animationPlayer.Play("walk");
                if (GD.Randf() > CHANCE_MOVE_RANDOM)
                {
                    _movementTarget = FindNearestObject();
                }
                else
                {
                    _movementTarget = GD.Randf() > 0.5f ? FindClosestMovementPoint() : FindRandomMovementPoint();
                }
            }
        }

        if (_grabbedObject != null)
        {
            // TODO - change to the rats head
            _grabbedObject.UpdateTargetPosition(GlobalTranslation);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        if (!isGrabbed)
        {
            AngularVelocity = Vector3.Zero;
            LinearVelocity = Vector3.Zero;
            
            if (_movementTarget != null)
            {
                // Pick a new target, or sit and wait
                if (GlobalTranslation.DistanceTo(_movementTarget.GlobalTranslation) < 1f)
                {
                    GD.Print("Target threshold...");
                    
                    // Attempt to pick it up
                    if (_movementTarget is GrabbableObject grabbableObject)
                    {
                        GD.Print("Rat: HOTDOG TIME");
                        _animationPlayer.Play("walk");
                        grabbableObject.Grab();
                        _grabbedObject = grabbableObject; 
                        Escape();
                    }
                    else
                    {
                        // Logic is weird since chance is inverted 
                        if (GD.Randf() > CHANCE_MOVE_RANDOM)
                        {
                            _movementTarget = FindNearestObject();
                        }
                        else if (GD.Randf() > CHANCE_WAIT)
                        {
                            GD.Print("Rat: I MOVE RANDOMLY");
                            _animationPlayer.Play("walk");
                            _movementTarget = FindRandomMovementPoint();
                        }
                        else
                        {
                            _animationPlayer.Play("sit");
                            _movementTarget = null;
                            _waitTime = Mathf.Lerp(WAIT_TIME_MIN, WAIT_TIME_MAX, GD.Randf());
                        }
                    }
                }
                else
                {
                    SetAxisVelocity(GlobalTranslation.DirectionTo(_movementTarget.GlobalTranslation) * MOVEMENT_SPEED_SCALE);
                }
            }
        } 
        else if (!BaseScene.Inspecting())
        {
            // Struggle to release itself, if not in inspector mode
            _escapeTime -= delta;
            if (_escapeTime < 0f)
            {
                Drop();
                Cursor.ForceReleaseObject();
            }
        }
    }

    public override void Grab()
    {
        base.Grab();
        _animationPlayer.PlaybackSpeed = ANIMATION_SPEED_GRABBED;
        _escapeTime = Mathf.Lerp(ESCAPE_GRAB_TIME_MIN, ESCAPE_GRAB_TIME_MAX, GD.Randf());
    }

    public override void Drop()
    {
        base.Drop();
        _animationPlayer.PlaybackSpeed = ANIMATION_SPEED_MOVE; 
    }

    /**
     * Use when the mouse enters the search area, causes the rat to scurry to its next point 
     */
    public void Alert()
    {
        GD.Print("Rat: RUN AWAY!!!");
        _waitTime = 0f;
        if (_grabbedObject != null)
        {
            _movementTarget = FindRandomMovementPoint();
        }
    }

    private void Escape()
    {
        GD.Print("Rat: TIME TO ESCAPE");
        // Seek the origin point
        _movementTarget = _originPoint;
        _waitTime = 0; 
    }

    private Spatial FindClosestMovementPoint()
    {
        Spatial sendBack = null; 
        float nearestDist = 0f; 
        foreach (Spatial point in movementPointList)
        {
            if (point == _movementTarget) continue;

            float dist = GlobalTranslation.DistanceTo(point.GlobalTranslation);
            if (nearestDist == 0f || dist < nearestDist)
            {
                nearestDist = dist; 
                sendBack = point;
            } 
        }
        return sendBack;
    }

    private Spatial FindRandomMovementPoint()
    {
        // Avoid problems where we could get negatives from GD.Randi()
        return movementPointList[Mathf.Abs((int)GD.Randi()) % movementPointList.Count];
    }

    private Spatial FindNearestObject()
    {
        Array bodies = _searchArea.GetOverlappingBodies();

        foreach (var body in bodies)
        {
            if (body is Hotdog dog)
            {
                return dog; 
            }
        }
        
        // If we can't find a hotdog then go to a random target instead.
        return FindRandomMovementPoint();
    }
}