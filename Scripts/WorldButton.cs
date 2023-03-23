using Godot;

public class WorldButton:Node
{
    [Signal] public delegate void Pressed();

    [Export] public NodePath animationPlayerPath;
    [Export] public string animationName;
    
    private InteractableObject _interactableObject;
    private AudioStreamPlayer3D _sound;
    private AnimationPlayer _animationPlayer; 
    
    public override void _Ready()
    {
        base._Ready();
        _interactableObject = GetNode<InteractableObject>("KinematicBody");
        _interactableObject.Connect("Interacted", this, nameof(OnInteractedWith));
        _sound = GetNode<AudioStreamPlayer3D>("Sound");
        
        if (animationPlayerPath != null) _animationPlayer = GetNodeOrNull<AnimationPlayer>(animationPlayerPath);
    }

    private void OnInteractedWith()
    {
        _animationPlayer?.Play(animationName);
        EmitSignal(nameof(Pressed));
        _sound.Play();
    }
}
