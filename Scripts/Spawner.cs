using Godot;

public class Spawner : Spatial
{
    [Export] 
    public PackedScene hotdog;

    [Export] 
    public Vector3 spawnPoint;
    
    // Re-usable list of hotdogs we have spawned in the scene already
    private int spawnsAvailable = 0;
    
    public override void _Ready()
    {
        spawnsAvailable = 10; 
    }

    public void OnSpawnButton()
    {
        if (spawnsAvailable > 0)
        {
            // Randomly choose one type of hotdog from a random roll, then drop it out the spawner
            Spatial dog = (Spatial)hotdog.Instance();

            dog.Translation = GlobalTranslation + spawnPoint;
            dog.Rotation = new Vector3(GD.Randf() * 45, GD.Randf() * 45, GD.Randf() * 45);

            GetTree().CurrentScene.AddChild(dog);

            --spawnsAvailable;
        }
        else
        {
            GD.Print("No more hotdogs!");
        }
    }
}
