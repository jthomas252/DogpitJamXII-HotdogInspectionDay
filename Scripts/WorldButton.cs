using Godot;

public class WorldButton:Node
{
    [Signal]
    public delegate void Pressed();

    private InteractableObject _interactableObject;
    
    public override void _Ready()
    {
        base._Ready();
        _interactableObject = GetNode<InteractableObject>("KinematicBody");
        _interactableObject.Connect("Interacted", this, nameof(OnInteractedWith));
    }

    private void OnInteractedWith()
    {
        EmitSignal(nameof(Pressed));
    }
}
