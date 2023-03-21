using Godot;

public class Spawner : Spatial
{
    [Export] 
    public PackedScene hotdog;

    private Vector3 _spawnPoint; 

    // Re-usable list of hotdogs we have spawned in the scene already
    private int _spawnsAvailable = 0;
    
    public override void _Ready()
    {
        _spawnsAvailable = 10;
        _spawnPoint = GetTree().CurrentScene.GetNode<Spatial>("Points/SpawnPoint").GlobalTranslation;
    }

    public void OnSpawnButton()
    {
        if (_spawnsAvailable > 0)
        {
            // Randomly choose one type of hotdog from a random roll, then drop it out the spawner
            Hotdog dog = (Hotdog)hotdog.Instance();
            GetTree().CurrentScene.AddChild(dog);
            GD.Print($"Hotdog spawned at ${_spawnPoint.ToString()}");
            dog.GlobalTranslation = _spawnPoint;
            dog.GlobalRotation = new Vector3(GD.Randf(), GD.Randf(), GD.Randf());
            --_spawnsAvailable;
        }
        else
        {
            GD.Print("No more hotdogs!");
            ComputerScreen.PlayErrorSound();
        }
    }
}
