using Godot;

public class ScanTrigger : Trigger
{
    public override void OnChildEntered(Node node)
    {
        base.OnChildEntered(node);

        if (node is RigidBody rigidBody)
        {
            if (rigidBody is Hotdog dog)
            {
                if (dog.IsFrozen())
                {
                    ComputerScreen.UpdateBodyText("HOTDOG TOO FROZEN");
                }
                else if (dog.IsBurnt())
                {
                    ComputerScreen.UpdateBodyText("HOTDOG TOO BURNT");
                }
                else
                {
                    ComputerScreen.UpdateBodyText(dog.GetInfo());
                }
            }
            else
            {
                ComputerScreen.UpdateBodyText("NOT HOTDOG");
            }
        } 
    }

    public override void OnChildExited(Node node)
    {
        base.OnChildExited(node);
        
        if (node is GrabbableObject grabbableObject && grabbableObject.GetParent() is Hotdog dog)
        {
            ComputerScreen.UpdateBodyText("AWAITING HOTDOG...");
        }         
    }
}