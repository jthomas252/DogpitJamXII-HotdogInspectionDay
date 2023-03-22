using Godot;
using System;

public class ComputerScreen : Control
{
    private const float FLASH_DURATION = 2.5f;
    
    [Export] public AudioStream errorSound;
    [Export] public AudioStream successSound;
    
    private static ComputerScreen _instance; 
    
    private ColorRect headerRect;
    private Label headerText;

    private ColorRect bodyRect;
    private Label bodyTopText;
    private Label bodyBottomText;

    private ColorRect flashRect;
    private Label flashText;

    private AudioStreamPlayer3D audioPlayer;
    private float flashDuration;

    public override void _Ready()
    {
        _instance = this; 
        
        headerRect = GetNode<ColorRect>("HeaderRect");
        headerText = headerRect.GetNode<Label>("Text");
        
        bodyRect = GetNode<ColorRect>("BodyRect");
        bodyTopText = bodyRect.GetNode<Label>("TopText");        
        bodyBottomText = bodyRect.GetNode<Label>("BottomText");

        flashRect = GetNode<ColorRect>("FlashRect");
        flashText = flashRect.GetNode<Label>("Text");
        
        audioPlayer = GetNode<AudioStreamPlayer3D>("Sound");

        flashDuration = 0;
    }

    public override void _Process(float delta)
    {
        if (flashDuration > 0f)
        {
            flashDuration -= delta;
            if (flashDuration < 0f) HideFlash();
        }
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

    public static void HideFlash()
    {
        _instance.flashRect.Visible = false;
    }

    /**
     * Pop up a message on the computer screen for a moment
     */
    public static void FlashMessage(string flashText, Color flashColor)
    {
        _instance.flashRect.Color = flashColor;
        _instance.flashRect.Visible = true;
        _instance.flashText.Text = flashText;
        _instance.flashDuration = FLASH_DURATION;
    }

    public static void FlashSuccess(string flashText)
    {
        _instance.flashRect.Color = Colors.Green;
        _instance.flashRect.Visible = true;
        _instance.flashText.Text = flashText;
        _instance.flashDuration = FLASH_DURATION;
    }

    public static void FlashError(string flashText)
    {
        _instance.flashRect.Color = Colors.Red;
        _instance.flashRect.Visible = true;
        _instance.flashText.Text = flashText;
        _instance.flashDuration = FLASH_DURATION;        
    }

    /**
     * Pop up a error message on the computer screen
     */
    public static void FlashWarningMessage(string flashText)
    {
        _instance.flashRect.Visible = true;
        _instance.flashText.Text = flashText;
        _instance.flashDuration = FLASH_DURATION;
    }

    public static void PlaySuccessSound()
    {
        if (!_instance.audioPlayer.Playing)
        {
            _instance.audioPlayer.Stream = _instance.successSound;
            _instance.audioPlayer.Play();
        }
    }
    
    public static void PlayErrorSound()
    {
        if (!_instance.audioPlayer.Playing)
        {
            _instance.audioPlayer.Stream = _instance.errorSound;
            _instance.audioPlayer.Play();
        }        
    }
    
    /**
     * Play a sound effect at the computer
     */
    public static void PlayAudioStream(AudioStream stream)
    {
        if (!_instance.audioPlayer.Playing)
        {
            _instance.audioPlayer.Stream = stream;
            _instance.audioPlayer.Play();
        }          
    }
}
