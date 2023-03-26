using Godot;

public class EndScreen : Control
{
    public void OnFadeApex(string callback)
    {
        if (callback == "show_end")
        {
            Visible = true; 
        }
    }
}
