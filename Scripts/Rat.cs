using Godot;
using System.Collections.Generic;
using Godot.Collections;

public class Rat : GrabbableObject
{
    private const float ANIMATION_SPEED_MOVE = 4f;
    private const float ANIMATION_SPEED_GRABBED = 10f;

    private const float ESCAPE_GRAB_TIME_MIN = 1.5f;
    private const float ESCAPE_GRAB_TIME_MAX = 4f;

    private const float WAIT_TIME_MIN = 0.2f;
    private const float WAIT_TIME_MAX = 1.75f;

    private const float MOVEMENT_SPEED_SCALE = 32f;

    private const float CHANCE_GRAB_HOTDOG = 0.8f;
    private const float CHANCE_MOVE_RANDOM = 0.25f;

    private const float NOISE_CHANCE = 0.001f;
    private const float ALERT_DELAY = 3f;
    private const float MAX_PURSUIT_TIME = 10f;
    private const float STUN_TIME = 2.5f; 
    
    private static List<Spatial> movementPointList;

    [Export] public AudioStream[] soundRatAlert;
    [Export] public AudioStream[] soundRatSqueak;

    private AnimationPlayer _animationPlayer;
    private Spatial _movementTarget;
    private Area _searchArea;

    private GrabbableObject _grabbedObject;

    private float _escapeTime;
    private float _waitTime;
    private float _alertTime;
    private float _pursuitTime;
    private float _stunTime; 
    
    private Spatial _escapePoint;
    private Spatial _originPoint;
    private Spatial _holdPoint;
    private Spatial _debug;
    private AudioStreamPlayer3D _audioPlayer;

    private AudioStream GetAlertNoise()
    {
        return soundRatAlert[GD.Randi() % soundRatAlert.Length];
    }

    private AudioStream GetSqueakNoise()
    {
        return soundRatSqueak[GD.Randi() % soundRatSqueak.Length]; 
    }
    
    public override void _Ready()
    {
        base._Ready();

        _debug = GetNode<Spatial>("Debug");
        _holdPoint = GetNode<Spatial>("HoldPoint");
        _escapePoint = GetTree().CurrentScene.GetNode<Spatial>("Points/RatEscapePoint");
        _originPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/RatSpawnPoint");
        _audioPlayer = GetNode<AudioStreamPlayer3D>("Sound");
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

        _audioPlayer.Stream = GetAlertNoise();
        _audioPlayer.Play();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_alertTime > 0f) _alertTime -= delta;
        
        if (_stunTime > 0f) _stunTime -= delta;

        if (_pursuitTime > 0f)
        {
            _pursuitTime -= delta;
            if (_pursuitTime <= 0f)
            {
                UpdateTarget(FindRandomMovementPoint());
            }
        }
        
