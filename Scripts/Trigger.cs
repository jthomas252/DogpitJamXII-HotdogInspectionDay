using Godot;

public class Trigger : Area
{
    [Signal]
    public delegate void OnTrigger(); 
    
    [Export] public bool isKillZone = false; 
    [Export] public string tooltipText = "";

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
            if (isKillZone)
            {
                if (node is Hotdog dog && dog.IsValid())
                {
                    ComputerScreen.FlashError("VALID HOTDOG TRASHED");
                    BaseScene.DecrementScore();
                }
                
                EmitSignal(nameof(OnTrigger));
                rigidBody.QueueFree();
            }
        }
    }

    public virtual void OnChildExited(Node node)
    {
        
    }
    
    public virtual string GetTooltip()
    {
        return tooltipText; 
    }    
}
