using Godot;
using System;

public class ComputerScreen : Control
{
    public static ComputerScreen _instance; 
    
    protected ColorRect headerRect;
    protected Label headerText;

    private ColorRect bodyRect;
    private Label bodyTopText;
    private Label bodyBottomText;

    public override void _Ready()
    {
        _instance = this; 
        
        headerRect = GetNode<ColorRect>("HeaderRect");
        headerText = headerRect.GetNode<Label>("Text");
        
        bodyRect = GetNode<ColorRect>("BodyRect");
        bodyTopText = bodyRect.GetNode<Label>("TopText");        
        bodyBottomText = bodyRect.GetNode<Label>("BottomText");        
    }

    public static void UpdateHeaderText(string newText)
    {
        if (_instance != null)
            _instance.headerText.Text = newText;
    }
    
    public static void UpdateBodyText(string newText)
    {
        if (_instance != null)
            _instance.bodyTopText.Text = newText;
    }

    public static void UpdateBodyBottomText(string newText)
    {
        if (_instance != null)
            _instance.bodyBottomText.Text = newText;
    }
}
