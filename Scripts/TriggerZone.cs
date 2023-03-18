using Godot;
using System;

public class TriggerZone : Area
{
    public void _on_Area_area_entered(Area a)
    {
        GD.Print("How dumb");
    }
    
    public override void _Ready()
    {
        Connect("body_entered", this, nameof(OnChildEntered));
        Connect("body_exited", this, nameof(OnChildExited));
    }

    public void OnChildEntered(Node node)
    {
        // See if this contains a scannerInfo property
        if (node is GrabableObject grabableObject)
        {
            GD.Print("This has interactive properties.");
            ComputerScreen.UpdateBodyText($"${grabableObject.GlobalTranslation.ToString()}");
        }
        else
        {
            GD.Print($"Non-interactive object ${node.ToString()}");
            ComputerScreen.UpdateBodyText($"${node.ToString()}");
        }
    }

    public void OnChildExited(Node node)
    {
        ComputerScreen.UpdateBodyText(
            "###############\n" + 
            "# QUOTA       #\n" +
            "# 0 of 20     #\n" +
            "###############\n");
    }
}
