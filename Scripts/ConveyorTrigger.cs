using Godot;
using Godot.Collections;

public class ConveyorTrigger : Trigger
{
    private readonly float MOVEMENT_SCALE = 35f;
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

            if (body is Rat rat)
            {
                rat.Stun(10f);
            }
        }
    }
}