using Godot;
using Godot.Collections;

public class UraniumRod : GrabbableObject
{
    private const float RAD_LEVEL = 3f; 
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
        Array bodies = _area.GetOverlappingBodies();
        foreach (var body in bodies)
        {
            if (body is Hotdog dog)
            {
                dog.ApplyRads((RAD_LEVEL / GlobalTranslation.DistanceTo(dog.GlobalTranslation)) * delta);
            }
        }
    }
}