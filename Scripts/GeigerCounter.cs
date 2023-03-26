using Godot;
using Godot.Collections;

public class GeigerCounter : GrabbableObject
{
    [Export] public AudioStream[] geigerClicks;

    private const float MAX_ROTATION = -2f;
    private const float MIN_ROTATION = 0f;

    private const float MIN_RADIATION = 0f;
    private const float MAX_RADIATION = 10f;

    private const float DISTANCE = 45f;

    private const float CLICK_MIN_RADIATION = 1f; 
    private const float CLICK_TIME = 0.66f; 

    private Area _detectionArea; 
    private Spatial _needle;
    private AudioStreamPlayer3D _audioPlayer;
    private float _clickTime;

    private Transform _originalTransform;
    
    public override void _Ready()
    {
        base._Ready();

        _detectionArea = GetNode<Area>("DetectionArea"); 
        _needle = GetNode<Spatial>("Needle");
        _audioPlayer = GetNode<AudioStreamPlayer3D>("Sound");
        _clickTime = 0f;

        _originalTransform = Transform;
    }

    // Just reset the original position on the desk instead of despawning
    public override void Despawn()
    {
        Transform = _originalTransform;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        float radLevel = DetectRadiation();
        _needle.Rotation = new Vector3(
            0f, 
            Mathf.Lerp(MIN_ROTATION, MAX_ROTATION, Mathf.Clamp(radLevel, MIN_RADIATION, MAX_RADIATION) / MAX_RADIATION), 
            0f
        );

        _clickTime -= delta;
        if (_clickTime < 0f && radLevel > CLICK_MIN_RADIATION)
        {
            _clickTime = CLICK_TIME;
            _audioPlayer.Stream = GetClick(radLevel);
            _audioPlayer.Play();
        }
    }

    private float DetectRadiation()
    {
        float detected = 0f;

        Array bodies = _detectionArea.GetOverlappingBodies();

        foreach (var body in bodies)
        {
            if (body is GrabbableObject grabbableObject)
            {
                detected += grabbableObject.GetRadiation() * (1f - (GlobalTranslation.DistanceTo(grabbableObject.GlobalTranslation) / DISTANCE)); 
            }
        }
        
        return detected;
    }

    private AudioStream GetClick(float radLevel)
    {
        int index = Mathf.FloorToInt((Mathf.Clamp(radLevel, MIN_RADIATION, MAX_RADIATION) / MAX_RADIATION) * (geigerClicks.Length - 1));
        return geigerClicks[index];
    }
}