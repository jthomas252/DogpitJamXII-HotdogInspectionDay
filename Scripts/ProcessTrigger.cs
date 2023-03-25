using Godot; 

public class ProcessTrigger:Trigger
{
    [Signal]
    public delegate void OnProcess(); 
    
    public override void OnChildEntered(Node node)
    {
        if (node is RigidBody rigidBody)
        {
            if (rigidBody is Hotdog hotdog)
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
            }
            else
            {
                ComputerScreen.FlashError("NOT HOTDOG");
            }
            
            EmitSignal(nameof(OnProcess));
            node.QueueFree();
        }
    }
}