        if (IsInstanceValid(_movementTarget))
        {
            _debug.GlobalTranslation = _movementTarget.GlobalTranslation;
            if (GlobalTranslation.DistanceTo(_movementTarget.GlobalTranslation) > 100f)
            {
                GD.Print("Rat: Finding new target");
                _movementTarget = FindRandomMovementPoint();
            }
        }
        
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
                    UpdateTarget(FindRandomMovementPoint());
                }
            }
        }

        if (IsInstanceValid(_grabbedObject))
        {
            // TODO - change to the rats head
            _grabbedObject.UpdateTargetPosition(_holdPoint.GlobalTranslation);
        }

        if (GD.Randf() < NOISE_CHANCE)
        {
            _audioPlayer.Stream = GetSqueakNoise();
            _audioPlayer.Play();
        }
    }

    /**
     * Taken from https://docs.godotengine.org/en/3.5/tutorials/physics/rigid_body.html
     */
    private void LookFollow(PhysicsDirectBodyState state, Transform currentTransform, Vector3 targetPosition)
    {
        Vector3 upDir = Vector3.Up;
        Vector3 curDir = currentTransform.basis.Xform(new Vector3(0,0,-1));
        Vector3 targetDir = targetPosition.DirectionTo(currentTransform.origin);
        float rotationAngle = Mathf.Acos(curDir.x) - Mathf.Acos(targetDir.x);
        float rotationVelocity = rotationAngle / state.GetStep();
        
        state.SetAngularVelocity(upDir * rotationVelocity);
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        if (IsInstanceValid(_movementTarget) && !isGrabbed && IsOnGround() && _stunTime <= 0f)
        {
            LookFollow(state, GlobalTransform, _movementTarget.GlobalTransform.origin);
            SetAxisVelocity(GlobalTranslation.DirectionTo(_movementTarget.GlobalTranslation) * MOVEMENT_SPEED_SCALE);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (IsInstanceValid(_grabbedObject))
        {
            _grabbedObject.ForcePosition(_holdPoint.GlobalTranslation, _holdPoint.GlobalRotation);
        }
        
        if (!isGrabbed && IsOnGround() && IsInstanceValid(_movementTarget) && _stunTime <= 0f)
        {
            LinearVelocity = Vector3.Zero;

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
                    Cursor.ForceReleaseObject(grabbableObject);
                    _animationPlayer.Play("walk");
                    grabbableObject.Grab(true);
                    _grabbedObject = grabbableObject;
                    _grabbedObject.ForcePosition(_holdPoint.GlobalTranslation, _holdPoint.GlobalRotation);
                    Escape();
                }
                else
                {
                    if (GD.Randf() < CHANCE_GRAB_HOTDOG)
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
                    else if (GD.Randf() < CHANCE_MOVE_RANDOM)
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

        // Struggle to break free
        if (isGrabbed && !BaseScene.Inspecting())
        {
            // Struggle to release itself, if not in inspector mode
            _escapeTime -= delta;
            if (_escapeTime < 0f)
            {
                Drop();
                _stunTime = 0f;
                Cursor.ForceReleaseObject(this);
            }
        }
    }

    public override void Grab(bool disableCollision = false)
    {
        base.Grab();
        _animationPlayer.PlaybackSpeed = ANIMATION_SPEED_GRABBED;
        _escapeTime = Mathf.Lerp(ESCAPE_GRAB_TIME_MIN, ESCAPE_GRAB_TIME_MAX, GD.Randf());

        if (IsInstanceValid(_grabbedObject))
        {
            _grabbedObject.Drop();
            _grabbedObject = null; 
        }
        
        _audioPlayer.Stream = GetSqueakNoise();
        _audioPlayer.Play();        
    }

    public void Stun(float time)
    {
        _stunTime = time; 
    }

    public override void Drop()
    {
        base.Drop();
        _stunTime = STUN_TIME; 
        _animationPlayer.PlaybackSpeed = ANIMATION_SPEED_MOVE;
    }

    public bool IsGrabbing()
    {
        return _grabbedObject != null;
    }

    public bool HasValidDog()
    {
        if (_grabbedObject is Hotdog dog && dog.IsValid()) return true;
        return false; 
    }

    /**
     * Despawn self and object
     */
    public override void Despawn()
    {
        _audioPlayer.Stream = GetAlertNoise();
        _audioPlayer.Play();     
        
        if (IsInstanceValid(_grabbedObject)) _grabbedObject.QueueFree();
        QueueFree();
    }

    private void UpdateTarget(Spatial newTarget)
    {
        // LookAt(-newTarget.GlobalTranslation, Vector3.Up);
        _movementTarget = newTarget;
        _pursuitTime = MAX_PURSUIT_TIME; 
    }

    // Raycast and see if it's on the ground
    private bool IsOnGround()
    {
        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
        Dictionary hits = spaceState.IntersectRay(
            GlobalTranslation,
            GlobalTranslation + (Vector3.Down * 8f),
            null,
            1
        );
        return hits.Count > 0;
    }

    /**
     * Use when the mouse enters the search area, causes the rat to scurry to its next point 
     */
    public void Alert()
    {
        if (!IsGrabbing() && _alertTime <= 0f)
        {
            _waitTime = 0f;
            _alertTime = ALERT_DELAY; 
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