using Godot;

/**
 * Call this to modify shaders and screen states
 */
public class Computer : Spatial
{
    private static Computer _instance; 
    private Spatial _screen; 
    
    public override void _Ready()
    {
        _instance = this;
        _screen = GetNode<Spatial>("Screen");
        _screen.Visible = false; 
    }

    public static void ActivateScreen()
    {
        _instance._screen.Visible = true;
    }

    public static void DeactiveScreen()
    {
        _instance._screen.Visible = false; 
    }
}