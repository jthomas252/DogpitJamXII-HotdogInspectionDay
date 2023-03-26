using Godot;
using Godot.Collections;

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

        // Check if any hotdogs are remaining, if not then output the default text.
        Array bodies = GetOverlappingBodies();

        foreach (var body in bodies)
        {
            if (body is Hotdog dog)
            {
                ComputerScreen.UpdateBodyText(dog.GetInfo());
                return; 
            }
        }

        ComputerScreen.UpdateBodyText("AWAITING HOTDOG...");
    }
}