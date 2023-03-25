using Godot;
using System.Collections.Generic;
using Godot.Collections;

public class Rat : GrabbableObject
{
    private const float ANIMATION_SPEED_MOVE = 4f;
    private const float ANIMATION_SPEED_GRABBED = 10f;
    
    private const float ESCAPE_GRAB_TIME_MIN = 1.5f;
    private const float ESCAPE_GRAB_TIME_MAX = 4f;
    
    private const float WAIT_TIME_MIN = 1.5f;
    private const float WAIT_TIME_MAX = 4f; 
    
    private const float MOVEMENT_SPEED_SCALE = 20f;

    private const float CHANCE_MOVE_RANDOM = 0.5f;
    private const float CHANCE_WAIT = 0.5f; 
    
    private static List<Spatial> movementPointList;

    [Export] public AudioStream[] soundRatAlert;
    [Export] public AudioStream[] soundRatSqueak;

    private AnimationPlayer _animationPlayer;
    private Spatial _movementTarget;
    private Area _searchArea;

    private GrabbableObject _grabbedObject;

    private float _escapeTime;
    private float _waitTime;
    private Spatial _escapePoint; 
    private Spatial _originPoint;
    private Spatial _holdPoint; 
    private Spatial _debug; 

    public override void _Ready()
    {
        base._Ready();

        _debug = GetNode<Spatial>("Debug");
        _holdPoint = GetNode<Spatial>("HoldPoint");
        _escapePoint = GetTree().CurrentScene.GetNode<Spatial>("Points/RatEscapePoint");
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

        GD.Print("Rat: I SPAWNED");
        UpdateTarget(FindClosestMovementPoint());
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_movementTarget != null) _debug.GlobalTranslation = _movementTarget.GlobalTranslation;

        if (_waitTime > 0)
        {
            _waitTime -= delta; 
            if (_waitTime < 0)
            {
                _animationPlayer.Play("walk");
                if (GD.Randf() > CHANCE_MOVE_RANDOM)
                {
                    UpdateTarget(FindNearestObject());
                }
                else
                {
                    UpdateTarget(GD.Randf() > 0.5f ? FindClosestMovementPoint() : FindRandomMovementPoint());
                }
            }
        }

        if (_grabbedObject != null)
        {
            // TODO - change to the rats head
            _grabbedObject.UpdateTargetPosition(_holdPoint.GlobalTranslation);
        }
    }
    
    /**
     * Taken from https://docs.godotengine.org/en/3.5/tutorials/physics/rigid_body.html
     */
    private void LookFollow(PhysicsDirectBodyState state, Transform currentTransform, Vector3 targetPosition)
    {
        var upDir = new Vector3(0, 1, 0);
        var curDir = currentTransform.basis.Xform(new Vector3(0, 0, 1));
        var targetDir = (targetPosition - currentTransform.origin).Normalized();
        var rotationAngle = Mathf.Acos(curDir.x) - Mathf.Acos(targetDir.x);
        state.SetAngularVelocity(upDir * (rotationAngle / state.GetStep()));
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        if (_movementTarget != null && !isGrabbed)
        {
            LookFollow(state, GlobalTransform, _movementTarget.GlobalTransform.origin);
            SetAxisVelocity(GlobalTranslation.DirectionTo(_movementTarget.GlobalTranslation) * MOVEMENT_SPEED_SCALE);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        if (!isGrabbed)
        {
            // AngularVelocity = Vector3.Zero;
            LinearVelocity = Vector3.Zero;
            
            if (_movementTarget != null)
            {
                // Pick a new target, or sit and wait
                if (GlobalTranslation.DistanceTo(_movementTarget.GlobalTranslation) < 8f)
                {
                    // Return to the origin if this was the escape point. Skip other logic. 
                    if (_movementTarget == _escapePoint)
                    {
                        _movementTarget = _originPoint;
                        return;
                    }
                    
                    // Attempt to pick it up
                    if (_movementTarget is GrabbableObject grabbableObject)
                    {
                        GD.Print("Rat: HOTDOG TIME");
                        _animationPlayer.Play("walk");
                        grabbableObject.Grab(true);
                        _grabbedObject = grabbableObject;
                        _grabbedObject.ForcePosition(_holdPoint.GlobalTranslation, GlobalRotation);
                        Escape();
                    }
                    else
                    {
                        // Logic is weird since chance is inverted 
                        if (GD.Randf() > CHANCE_MOVE_RANDOM)
                        {
                            if (IsGrabbing())
                            {
                                Escape();
                            }
                            else
                            {
                                UpdateTarget(FindNearestObject());
                            }
                        }
                        else if (GD.Randf() > CHANCE_WAIT)
                        {
                            GD.Print("Rat: I MOVE RANDOMLY");
                            _animationPlayer.Play("walk");
                            UpdateTarget(FindRandomMovementPoint());
                        }
                        else
                        {
                            _animationPlayer.Play("sit");
                            _movementTarget = null;
                            _waitTime = Mathf.Lerp(WAIT_TIME_MIN, WAIT_TIME_MAX, GD.Randf());
                        }
                    }
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

    public override void Grab(bool disableCollision = false)
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

    public bool IsGrabbing()
    {
        return _grabbedObject != null; 
    }

    /**
     * Despawn self and object
     */
    public void Despawn()
    {
        if (_grabbedObject != null) _grabbedObject.QueueFree();
        QueueFree();
    }

    private void UpdateTarget(Spatial newTarget)
    {
        // LookAt(-newTarget.GlobalTranslation, Vector3.Up);
        _movementTarget = newTarget;
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
            UpdateTarget(FindRandomMovementPoint());
        }
    }

    private void Escape()
    {
        GD.Print("Rat: TIME TO ESCAPE");
        // Seek the origin point
        UpdateTarget(_escapePoint);
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