using Godot;

public class ScanZone : TriggerZone
{
    public override void OnChildEntered(Node node)
    {
        base.OnChildEntered(node);

        if (node is GrabbableObject grabbableObject && grabbableObject.GetParent() is Hotdog dog)
        {
            ComputerScreen.UpdateBodyText(dog.GetInfo());
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