using Godot;

public class RatTrigger : Trigger
{
    public override void OnChildEntered(Node node)
    {
        base.OnChildEntered(node);
        if (node is Rat rat && rat.IsGrabbing())
        {
            if (rat.HasValidDog()) BaseScene.IterateRatLoss();
            rat.Despawn();
            GD.Print("Rat: SO LONG SHITLORD");
        }
    }
}