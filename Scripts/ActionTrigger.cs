using Godot;

public class ActionTrigger : Trigger
{
    [Signal]
    public delegate void OnAction();
    
    public override void OnChildEntered(Node node)
    {
        if (node is Rat)
        {
            EmitSignal(nameof(OnAction));
        }
    }
}