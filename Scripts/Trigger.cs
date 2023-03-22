using Godot;

public class Trigger : Area
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

        if (node is RigidBody rigidBody)
        {
            GD.Print($"Hit at {rigidBody.LinearVelocity}");
        }
        
        if (isKillZone && node is GrabbableObject) node.QueueFree();
    }

    public virtual void OnChildExited(Node node)
    {
        
    }
}
