using Godot;

public class UraniumRod : GrabbableObject
{
    private Area _area;

    public override void _Ready()
    {
        base._Ready();
        _area = GetNode<Area>("Area");

        radiation = 100f; 
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        // Find and irradiate nearby hotdogs and rats 
    }
}