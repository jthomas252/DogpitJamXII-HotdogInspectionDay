using Godot;

public class TriggerZone : Area
{
    public override void _Ready()
    {
        Connect("body_entered", this, nameof(OnChildEntered));
        Connect("body_exited", this, nameof(OnChildExited));
    }

    public virtual void OnChildEntered(Node node)
    {
        
    }

    public virtual void OnChildExited(Node node)
    {
        
    }
}
