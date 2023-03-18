using Godot;
using System;

public class ComputerScreen : Control
{
    public static ComputerScreen _instance; 
    
    protected ColorRect headerRect;
    protected Label headerText;

    private ColorRect bodyRect;
    private Label bodyText;

    public override void _Ready()
    {
        _instance = this; 
        
        headerRect = GetNode<ColorRect>("HeaderRect");
        headerText = headerRect.GetNode<Label>("Text");
        
        bodyRect = GetNode<ColorRect>("BodyRect");
        bodyText = bodyRect.GetNode<Label>("Text");        
    }

    public static void UpdateHeaderText(string newText)
    {
        if (_instance != null)
            _instance.headerText.Text = newText;
    }
    
    public static void UpdateBodyText(string newText)
    {
        if (_instance != null)
            _instance.bodyText.Text = newText;
    }    
}
