using Godot;

public class RatTrigger : Trigger
{
    public override void OnChildEntered(Node node)
    {
        base.OnChildEntered(node);
        if (node is Rat rat && rat.IsGrabbing())
        {
            rat.Despawn();
            GD.Print("Rat: SO LONG SHITLORD");
        }
    }
}