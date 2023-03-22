using Godot; 

public class SnapTrigger:Area
{
    private Vector3 _snapPoint;
    
    public override void _Ready()
    {
        _snapPoint = GetNode<Spatial>("SnapPoint").GlobalTranslation;
    }

    public Vector3 GetSnapPoint()
    {
        return _snapPoint;
    }
}
