using Godot;

public class Vent : Spatial
{
    private AnimationPlayer _animationPlayer;
    private AudioStreamPlayer3D _audioPlayer;
    private CollisionShape _collision;
    private bool _played;
    
    public override void _Ready()
    {
        _collision = GetNode<CollisionShape>("CollisionShape");
        _audioPlayer = GetNode<AudioStreamPlayer3D>("Sound");
        _animationPlayer = GetNode<AnimationPlayer>("RootNode/AnimationPlayer");
        _animationPlayer.Play("unbroken");
        _played = false;
    }

    public void OnAction()
    {
        if (!_played)
        {
            _played = true; 
            _collision.Disabled = true;
            _audioPlayer.Play();
            _animationPlayer.Play("smash");
        }
    }
}