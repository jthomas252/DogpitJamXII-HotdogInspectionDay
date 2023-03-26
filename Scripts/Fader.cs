using Godot;

public class Fader : ColorRect
{
    [Signal]
    public delegate void FadeApex(string callback); 
    
    private const float FADE_TIME = 1.5f; 
    
    private static Fader _instance;

    private bool _active;
    private bool _direction;
    private float _time;
    private string _callbackName; 
    
    public override void _Ready()
    {
        base._Ready();
        _instance = this; 
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_active)
        {
            _time += _direction ? delta : -delta;  
            Color = new Color(0,0,0,_time/FADE_TIME);

            // Reverse the fade once it hits the apex.
            if (_time >= FADE_TIME && _direction)
            {
                _direction = false;
                EmitSignal(nameof(FadeApex), _callbackName);
            } 
            
            // Stop the effect once it's done
            if (_time <= 0f && !_direction) _active = false; 
        }
    }

    public static void FadeOut(string callback)
    {
        _instance._active = true;
        _instance._direction = true;
        _instance._time = 0f;
        _instance._callbackName = callback; 
    }

    public static void Blackout(string callback)
    {
        _instance._active = true;
        _instance._direction = true;
        _instance._time = FADE_TIME; 
        _instance._callbackName = callback;
    }
}