using Godot;

public class WorldButton:Node
{
    [Signal]
    public delegate void Pressed();

    private InteractableObject _interactableObject;
    private AudioStreamPlayer3D _sound;
    
    public override void _Ready()
    {
        base._Ready();
        _interactableObject = GetNode<InteractableObject>("KinematicBody");
        _interactableObject.Connect("Interacted", this, nameof(OnInteractedWith));

        _sound = GetNode<AudioStreamPlayer3D>("Sound");
    }

    private void OnInteractedWith()
    {
        EmitSignal(nameof(Pressed));
        _sound.Play();
        GD.Print(_sound.Playing.ToString());
    }
}
