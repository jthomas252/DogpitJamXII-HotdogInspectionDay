using Godot;

public class RatSpawner : Spatial
{
    [Export] public PackedScene ratPrefab;
    
    private readonly float RAT_SPAWN_TIME_MIN = 25f;
    private readonly float RAT_SPAWN_TIME_MAX = 75f;

    private float _spawnTime;
    private Spatial _spawnPoint; 
    
    public override void _Ready()
    {
        SetSpawnTime();
        _spawnPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/RatSpawnPoint");
        
        // Force an early spawn 
        _spawnTime = 1f; 
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
        _spawnTime = Mathf.Lerp(RAT_SPAWN_TIME_MIN, RAT_SPAWN_TIME_MAX, GD.Randf());
    }
}
