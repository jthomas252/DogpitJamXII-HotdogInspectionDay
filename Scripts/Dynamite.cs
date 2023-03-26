using Godot;

public class Dynamite : GrabbableObject
{
    [Export] public AudioStream explosionNoise; 
    
    private const float EXPLODE_TIME = 10f;

    private float _explodeTime;
    private bool _exploded;
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        base._Ready();
        _explodeTime = EXPLODE_TIME;
        _exploded = false; 

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.GetAnimation("burn_fuse");
        _animationPlayer.PlaybackSpeed = 1f / EXPLODE_TIME; // Animation is 1s, so divide the total time. 
        _animationPlayer.Play("burn_fuse");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (!_exploded)
        {
            _explodeTime -= delta;
            if (_explodeTime <= 0f)
            {
                _exploded = true;
                Explode();
            }
        }
    }

    // Uh-oh
    public void Explode()
    {
        BaseScene.PlaySound(explosionNoise, 0f);
        BaseScene.GameOver("You have been exploded.");
        Cursor.ForceReleaseObject(this);
        Fader.Blackout("show_stat_menu");
    }
}