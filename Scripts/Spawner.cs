using Godot;

public class Spawner : Spatial
{
    private static Spawner _instance; 
    
    private const float SPAWN_DELAY = 3.5f;
    private const float BETWEEN_OBJECT_DELAY = 0.33f;
    private const float DELAY_ADDED_PER_SPAWN = 0.33f;

    private const int MIN_OBJECTS = 2;
    private const int MAX_OBJECTS = 5;

    private const int RANDOM_OBJECT_LEVEL_MIN = 1;
    private const int RANDOM_OBJECT_DANGER_LEVEL_MIN = 3;  
    
    [Export] public AudioStream spawnNoise;
    [Export] public NodePath activeLight;
    
    [Export] public PackedScene hotdog;
    [Export] public PackedScene[] randomObject;
    [Export] public PackedScene[] randomObjectDanger;

    private float[] randomObjectChances = new float[]
    {
        0f,
        0.05f,
        0.21f,
        0.26f,
        0.31f,
        0.38f,
    };
    
    private Vector3 _spawnPoint;
    private AudioStreamPlayer3D _audioPlayer;

    private OmniLight _spawnLight;
    private float _spawnTime;
    private float _objectDelay;
    private int _queuedObjects;
    
    private float _extraDelay;
    private int _spawnPresses; 
    
    public override void _Ready()
    {
        _instance = this; 
        
        _spawnLight = GetNode<OmniLight>(activeLight);
        _spawnPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/SpawnPoint").GlobalTranslation;
        _audioPlayer = GetNode<AudioStreamPlayer3D>("Sound");
        _spawnTime = 0f; 
        
        GetTree().CurrentScene.Connect("LevelReset", this, nameof(Reset));
    }

    public void Reset()
    {
        _spawnPresses = 0;
        _spawnTime = 0f; 
    }

    public void OnSpawnButton()
    {
        if (_spawnTime <= 0f)
        {
            // Start the level from here. 
            if (!BaseScene.IsGameActive())
            {
                BaseScene.StartNextLevel();
            }

            _audioPlayer.Stream = spawnNoise;
            _audioPlayer.Play();

            // Queue a random number of objects
            _objectDelay = 0f;
            _queuedObjects = MIN_OBJECTS + (Mathf.Abs( (int)GD.Randi()) % (MAX_OBJECTS - MIN_OBJECTS));

            // Deactivate the light until ready again.
            _spawnLight.Visible = false;
            _spawnTime = SPAWN_DELAY + (_spawnPresses * DELAY_ADDED_PER_SPAWN);
            _spawnPresses++; 
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (_spawnTime > 0f)
        {
            _spawnTime -= delta;
            if (_spawnTime <= 0f)
            {
                _spawnLight.Visible = true; 
            }
        }

        if (_queuedObjects > 0 && _objectDelay <= 0f)
        {
            if (GD.Randf() > randomObjectChances[BaseScene.GetLevel()] || BaseScene.GetLevel() < RANDOM_OBJECT_LEVEL_MIN)
            {
                // Spawn hotdog
                SpawnObject(hotdog);
            }
            else
            {
                // Get random object
                SpawnObject(GetRandomObject());
            }
            _objectDelay = BETWEEN_OBJECT_DELAY;
            _queuedObjects--; 
        }
        else
        {
            _objectDelay -= delta; 
        }
    }

    private PackedScene GetRandomObject()
    {
        if (BaseScene.GetLevel() >= RANDOM_OBJECT_DANGER_LEVEL_MIN)
        {
            return randomObjectDanger[GD.Randi() % randomObjectDanger.Length];
        }
        return randomObject[GD.Randi() % randomObject.Length];
    }

    /**
     * Use for spawning any non-hotdog objects
     * Assumes that there's at least a Spatial on them
     */
    public void SpawnObject(PackedScene scene)
    {
        Spatial obj = (Spatial)scene.Instance();
        GetTree().CurrentScene.AddChild(obj);
        GD.Print($"Object named {obj.Name} spawned at ${_spawnPoint.ToString()}");
        obj.GlobalTranslation = _spawnPoint;
        obj.GlobalRotation = new Vector3(GD.Randf(), GD.Randf(), GD.Randf());        
    }

    public static void Spawn(PackedScene scene)
    {
        _instance.SpawnObject(scene);
    }
}
