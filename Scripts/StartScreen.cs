using Godot;

public class StartScreen : Control
{
    public void OnStartButton()
    {
        Fader.FadeOut("hide_start_menu");
        BaseScene.PlaySound(BaseScene.GetHotdogNoise());
    }

    // Link to signal from Fader
    public void Hide(string call)
    {
        if (call == "hide_start_menu")
        {
            Visible = false; 
        }
    }
}