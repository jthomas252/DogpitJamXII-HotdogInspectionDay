using Godot;
using Godot.Collections;

public class RatSpawner : Spatial
{
    [Export] public PackedScene ratPrefab;

    struct SpawnRate
    {
        public float minTime { get; }
        public float maxTime { get; }
        public bool canSpawn { get; }

        public SpawnRate(bool allow, float _min, float _max)
        {
            canSpawn = allow;
            minTime = _min;
            maxTime = _max; 
        }
    }

    private SpawnRate[] _spawnRates;

    private float _spawnTime;
    private Spatial _spawnPoint; 
    
    public override void _Ready()
    {
        _spawnRates = new SpawnRate[]
        {
            new SpawnRate(false, 0f, 0f),
            new SpawnRate(false, 0f, 0f),
            new SpawnRate(true, 45f, 100f),
            new SpawnRate(true, 35f, 90f),
            new SpawnRate(true, 25f, 60f),
            new SpawnRate(true, 1f, 5f),
        };

        GetTree().CurrentScene.Connect("LevelStart", this, nameof(OnLevelStart));
        _spawnPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/RatSpawnPoint");
    }

    public void OnLevelStart()
    {
        SetSpawnTime();
    }

    public override void _Process(float delta)
    {
        if (BaseScene.IsGameActive() && _spawnTime > 0)
        {
            _spawnTime -= delta;
            if (_spawnTime < 0)
            {
                SpawnRat();
                SetSpawnTime();
            }
        }
    }

    private void SpawnRat()
    {
        Rat rat = (Rat)ratPrefab.Instance();
        GetTree().CurrentScene.AddChild(rat);
        rat.GlobalTranslation = _spawnPoint.GlobalTranslation;
        rat.GlobalRotation = _spawnPoint.GlobalRotation;
    }

    private void SetSpawnTime()
    {
        _spawnTime = Mathf.Lerp(_spawnRates[BaseScene.GetLevel()].minTime, _spawnRates[BaseScene.GetLevel()].maxTime, GD.Randf());
    }
}
