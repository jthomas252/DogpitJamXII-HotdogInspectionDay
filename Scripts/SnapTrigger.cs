using Godot; 

public class SnapTrigger:Area
{
    private Spatial _snapPoint;
    
    public override void _Ready()
    {
        _snapPoint = GetNode<Spatial>("SnapPoint");
    }

    public Vector3 GetSnapPoint()
    {
        return _snapPoint.GlobalTranslation;
    }

    public Vector3 GetSnapRotation()
    {
        return _snapPoint.GlobalRotation;
    }
}
