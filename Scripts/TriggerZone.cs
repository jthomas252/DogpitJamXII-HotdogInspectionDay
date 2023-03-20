using Godot;

public class TriggerZone : Area
{
    [Export] public bool isKillZone = false; 
    
    public override void _Ready()
    {
        Connect("body_entered", this, nameof(OnChildEntered));
        Connect("body_exited", this, nameof(OnChildExited));
    }

    public virtual void OnChildEntered(Node node)
    {
        GD.Print($"Node entered ${node.ToString()}");
        if (isKillZone) node.QueueFree();
    }

    public virtual void OnChildExited(Node node)
    {
        
    }
}
