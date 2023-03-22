using Godot;
using Godot.Collections;

public class ConveyorTrigger : Trigger
{
    private readonly float MOVEMENT_SCALE = 16.5f;
    private Vector3 _direction;

    public override void _Ready()
    {
        base._Ready();
        _direction = GetNode<Spatial>("Direction").GlobalTranslation;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        Array bodies = GetOverlappingBodies();

        foreach (var body in bodies)
        {
            if (body is RigidBody rigidBody)
            {
                rigidBody.SetAxisVelocity(rigidBody.GlobalTranslation.DirectionTo(_direction) * MOVEMENT_SCALE);
            }
        }
    }
}