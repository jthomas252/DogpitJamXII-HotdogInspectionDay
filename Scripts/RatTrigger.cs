using Godot;

public class RatTrigger : Trigger
{
    public override void OnChildEntered(Node node)
    {
        base.OnChildEntered(node);

        if (node is Rat)
        {
            
        }
    }
}