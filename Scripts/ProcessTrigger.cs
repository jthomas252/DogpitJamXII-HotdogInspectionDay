using Godot; 

public class ProcessTrigger:Trigger
{
    public override void OnChildEntered(Node node)
    {
        if (node.GetParent() is Hotdog hotdog)
        {
            GD.Print("Hotdog received");

            if (hotdog.IsValid())
            {
                BaseScene.IterateScore();
                ComputerScreen.FlashSuccess("HOTDOG ACCEPTED");
            }
            else
            {
                BaseScene.DecrementScore();
                ComputerScreen.FlashError($"HOTDOG REJECTED\n{hotdog.GetInvalidReason()}");
            }
            
            node.QueueFree();
        }
    }
}
