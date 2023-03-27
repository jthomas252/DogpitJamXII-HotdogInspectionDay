using Godot; 

/**
 * Button to end the level early once quota is hit. 
 */
public class PhoneButton : InteractableObject
{
    [Export] public NodePath light; 
    
    private static PhoneButton _instance;
    private bool _active;
    private OmniLight _light;
    
    public override void _Ready()
    {
        base._Ready();
        _instance = this;
        _active = false;
        _light = GetNode<OmniLight>(light);
        _light.Visible = false; 
    }

    public static void Activate()
    {
        if (_instance != null)
        {
            _instance._active = true;
            _instance._light.Visible = true; 
        } 
    }

    public static void Deactivate()
    {
        if (_instance != null)
        {
            _instance._active = false;
            _instance._light.Visible = false;
        } 
    }

    public override void OnInteractedWith()
    {
        if (_active) BaseScene.OnLevelEnd();
    }

    public override string GetTooltip()
    {
        return _active ? "Clock Out" : "";
    }
}
