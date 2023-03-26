using Godot; 

public class HelperText : Label3D
{
    public override void _Ready()
    {
        GetTree().CurrentScene.Connect("LevelStart", this, nameof(Hide));
        GetTree().CurrentScene.Connect("LevelReset", this, nameof(Show));
    }

    public void Hide()
    {
        Visible = false; 
    }

    public void Show()
    {
        Visible = true; 
    }
}
