using Godot;
using System;

public class ComputerScreen : Control
{
    private static ComputerScreen _instance; 
    
    private ColorRect headerRect;
    private Label headerText;

    private ColorRect bodyRect;
    private Label bodyTopText;
    private Label bodyBottomText;

    private ColorRect flashRect;
    private Label flashText;

    private AudioStreamPlayer3D audioPlayer;

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

    /**
     * Pop up a message on the computer screen for a moment
     */
    public static void FlashMessage(string flashText)
    {
        
    }

    /**
     * Pop up a error message on the computer screen
     */
    public static void FlashWarningMessage(string flashText)
    {
        
    }

    /**
     * Play a sound effect at the computer
     */
    public static void PlaySound(AudioStream stream)
    {
        
    }
}
